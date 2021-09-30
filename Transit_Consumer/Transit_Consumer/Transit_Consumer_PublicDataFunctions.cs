using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Transit_Consumer
{
    class Transit_Consumer_PublicDataFunctions
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
            if (GetCurrentVehiclePositionData())
            {
                if (GetCurrentTripUpdateData())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                Transit_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!Transit_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    Transit_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }
                        }
                    }

                }

                Transit_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in GetAgencyInfo()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetCurrentVehiclePositionData()
        {
            //method to refresh the vehicle position data in the dictionary. Only store in memory the data within the last two days
            //so that it doesn't grow out of control. This will allow us to compare vehicle positions with last update to ensure 
            //they have changed before we do anything with the data
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _vehicle_label, _trip_id, _route_id, _stop_id, _last_update from transitds.sp_getgtfsvehiclepositions();";

            try
            {
                Transit_Consumer_Globals.dictStoredVehicleLocations.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!Transit_Consumer_Globals.dictStoredVehicleLocations.ContainsKey(rsRead[1].ToString() + "|" +
                                    rsRead[2].ToString() + "|" + rsRead[3].ToString() + "|" + rsRead[4].ToString()))
                                {
                                    Transit_Consumer_Globals.dictStoredVehicleLocations.Add(rsRead[1].ToString() + "|" +
                                    rsRead[2].ToString() + "|" + rsRead[3].ToString() + "|" + rsRead[4].ToString(), Convert.ToDateTime(rsRead[5]));
                                }
                            }

                        }
                    }

                }

                Transit_Consumer_Globals.LogFile.Log("Successfully read agency data in GetCurrentVehiclePositionData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in GetCurrentVehiclePositionData()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetCurrentTripUpdateData()
        {
            //method to refresh the trip update data in the dictionary. Only store in memory the data within the last two days
            //so that it doesn't grow out of control. This will allow us to compare trip updates with last update to ensure 
            //they have changed before we do anything with the data
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _vehicle_label, _trip_id, _route_id, _stop_id, _stop_time_update_id, " +
                "_last_update from transitds.sp_getgtfstripupdates();";

            try
            {
                Transit_Consumer_Globals.dictStoredTripUpdates.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!Transit_Consumer_Globals.dictStoredTripUpdates.ContainsKey(rsRead[1].ToString() + "|" +
                                    rsRead[2].ToString() + "|" + rsRead[3].ToString() + "|" + rsRead[4].ToString()))
                                {
                                    Transit_Consumer_Globals.dictStoredTripUpdates.Add(rsRead[1].ToString() + "|" +
                                    rsRead[2].ToString() + "|" + rsRead[3].ToString() + "|" + rsRead[4].ToString(), Convert.ToDateTime(rsRead[6]));
                                }

                                //if (!Transit_Consumer_Globals.dictStoredStopTimeUpdateId.ContainsKey(rsRead[1].ToString() + "|" +
                                //    rsRead[2].ToString() + "|" + rsRead[3].ToString() + "|" + rsRead[4].ToString()))
                                //{
                                //    Transit_Consumer_Globals.dictStoredStopTimeUpdateId.Add(rsRead[1].ToString() + "|" +
                                //    rsRead[2].ToString() + "|" + rsRead[3].ToString() + "|" + rsRead[4].ToString(), Convert.ToInt32(rsRead[5]));
                                //}

                            }

                        }
                    }

                }

                Transit_Consumer_Globals.LogFile.Log("Successfully read agency data in GetCurrentTripUpdateData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in GetCurrentTripUpdateData()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
