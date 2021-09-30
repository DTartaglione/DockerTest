using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace VMS_Consumer
{
    class VMS_Consumer_ProcessVMS
    {
        public bool UpdateVMS(VMSData _VMS)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_updatevms";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(VMS_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = VMS_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Integer, _VMS.AgencyID);
                        cmdSQL.Parameters.AddWithValue("_native_vms_id", NpgsqlDbType.Varchar, _VMS.SourceVMSID);
                        cmdSQL.Parameters.AddWithValue("_vms_name", NpgsqlDbType.Varchar, _VMS.VMSName);
                        cmdSQL.Parameters.AddWithValue("_roadway", NpgsqlDbType.Varchar, _VMS.Roadway);
                        cmdSQL.Parameters.AddWithValue("_direction", NpgsqlDbType.Varchar, _VMS.Direction);
                        cmdSQL.Parameters.AddWithValue("_latitude", NpgsqlDbType.Double, _VMS.Coords.Latitude);
                        cmdSQL.Parameters.AddWithValue("_longitude", NpgsqlDbType.Double, _VMS.Coords.Longitude);
                        cmdSQL.Parameters.AddWithValue("_message", NpgsqlDbType.Varchar, _VMS.Message);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.TimestampTZ, _VMS.LastUpdate);

                        cmdSQL.ExecuteNonQuery();

                        //Add to VMS ID map - first clear original if exists
                        if (VMS_Consumer_Globals.dictVMSIDMap.ContainsKey(_VMS.AgencyID + "|" + _VMS.SourceVMSID))
                        {
                            VMS_Consumer_Globals.dictVMSIDMap.Remove(_VMS.AgencyID + "|" + _VMS.SourceVMSID);
                        }

                        VMS_Consumer_Globals.dictVMSIDMap.Add(_VMS.AgencyID + "|" + _VMS.SourceVMSID, _VMS);
                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                VMS_Consumer_Globals.LogFile.LogErrorText("Error in UpdateVMS().");
                VMS_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                VMS_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
