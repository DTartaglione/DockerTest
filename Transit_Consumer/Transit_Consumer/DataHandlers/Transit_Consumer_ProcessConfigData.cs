using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Transit_Consumer
{
    class Transit_Consumer_ProcessConfigData
    {
        public static int InsertRouteConfig(Config _Config)
        {
            Route _Route = new Route();
            int iResult = -1;
            bool bResult = false;
            string sSQL = "transitds.sp_updatetblroutes";
            
            try
            {
                _Route = _Config.RouteConfig;

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;
                        
                        cmdSQL.Parameters.AddWithValue("_route_id", NpgsqlDbType.Varchar, _Route.Route_Id);
                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Varchar, _Route.Agency_Id);
                        cmdSQL.Parameters.AddWithValue("_route_short_name", NpgsqlDbType.Varchar, _Route.Short_Name);
                        cmdSQL.Parameters.AddWithValue("_route_long_name", NpgsqlDbType.Varchar, _Route.Name);
                        cmdSQL.Parameters.AddWithValue("_route_desc", NpgsqlDbType.Varchar, _Route.Description);
                        cmdSQL.Parameters.AddWithValue("_route_type", NpgsqlDbType.Integer, _Route.Type);
                        cmdSQL.Parameters.AddWithValue("_route_url", NpgsqlDbType.Varchar, _Route.URL);
                        cmdSQL.Parameters.AddWithValue("_route_color", NpgsqlDbType.Varchar, _Route.Color);
                        cmdSQL.Parameters.AddWithValue("_route_text_color", NpgsqlDbType.Varchar, _Route.Text_Color);
                        cmdSQL.Parameters.AddWithValue("_route_sort_order", NpgsqlDbType.Integer, _Route.Sort_Order);

                        iResult = (Int32)cmdSQL.ExecuteScalar();

                    }
                }
                
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in InsertRouteConfig().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                iResult = -1;
            }

            return iResult;
        }

        public static bool InsertRouteShapeConfig(Config _Config)
        {
            Route _Route = new Route();
            List<Coordinate> lstRouteCoords = new List<Coordinate>();
            bool bResult = false;
            string sSQL = "transitds.sp_updatetbltransitshapes";
            string sLineString = "";

            try
            {
                _Route = _Config.RouteConfig;
                lstRouteCoords = _Route.Route_Path;

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        foreach (Coordinate _Coords in lstRouteCoords)
                        {
                            if (sLineString != "")
                            {
                                sLineString += ",";
                            }

                            sLineString += _Coords.Longitude + " " + _Coords.Latitude;
                        }

                        cmdSQL.Parameters.AddWithValue("_route_id", NpgsqlDbType.Varchar, _Route.Route_Id);
                        cmdSQL.Parameters.AddWithValue("_shape_id", NpgsqlDbType.Integer, _Route.Local_Route_Id);
                        cmdSQL.Parameters.AddWithValue("_shape_linestring", NpgsqlDbType.Varchar, sLineString);
                        cmdSQL.Parameters.AddWithValue("_shape_length", NpgsqlDbType.Integer, 0);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in InsertRouteShapeConfig().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                bResult = false;
            }

            return bResult;
        }

        public static bool InsertStopConfig(Config _Config)
        {
            List<Stop> lstStopList = new List<Stop>();
            Coordinate _Coords = new Coordinate();
            bool bResult = false;
            string sSQL = "transitds.sp_updatetblstops";
            int iResult = -1;

            try
            {
                lstStopList = _Config.StopList;

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    foreach (Stop _Stop in lstStopList) {

                        using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                        {
                            cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;
                            cmdSQL.CommandType = CommandType.StoredProcedure;

                            _Coords = new Coordinate();
                            _Coords = _Stop.Coords;

                            cmdSQL.Parameters.AddWithValue("_stop_id", NpgsqlDbType.Integer, _Stop.Stop_Id);
                            cmdSQL.Parameters.AddWithValue("_stop_code", NpgsqlDbType.Varchar, _Stop.Code);
                            cmdSQL.Parameters.AddWithValue("_stop_name", NpgsqlDbType.Varchar, _Stop.Name);
                            cmdSQL.Parameters.AddWithValue("_stop_desc", NpgsqlDbType.Varchar, _Stop.Description);
                            cmdSQL.Parameters.AddWithValue("_stop_lat", NpgsqlDbType.Double, _Coords.Latitude);
                            cmdSQL.Parameters.AddWithValue("_stop_lon", NpgsqlDbType.Double, _Coords.Longitude);
                            cmdSQL.Parameters.AddWithValue("_zone_id", NpgsqlDbType.Integer, _Stop.Zone_Id);
                            cmdSQL.Parameters.AddWithValue("_stop_url", NpgsqlDbType.Varchar, _Stop.URL);
                            cmdSQL.Parameters.AddWithValue("_location_type", NpgsqlDbType.Integer, _Stop.Location_Type);
                            cmdSQL.Parameters.AddWithValue("_parent_station", NpgsqlDbType.Integer, _Stop.Parent_Station_Id);
                            cmdSQL.Parameters.AddWithValue("_stop_timezone", NpgsqlDbType.Varchar, _Stop.Stop_Timezone);
                            cmdSQL.Parameters.AddWithValue("_wheelchair_boarding", NpgsqlDbType.Integer, _Stop.Wheelchair_Boarding);
                            cmdSQL.Parameters.AddWithValue("_level_id", NpgsqlDbType.Integer, _Stop.Level_Id);
                            cmdSQL.Parameters.AddWithValue("_platform_code", NpgsqlDbType.Varchar, _Stop.Platform_Code);
 
                            iResult = (Int32)cmdSQL.ExecuteScalar();

                        }
                        
                        if (!Transit_Consumer_Globals.dictTransitStopMap.ContainsKey(iResult)) {
                                Transit_Consumer_Globals.dictTransitStopMap.Add(iResult, _Stop.Stop_Id);
                        }

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in InsertStopConfig().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                bResult = false;
            }
            return bResult;
        }
    }
}
