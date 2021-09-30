using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using NpgsqlTypes;


namespace Waze_Route_Consumer
{
    class Waze_Consumer_ProcessData
    {

        public static bool InsertRouteData(Route _RouteData)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_insertwazeroutedata";
            int iWazeRouteID = 0;
            int iSourceRouteID = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        iSourceRouteID = Convert.ToInt32(_RouteData.RouteID.ToString());

                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Integer, _RouteData.AgencyID);
                        cmdSQL.Parameters.AddWithValue("_source_route_id", NpgsqlDbType.Varchar, _RouteData.RouteID.ToString());
                        cmdSQL.Parameters.AddWithValue("_name", NpgsqlDbType.Varchar, _RouteData.RouteName);
                        cmdSQL.Parameters.AddWithValue("_length_in_meters", NpgsqlDbType.Integer, _RouteData.Length);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _RouteData.LastUpdate);
                        cmdSQL.Parameters.AddWithValue("_travel_time_seconds", NpgsqlDbType.Integer, _RouteData.CurrentTime);
                        cmdSQL.Parameters.AddWithValue("_historic_travel_time_seconds", NpgsqlDbType.Integer, _RouteData.HistoricTime);
                        cmdSQL.Parameters.AddWithValue("_deviation_seconds", NpgsqlDbType.Integer, (_RouteData.CurrentTime - _RouteData.HistoricTime));
                        cmdSQL.Parameters.AddWithValue("_jam_level", NpgsqlDbType.Integer, _RouteData.JamLevel);

                        iWazeRouteID = (Int32)cmdSQL.ExecuteScalar();

                        Waze_Route_Consumer_Globals.dictRouteLookupInfo.Add(iSourceRouteID, iWazeRouteID);
                        Waze_Route_Consumer_Globals.dictRouteLastUpdateInfo.Add(iSourceRouteID, _RouteData.LastUpdate);

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Consumer_ProcessData.InsertRouteData()");
                Waze_Route_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        public static bool UpdateRouteData(Route _RouteData)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_updatewazeroutedata";
            int iWazeRouteID = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        iWazeRouteID = Waze_Route_Consumer_Globals.dictRouteLookupInfo[Convert.ToInt32(_RouteData.RouteID.ToString())];

                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_waze_route_id", NpgsqlDbType.Integer, iWazeRouteID);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _RouteData.LastUpdate);
                        cmdSQL.Parameters.AddWithValue("_travel_time_seconds", NpgsqlDbType.Integer, _RouteData.CurrentTime);
                        cmdSQL.Parameters.AddWithValue("_historic_travel_time_seconds", NpgsqlDbType.Integer, _RouteData.HistoricTime);
                        cmdSQL.Parameters.AddWithValue("_deviation_seconds", NpgsqlDbType.Integer, (_RouteData.CurrentTime - _RouteData.HistoricTime));
                        cmdSQL.Parameters.AddWithValue("_jam_level", NpgsqlDbType.Integer, _RouteData.JamLevel);

                        cmdSQL.ExecuteNonQuery();

                        //update dictionary with last update to latest time
                        Waze_Route_Consumer_Globals.dictRouteLastUpdateInfo[Convert.ToInt32(_RouteData.RouteID.ToString())] = _RouteData.LastUpdate;

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Consumer_ProcessData.UpdateRouteData()");
                Waze_Route_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        public static int InsertSubRouteData(SubRoute _SubRouteData)
        {
            //bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_insertwazesubroutedata";
            int iWazeRouteID = 0;
            int iWazeSubRouteID = 0;
            string sName = "";


            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        sName = _SubRouteData.FromName + " - " + _SubRouteData.ToName;
                        iWazeRouteID = Waze_Route_Consumer_Globals.dictRouteLookupInfo[Convert.ToInt32(_SubRouteData.RouteID.ToString())];

                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_waze_route_id", NpgsqlDbType.Integer, iWazeRouteID);
                        cmdSQL.Parameters.AddWithValue("_source_route_id", NpgsqlDbType.Varchar, _SubRouteData.RouteID.ToString());
                        cmdSQL.Parameters.AddWithValue("_name", NpgsqlDbType.Varchar, sName);
                        cmdSQL.Parameters.AddWithValue("_length_in_meters", NpgsqlDbType.Integer, _SubRouteData.Length);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _SubRouteData.LastUpdate);
                        cmdSQL.Parameters.AddWithValue("_travel_time_seconds", NpgsqlDbType.Integer, _SubRouteData.CurrentTime);
                        cmdSQL.Parameters.AddWithValue("_historic_travel_time_seconds", NpgsqlDbType.Integer, _SubRouteData.HistoricTime);
                        cmdSQL.Parameters.AddWithValue("_deviation_seconds", NpgsqlDbType.Integer, (_SubRouteData.CurrentTime - _SubRouteData.HistoricTime));
                        cmdSQL.Parameters.AddWithValue("_jam_level", NpgsqlDbType.Integer, _SubRouteData.JamLevel);

                        iWazeSubRouteID = (Int32)cmdSQL.ExecuteScalar();

                        Waze_Route_Consumer_Globals.dictSubRouteLookupInfo.Add(_SubRouteData.RouteID.ToString()+"|"+sName, iWazeSubRouteID);

                        //bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Consumer_ProcessData.InsertSubRouteData()");
                Waze_Route_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }

            return iWazeSubRouteID;
        }

        public static bool UpdateSubRouteData(SubRoute _SubRouteData)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_updatewazesubroutedata";
            int iWazeSubRouteID = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        iWazeSubRouteID = Waze_Route_Consumer_Globals.dictSubRouteLookupInfo[_SubRouteData.RouteID.ToString()+"|"+_SubRouteData.FromName + " - " + _SubRouteData.ToName];

                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_waze_subroute_id", NpgsqlDbType.Integer, iWazeSubRouteID);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _SubRouteData.LastUpdate);
                        cmdSQL.Parameters.AddWithValue("_travel_time_seconds", NpgsqlDbType.Integer, _SubRouteData.CurrentTime);
                        cmdSQL.Parameters.AddWithValue("_historic_travel_time_seconds", NpgsqlDbType.Integer, _SubRouteData.HistoricTime);
                        cmdSQL.Parameters.AddWithValue("_deviation_seconds", NpgsqlDbType.Integer, (_SubRouteData.CurrentTime - _SubRouteData.HistoricTime));
                        cmdSQL.Parameters.AddWithValue("_jam_level", NpgsqlDbType.Integer, _SubRouteData.JamLevel);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Consumer_ProcessData.UpdateSubRouteData()");
                Waze_Route_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        public static bool UpdateLinestring(string sRouteType, string sLinestring, string sSourceRouteID)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_updatewazelinestring";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        
                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_route_type", NpgsqlDbType.Varchar, sRouteType);
                        cmdSQL.Parameters.AddWithValue("_linestring", NpgsqlDbType.Varchar, sLinestring);
                        cmdSQL.Parameters.AddWithValue("_source_route_id", NpgsqlDbType.Varchar, sSourceRouteID);                       

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Consumer_ProcessData.UpdateLinestring()");
                Waze_Route_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }


            return bResult;
        }
    }
}
