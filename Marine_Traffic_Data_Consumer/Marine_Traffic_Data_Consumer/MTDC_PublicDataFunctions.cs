using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Marine_Traffic_Data_Consumer
{
    class MTDC_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            //if (GetSensorZoneInfo())
            //{
            //    if (GetSensorDeviceInfo())
            //    {
            //        return true;
            //    }
           // }
            return true;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                MTDC_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(MTDC_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = MTDC_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!MTDC_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    MTDC_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((MTDC_Globals.iAgencyID = MTDC_Globals.SetAgencyID(MTDC_Globals.sAgencyName)) == 0)
                            {
                                MTDC_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + MTDC_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                                return false;
                            }
                        }
                    }

                }

                MTDC_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                MTDC_Globals.LogFile.LogErrorText("Error in MTDC_PublicDataFunctions.GetAgencyInfo()");
                MTDC_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }
    }
}
