using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using NpgsqlTypes;


namespace PA_Alerts_Data_Consumer
{
    class PA_Alerts_Data_Consumer_ProcessData
    {

        public static bool UpdateAOCTrafficStatus(AOC_Traffic_Status _AOC_Traffic_Status)
        {
            bool bResult = false;
            string sSQL = "pads.sp_updateaoctrafficstatus";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_aoc_id", NpgsqlDbType.Integer, PA_Alerts_Data_Consumer_Globals.iAOCID);
                        cmdSQL.Parameters.AddWithValue("_code_id", NpgsqlDbType.Integer, _AOC_Traffic_Status.CodeID);
                        cmdSQL.Parameters.AddWithValue("_code_name", NpgsqlDbType.Varchar, _AOC_Traffic_Status.CodeName);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _AOC_Traffic_Status.LastUpdate);
                        
                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_ProcessData.UpdateAOCTrafficStatus()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        public static int InsertAlert(Alerts _Alerts)
        {
            string sSQL = "pads.sp_insertalert";
            int iAlertID = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_aoc_id", NpgsqlDbType.Integer, PA_Alerts_Data_Consumer_Globals.iAOCID);
                        cmdSQL.Parameters.AddWithValue("_alert_message", NpgsqlDbType.Varchar, _Alerts.AlertMessage);
                        cmdSQL.Parameters.AddWithValue("_create_timestamp", NpgsqlDbType.Timestamp, _Alerts.CreateTimestamp);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _Alerts.LastUpdate);

                        if (_Alerts.Geom != "")
                        {
                            cmdSQL.Parameters.AddWithValue("_geom", NpgsqlDbType.Varchar, _Alerts.Geom);
                        }
                        else
                        {
                            cmdSQL.Parameters.AddWithValue("_geom", DBNull.Value);
                        }

                        cmdSQL.Parameters.AddWithValue("_email_type_id", NpgsqlDbType.Integer, _Alerts.EmailTypeID);

                        iAlertID = Convert.ToInt32(cmdSQL.ExecuteScalar());

                        PA_Alerts_Data_Consumer_Globals.dictActiveAlerts.Add(iAlertID, _Alerts.LastUpdate);

                        return iAlertID;
                    }
                }

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_ProcessData.InsertAlert()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return iAlertID;
        }

        public static bool UpdatePAXVolume(PAXUpdate _PAXUpdate)
        {
            bool bResult = false;
            string sSQL = "pads.sp_updatepaxvolume";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_aoc_id", NpgsqlDbType.Integer, PA_Alerts_Data_Consumer_Globals.iAOCID);
                        cmdSQL.Parameters.AddWithValue("_airline_code", NpgsqlDbType.Varchar, _PAXUpdate.AirlineCode);
                        cmdSQL.Parameters.AddWithValue("_hour_timestamp", NpgsqlDbType.Timestamp, _PAXUpdate.HourTimestamp);
                        cmdSQL.Parameters.AddWithValue("_arrv_volume", NpgsqlDbType.Integer, _PAXUpdate.ArrvVolume);
                        cmdSQL.Parameters.AddWithValue("_dept_volume", NpgsqlDbType.Integer, _PAXUpdate.DeptVolume);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _PAXUpdate.LastUpdate);
                        //cmdSQL.Parameters.AddWithValue("_pax_type", NpgsqlDbType.Varchar, _PAXUpdate.PAXType);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_ProcessData.UpdatePAXVolume()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }
    }
}
