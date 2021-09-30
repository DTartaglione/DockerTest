using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace PA_Alerts_Data_Consumer
{
    class PA_Alerts_Data_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            //if (GetAgencyInfo())
            //{
            //    return true;  
            //}
            return true;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                PA_Alerts_Data_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PA_Alerts_Data_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    PA_Alerts_Data_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((PA_Alerts_Data_Consumer_Globals.iAgencyID = PA_Alerts_Data_Consumer_Globals.SetAgencyID(PA_Alerts_Data_Consumer_Globals.sAgencyName)) == 0)
                            {
                                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + PA_Alerts_Data_Consumer_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                                return false;
                            }
                        }
                    }

                }

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_PublicDataFunctions.GetAgencyInfo()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetAOCInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _aoc_name from pads.sp_getAOCinfo(); ";

            try
            {
                PA_Alerts_Data_Consumer_Globals.dictAOCInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PA_Alerts_Data_Consumer_Globals.dictAOCInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    PA_Alerts_Data_Consumer_Globals.dictAOCInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((PA_Alerts_Data_Consumer_Globals.iAOCID = PA_Alerts_Data_Consumer_Globals.SetAOCID(PA_Alerts_Data_Consumer_Globals.sAOCName)) == 0)
                            {
                                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error setting AOC ID for AOC: " + PA_Alerts_Data_Consumer_Globals.sAOCName + ". Cannot handle any functions for this agency.");
                                return false;
                            }
                        }
                    }

                }

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully read aoc data in GetAOCInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_PublicDataFunctions.GetAOCInfo()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetEmailTypeInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _type_name from alertsproducerds.sp_getemailtypeinfo(" + PA_Alerts_Data_Consumer_Globals.iAgencyID + "); ";

            try
            {
                PA_Alerts_Data_Consumer_Globals.dictEmailType.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsKey(rsRead[1].ToString()))
                                {
                                    PA_Alerts_Data_Consumer_Globals.dictEmailType.Add(rsRead[1].ToString(), Convert.ToInt32(rsRead[0].ToString()));
                                }
                            }
                        }
                    }

                }

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully read PAX type data in GetEmailTypeInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_PublicDataFunctions.GetPAXTypeInfo()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetActiveAlerts()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _last_update from pads.sp_getactivealerts();";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                PA_Alerts_Data_Consumer_Globals.dictActiveAlerts.Add(Convert.ToInt32(rsRead[0].ToString()), Convert.ToDateTime(rsRead[1].ToString()));
                            }
                        }
                    }

                }

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully stored active alert data to dictionary dictActiveAlerts in GetActiveAlerts().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_PublicDataFunctions.GetActiveAlerts()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool DeactivateAlert(int iAlertID, int iDisableOlderThanValueHours)
        {
            bool bSuccess = false;
            string sSQL = "pads.sp_deactivatealert";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_alert_id", NpgsqlDbType.Integer, iAlertID);
                        cmdSQL.Parameters.AddWithValue("_disable_older_than_value_hours", NpgsqlDbType.Integer, iDisableOlderThanValueHours);

                        cmdSQL.ExecuteNonQuery();
                    }

                }

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_PublicDataFunctions.DeactivateAlert()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetLatestPAXUpdateInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _airline_code, _last_update from pads.sp_getlatestpaxupdateinfo();";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PA_Alerts_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PA_Alerts_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                PA_Alerts_Data_Consumer_Globals.dictLatestPAXUpdate.Add(rsRead[0].ToString(), Convert.ToDateTime(rsRead[1].ToString()));
                            }
                        }
                    }

                }

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully stored latest PAX update per airline info to dictionary dictLatestPAXUpdate in GetLatestPAXUpdateInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_PublicDataFunctions.GetActiveAlerts()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
