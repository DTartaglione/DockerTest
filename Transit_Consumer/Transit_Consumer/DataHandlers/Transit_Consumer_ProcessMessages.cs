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
    class Transit_Consumer_ProcessMessages
    {
        public static bool InsertMessage(Messages _Message)
        {
            bool bResult = false;
            string sSQL = "transitds.sp_updatetransitmessages";

            try
            {

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                       // cmdSQL.Parameters.AddWithValue("_route_id", NpgsqlDbType.Varchar, _Prediction.Route);
                        
                        //cmdSQL.Parameters.AddWithValue("_event_date_time", NpgsqlDbType.Timestamp, Transit_Consumer_Globals.FromUnixTimeMillis(_Prediction.EpochTime).ToLocalTime());

                       // cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in InsertMessage().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                bResult = false;
            }

            return bResult;
        }

    }
}
