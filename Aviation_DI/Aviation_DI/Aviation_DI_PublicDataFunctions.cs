using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Aviation_DI
{
    class Aviation_DI_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            if (GetExistingFlightData())
            {
                //if (GetAgencyInfo())
                //{
                    return true;  
                // }
            }

            return false;
        }

        public static bool GetExistingFlightData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select id, flight_number, last_update from dynamicds.tblflightstatus where flight_status <> 'landed';";
            Aviation_DI_StoredFlightData _StoredFlight = new Aviation_DI_StoredFlightData();
            Aviation_DI_FlightData _FlightData = new Aviation_DI_FlightData();

            try
            {
                Aviation_DI_Globals.dictStoredFlightData.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Aviation_DI_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Aviation_DI_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                if (!Aviation_DI_Globals.dictStoredFlightData.ContainsKey(rsRead[1].ToString()))
                                {
                                    _StoredFlight = new Aviation_DI_StoredFlightData();
                                    _FlightData = new Aviation_DI_FlightData();
                                    _FlightData.FlightNum = rsRead[1].ToString();
                                    _FlightData.LastUpdate = Convert.ToDateTime(rsRead[2].ToString());
                                    _StoredFlight.DoUpdate = false;
                                    _StoredFlight.StoredFlight = _FlightData;


                                    Aviation_DI_Globals.dictStoredFlightData.Add(rsRead[1].ToString(), _StoredFlight);
                                }
                            }

                        }
                    }

                }

                Aviation_DI_Globals.LogFile.Log("Successfully read flight data in GetExistingFlightData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Aviation_DI_Globals.LogFile.LogErrorText("Error in Aviation_DI_PublicDataFunctions.GetExistingFlightData()");
                Aviation_DI_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        //public static bool GetAgencyInfo()
        //{
        //    bool bSuccess = false;
        //    NpgsqlDataReader rsRead = null;
        //    string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

        //    try
        //    {
        //        Aviation_DI_Globals.dictAgencyInfo.Clear();

        //        using (NpgsqlConnection sConn = new NpgsqlConnection(Aviation_DI_Globals.sPG_MOD_DBConn))
        //        {
        //            sConn.Open();

        //            using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
        //            {
        //                cmdSQL.CommandTimeout = Aviation_DI_Globals.iDBCommandTimeoutInSeconds;

        //                rsRead = cmdSQL.ExecuteReader();

        //                if (rsRead.HasRows)
        //                {
        //                    while (rsRead.Read()) {      

        //                        if (!Aviation_DI_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
        //                        {
        //                            Aviation_DI_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
        //                        }
        //                    }

        //                    if ((Aviation_DI_Globals.iAgencyID = Aviation_DI_Globals.SetAgencyID(Aviation_DI_Globals.sAviationAgencyName)) == 0)
        //                    {
        //                        Aviation_DI_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + Aviation_DI_Globals.sAviationAgencyName + ". Cannot handle any functions for this agency.");
        //                        return false;
        //                    }
        //                }
        //            }

        //        }

        //        Aviation_DI_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

        //        bSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        Aviation_DI_Globals.LogFile.LogErrorText("Error in Aviation_DI_PublicDataFunctions.GetAgencyInfo()");
        //        Aviation_DI_Globals.LogFile.LogError(ex);
        //    }

        //    return bSuccess;
        //}

    }
}
