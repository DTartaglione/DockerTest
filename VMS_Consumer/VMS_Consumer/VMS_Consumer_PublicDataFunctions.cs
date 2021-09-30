using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace VMS_Consumer
{
    class VMS_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            if (GetAgencyInfo())
            {
                return true;
            }

            return false;
        }

        public static bool GetCurrentDynamicData()
        {
            if (GetCurrentVMSData())
            {
                return true;
            }
            return false;
        }

        public static bool GetCurrentVMSData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select _id, _native_vms_id, _agency_id, _latitude, _longitude, _vms_name, " +
                "_roadway, _direction, _message, _last_update FROM db_objects.sp_getVMS();";// + 
                //"where _agency_id = " + VMS_Consumer_Globals.iAgencyID + ";";
            VMSData _VMS = new VMSData();
            Point _Coords = new Point();
            DateTime dtTmpDate = new DateTime();

            try
            {
                VMS_Consumer_Globals.dictVMSIDMap.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(VMS_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = VMS_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                if (!VMS_Consumer_Globals.dictVMSIDMap.ContainsKey(rsRead[2].ToString() + "|" + rsRead[1].ToString()))
                                {
                                    _VMS = new VMSData();

                                    _VMS.LocalVMSID = Convert.ToInt32(rsRead[0]);
                                    _VMS.SourceVMSID = rsRead[1].ToString();
                                    _VMS.AgencyID = Convert.ToInt32(rsRead[2]);

                                    _Coords.Latitude = Convert.ToDouble(rsRead[3]);
                                    _Coords.Longitude = Convert.ToDouble(rsRead[4]);
                                    _VMS.Coords = _Coords;

                                    _VMS.VMSName = rsRead[5].ToString();
                                    _VMS.Roadway = rsRead[6].ToString();
                                    _VMS.Direction = rsRead[7].ToString();
                                    _VMS.Message = rsRead[8].ToString();
                                    _VMS.LastUpdate = string.IsNullOrEmpty(rsRead[9].ToString()) ? dtTmpDate : Convert.ToDateTime(rsRead[9].ToString());

                                    VMS_Consumer_Globals.dictVMSIDMap.Add(rsRead[2].ToString() + "|" + rsRead[1].ToString(), _VMS);
                                    _VMS = null;
                                }
                            }
                        }
                    }

                }

                VMS_Consumer_Globals.LogFile.Log("Successfully read VMS data in GetCurrentVMSData().");
                bSuccess = true;

            }
            catch (Exception ex)
            {
                VMS_Consumer_Globals.LogFile.LogErrorText("Error in VMS_Consumer_PublicDataFunctions.GetCurrentVMSData()");
                VMS_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                VMS_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(VMS_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = VMS_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!VMS_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    VMS_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }
                        }
                    }

                }

                VMS_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                VMS_Consumer_Globals.LogFile.LogErrorText("Error in GetAgencyInfo()");
                VMS_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
