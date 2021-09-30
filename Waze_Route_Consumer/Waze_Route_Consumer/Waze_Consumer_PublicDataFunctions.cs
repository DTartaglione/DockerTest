using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Waze_Route_Consumer
{
    class Waze_Route_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            //if (GetAgencyInfo())
            //{
            //    return true;  
            //}
            return true;
        }

        public static bool GetRouteLastUpdateData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select wr.source_route_id,wrd.last_update from dynamicds.tblwazeroutedata wrd inner join staticds.tblwazeroute wr ON wr.id = wrd.waze_route_id WHERE wr.status = 1;";

            try
            {
                Waze_Route_Consumer_Globals.dictRouteLastUpdateInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                Waze_Route_Consumer_Globals.dictRouteLastUpdateInfo.Add(Convert.ToInt32(rsRead[0].ToString()), Convert.ToDateTime(rsRead[1].ToString()));
                            }

                        }
                    }

                }

                Waze_Route_Consumer_Globals.LogFile.Log("Successfully stored last successful update for each route in GetRouteLastUpdateData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_PublicDataFunctions.GetRouteLastUpdateData()");
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }
            return bSuccess;
        }

        public static bool GetRouteLookupData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select id,source_route_id from staticds.tblwazeroute;";

            try
            {
                Waze_Route_Consumer_Globals.dictRouteLookupInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                Waze_Route_Consumer_Globals.dictRouteLookupInfo.Add(Convert.ToInt32(rsRead[1].ToString()), Convert.ToInt32(rsRead[0].ToString()));
                            }
                        }
                    }
                }

                Waze_Route_Consumer_Globals.LogFile.Log("Successfully stored route lookup info in GetRouteLookupData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_PublicDataFunctions.GetRouteLookupData()");
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }
            return bSuccess;
        }

        public static bool GetSubrouteLookupData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select waze_subroute_id,source_route_id,name from xref_tables.tblwazesubroutelookup;";

            try
            {
                Waze_Route_Consumer_Globals.dictSubRouteLookupInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                Waze_Route_Consumer_Globals.dictSubRouteLookupInfo.Add(rsRead[1].ToString() + "|" + rsRead[2].ToString(),Convert.ToInt32(rsRead[0].ToString()));
                            }

                        }
                    }

                }

                Waze_Route_Consumer_Globals.LogFile.Log("Successfully stored subroute lookup info in GetSubrouteLookupData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_PublicDataFunctions.GetSubrouteLookupData()");
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }
            return bSuccess;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                Waze_Route_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Waze_Route_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Waze_Route_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read()) {      
                                
                                if (!Waze_Route_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    Waze_Route_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((Waze_Route_Consumer_Globals.iAgencyID = Waze_Route_Consumer_Globals.SetAgencyID(Waze_Route_Consumer_Globals.sAgencyName)) == 0)
                            {
                                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + Waze_Route_Consumer_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                                return false;
                            }
                        }
                    }

                }

                Waze_Route_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_PublicDataFunctions.GetAgencyInfo()");
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
