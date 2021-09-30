using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace PAX_Consumer
{
    class PAX_Consumer_PublicDataFunctions
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
                PAX_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PAX_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    PAX_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((PAX_Consumer_Globals.iAgencyID = PAX_Consumer_Globals.SetAgencyID(PAX_Consumer_Globals.sAgencyName)) == 0)
                            {
                                PAX_Consumer_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + PAX_Consumer_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                                return false;
                            }
                        }
                    }

                }

                PAX_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_PublicDataFunctions.GetAgencyInfo()");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetAirportInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _airport_code from db_objects.sp_getairportinfo(); ";

            try
            {
                PAX_Consumer_Globals.dictAirportInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PAX_Consumer_Globals.dictAirportInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    PAX_Consumer_Globals.dictAirportInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((PAX_Consumer_Globals.iAirportID = PAX_Consumer_Globals.SetAirportID(PAX_Consumer_Globals.sAirportCode)) == 0)
                            {
                                PAX_Consumer_Globals.LogFile.LogErrorText("Error setting airport ID for code " + PAX_Consumer_Globals.sAirportCode + ". Cannot handle any functions for this airport.");
                                return false;
                            }
                        }
                    }

                }

                PAX_Consumer_Globals.LogFile.Log("Successfully read airport data in GetAirportInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Producer_PublicDataFunctions.GetAirportInfo()");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetPAXTypeInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _pax_type from alertsproducerds.sp_getpaxtypeinfo(); ";

            try
            {
                PAX_Consumer_Globals.dictPAXType.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PAX_Consumer_Globals.dictPAXType.ContainsKey(rsRead[1].ToString()))
                                {
                                    PAX_Consumer_Globals.dictPAXType.Add(rsRead[1].ToString(), Convert.ToInt32(rsRead[0].ToString()));
                                }
                            }
                        }
                    }

                    //Now set variables
                    PAX_Consumer_Globals.iAADailyProjectionsTypeID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sAAHourlyProjectionTypeName];
                    PAX_Consumer_Globals.iDeltaDailyProjectionsTypeID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sDeltaHourlyProjectionTypeName];
                    PAX_Consumer_Globals.iDeltaHourlyProjectionsTypeID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sDeltaHourlyProjectionTypeName];
                    PAX_Consumer_Globals.iDeltaMonthlyProjectionsTypeID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sDeltaMonthlyProjectionTypeName];
                    PAX_Consumer_Globals.iTSAEnplanementsTypeID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sTSAEnplanementsTypeName];
                    PAX_Consumer_Globals.iCBPPAXTypeID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sCBPAXTypeName];
                    PAX_Consumer_Globals.iTerminalBProjectionsID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sTerminalBProjectionsTypeName];
                    PAX_Consumer_Globals.iDeltaSixDayDetailedProjectionsID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sDeltaSixDayDetailedProjectionsTypeName];
                    PAX_Consumer_Globals.iJetBlueDailyProjectionsID = PAX_Consumer_Globals.dictPAXType[PAX_Consumer_Globals.sJetBlueDailyProjectionsTypeName];

                }

                PAX_Consumer_Globals.LogFile.Log("Successfully read PAX type data in GetPAXTypeInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_PublicDataFunctions.GetPAXTypeInfo(). Please verify database rows and config PAX type names match.");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetPAXTypeAirlineInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _pax_type_id, _airline_id, _airline_code, _airline_name from alertsproducerds.sp_getairlinepaxtypeinfo(); ";

            try
            {
                PAX_Consumer_Globals.dictPAXTypeAirline.Clear();
                PAX_Consumer_Globals.dictAirlineIDLookup.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PAX_Consumer_Globals.dictPAXTypeAirline.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    PAX_Consumer_Globals.dictPAXTypeAirline.Add(Convert.ToInt32(rsRead[0].ToString()), Convert.ToInt32(rsRead[1].ToString()));
                                }

                                if (!PAX_Consumer_Globals.dictAirlineIDLookup.ContainsKey(rsRead[2].ToString())){
                                    PAX_Consumer_Globals.dictAirlineIDLookup.Add(rsRead[2].ToString(), Convert.ToInt32(rsRead[1]));
                                }
                            }
                        }
                    }

                }

                PAX_Consumer_Globals.LogFile.Log("Successfully read PAX type data in GetPAXTypeAirlineInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_PublicDataFunctions.GetPAXTypeInfo()");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetFlightTypeInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _flight_type_id, _flight_type from alertsproducerds.sp_getflighttypeinfo(); ";

            try
            {
                PAX_Consumer_Globals.dictFlightType.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!PAX_Consumer_Globals.dictFlightType.ContainsKey(rsRead[1].ToString()))
                                {
                                    PAX_Consumer_Globals.dictFlightType.Add(rsRead[1].ToString(), Convert.ToInt32(rsRead[0].ToString()));
                                }
                            }
                        }
                    }

                }

                PAX_Consumer_Globals.LogFile.Log("Successfully read Flight Type data in GetFlightTypeInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_PublicDataFunctions.GetFlightTypeInfo()");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
