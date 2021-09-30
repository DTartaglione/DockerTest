using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transit_Consumer
{
    class Transit_Consumer_ProcessPredictions
    {

        public static bool InsertPrediction(Predictions _Prediction)
        {
            bool bResult = false;
            string sSQL = "transitds.sp_updatetransitprediction";

            try
            {

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_route_id", NpgsqlDbType.Varchar, _Prediction.Route);
                        cmdSQL.Parameters.AddWithValue("_stop_code", NpgsqlDbType.Varchar, _Prediction.StopTag);
                        cmdSQL.Parameters.AddWithValue("_is_departure", NpgsqlDbType.Boolean, _Prediction.IsDeparture);
                        cmdSQL.Parameters.AddWithValue("_minutes", NpgsqlDbType.Integer, _Prediction.Minutes);
                        cmdSQL.Parameters.AddWithValue("_seconds", NpgsqlDbType.Integer, _Prediction.Seconds);
                        cmdSQL.Parameters.AddWithValue("_vehicle_id", NpgsqlDbType.Varchar, _Prediction.Vehicle);
                        cmdSQL.Parameters.AddWithValue("_block_id", NpgsqlDbType.Varchar, _Prediction.Block);
                        cmdSQL.Parameters.AddWithValue("_direction", NpgsqlDbType.Varchar, _Prediction.DirTag);
                        cmdSQL.Parameters.AddWithValue("_event_date_time", NpgsqlDbType.Timestamp, Transit_Consumer_Globals.FromUnixTimeMillis(_Prediction.EpochTime).ToLocalTime());

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in InsertPrediction().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                bResult = false;
            }

            return bResult;
        }
    }
}
