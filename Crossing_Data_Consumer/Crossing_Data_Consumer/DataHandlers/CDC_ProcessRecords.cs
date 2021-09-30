using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Crossing_Data_Consumer
{
    class Crossing_Data_Consumer_ProcessCrossingRecords
    {
        public static bool ProcessCrossingRecords(CrossingData _CrossingData)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertcrossingdata";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Crossing_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Crossing_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_facility_id", NpgsqlDbType.Integer, _CrossingData.LocalFacilityId);
                        cmdSQL.Parameters.AddWithValue("_ca_wait_time_in_mins", NpgsqlDbType.Integer, _CrossingData.CAWaitTimeMins);
                        cmdSQL.Parameters.AddWithValue("_us_wait_time_in_mins", NpgsqlDbType.Integer, _CrossingData.USWaitTimeMins);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _CrossingData.LastUpdate);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error in UpdateVRCRecord().");
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Crossing_Data_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
