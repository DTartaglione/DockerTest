using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;

namespace Waze_Route_Consumer
{
    class Waze_Route_Consumer_HandleResponse
    {

        public static void HandleResponse(List<dynamic> lstWazeRouteData)
        {
            
            Route _RouteData = new Route();
            SubRoute _SubRouteData = new SubRoute();

            int iRouteID = 0;
            bool bDoUpdate = false;
            int iNumUpdates = 0;
            int i = 0;
            List<Route> lstNoSQLData = new List<Route>();
            List<SubRoute> lstSubRoutes = new List<SubRoute>();             //list used to add to Route.SubRoute list
            List<SubRoute> lstNoSQLDataSubRoutes = new List<SubRoute>();    //list for all subroutes in latest pull to be processed to MongoDB
            JObject jObj = null;
            JToken jSubRouteToken = null;
            DateTime dtLastDBUpdate = new DateTime();
            DateTime dtLastSourceUpdate = new DateTime();
            string sSubRouteName = "";
            int iWazeSubRouteID = 0;

                try
                {

                    foreach (dynamic dLstItem in lstWazeRouteData)
                    {

                    jObj = JObject.Parse(dLstItem.ToString());   

                        try
                        {
                            _RouteData = new Route();
                            lstSubRoutes = new List<SubRoute>();
                            iRouteID = Convert.ToInt32(jObj["RouteID"].ToString());
                            dtLastSourceUpdate = Convert.ToDateTime(jObj["LastUpdate"].ToString());
                           
                            _RouteData.AgencyID = Convert.ToInt32(jObj["AgencyID"].ToString());
                            _RouteData.RouteID = iRouteID;
                            _RouteData.RouteName = jObj["RouteName"].ToString();
                            _RouteData.FromName = jObj["FromName"].ToString();
                            _RouteData.ToName = jObj["ToName"].ToString();
                            _RouteData.Length = Convert.ToInt32(jObj["Length"].ToString());
                            _RouteData.CurrentTime = Convert.ToInt32(jObj["CurrentTime"].ToString());
                            _RouteData.HistoricTime = Convert.ToInt32(jObj["HistoricTime"].ToString());
                            _RouteData.JamLevel = Convert.ToInt32(jObj["JamLevel"].ToString());
                            _RouteData.LastUpdate = dtLastSourceUpdate;

                            if (!Waze_Route_Consumer_Globals.dictRouteLookupInfo.ContainsKey(iRouteID))
                            {
                                Waze_Route_Consumer_Globals.LogFile.Log("New route detected (" + iRouteID.ToString() + "). Adding to static table and dictionaries.");
                                Waze_Consumer_ProcessData.InsertRouteData(_RouteData);
                            }

                            if (jObj["Line"].ToString().Length > 0)
                            {
                                Waze_Route_Consumer_Globals.LogFile.Log("Storing linestring data for route:" + iRouteID.ToString() + ".");
                                _RouteData.Line = jObj["Line"].ToString();
                                Waze_Consumer_ProcessData.UpdateLinestring("Route", _RouteData.Line, iRouteID.ToString());
                            }

                        jSubRouteToken = jObj.SelectToken("SubRoutes");

                            if (jSubRouteToken != null)
                            {
                                foreach (JToken tkSubRoute in jSubRouteToken)
                                {
                                    sSubRouteName = (string)tkSubRoute.SelectToken("FromName") + " - " + (string)tkSubRoute.SelectToken("ToName");

                                    _SubRouteData = new SubRoute();
                                    _SubRouteData.RouteID = iRouteID;
                                    _SubRouteData.FromName = (string)tkSubRoute.SelectToken("FromName");
                                    _SubRouteData.ToName = (string)tkSubRoute.SelectToken("ToName");
                                    _SubRouteData.Length = (int)tkSubRoute.SelectToken("Length");
                                    _SubRouteData.CurrentTime = (int)tkSubRoute.SelectToken("CurrentTime");
                                    _SubRouteData.HistoricTime = (int)tkSubRoute.SelectToken("HistoricTime");
                                    _SubRouteData.JamLevel = (int)tkSubRoute.SelectToken("JamLevel");
                                    _SubRouteData.LastUpdate = dtLastSourceUpdate;

                                    lstSubRoutes.Add(_SubRouteData);

                                    if (dtLastSourceUpdate > dtLastDBUpdate)
                                    {
                                        lstNoSQLDataSubRoutes.Add(_SubRouteData);
                                    }


                                if (!Waze_Route_Consumer_Globals.dictSubRouteLookupInfo.ContainsKey(iRouteID.ToString() + "|" + sSubRouteName))
                                    {
                                    Waze_Route_Consumer_Globals.LogFile.Log("New subroute detected (" + sSubRouteName + "). Adding to static table and dictionaries.");
                                    iWazeSubRouteID = Waze_Consumer_ProcessData.InsertSubRouteData(_SubRouteData);

                                    //if (Waze_Route_Consumer_Globals.bStoreLinestring)
                                    //{
                                    //    Waze_Route_Consumer_Globals.LogFile.Log("Storing linestring data for subroute:" + sSubRouteName + ".");
                                    //    _SubRouteData.Line = (string)tkSubRoute.SelectToken("Line");
                                    //    Waze_Consumer_ProcessData.UpdateLinestring("SubRoute", _SubRouteData.Line, iWazeSubRouteID.ToString());
                                    //}
                                }

                                }

                                _RouteData.SubRoutes = lstSubRoutes;
                            }

                            //Only add routes with new timestamp
                            dtLastDBUpdate = Waze_Route_Consumer_Globals.dictRouteLastUpdateInfo[iRouteID];
                            if (dtLastSourceUpdate > dtLastDBUpdate)
                            {
                            lstNoSQLData.Add(_RouteData);
                            }
                                                  
                      
                        }
                        catch (Exception ex2)
                        {
                            Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_HandleResponse() updating individual route. Skip and continue.");
                            Waze_Route_Consumer_Globals.LogFile.LogError(ex2);
                        }

                    }

                if (lstNoSQLData.Count > 0)
                {
                    foreach (Route lstRouteProcess in lstNoSQLData)
                    {
                        try
                        {
                            Waze_Consumer_ProcessData.UpdateRouteData(lstRouteProcess);

                            foreach (SubRoute lstSubRouteProcess in lstRouteProcess.SubRoutes)
                            {
                                try
                                {
                                    Waze_Consumer_ProcessData.UpdateSubRouteData(lstSubRouteProcess);
                                }
                                catch (Exception ex4)
                                {
                                    Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error updating Waze SubRoute data for: " + lstSubRouteProcess.FromName.ToString() + " - " + lstSubRouteProcess.ToName.ToString() +
                                    " (staticds.tblwazesubroute.name) in Waze_Route_Consumer_HandleResponse()");
                                    Waze_Route_Consumer_Globals.LogFile.LogError(ex4);

                                }
                            }
                        }
                        catch (Exception ex3)
                        {
                            Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error updating Waze Route data for: " + lstRouteProcess.RouteID.ToString() + " (staticds.tblwazeroute.source_route_id)" +
                                " in Waze_Route_Consumer_HandleResponse()");
                            Waze_Route_Consumer_Globals.LogFile.LogError(ex3);
                        }
                    }

                    //MongoDB code
                    if (Waze_Route_Consumer_Globals.bSendToMongoDB)
                    { 
                        Waze_Route_Consumer_MongoCode.DoBatchUpdateRoutes(lstNoSQLData);
                        Waze_Route_Consumer_MongoCode.DoBatchUpdateSubRoutes(lstNoSQLDataSubRoutes);
                    }

                    }
                }
                catch (Exception ex)
                {
                    Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_HandleResponse()");
                    Waze_Route_Consumer_Globals.LogFile.LogError(ex);                    
                }
                finally
                {
                    Waze_Route_Consumer_Globals.LogFile.Log("Finished processing Route data from Waze. " + lstNoSQLData.Count.ToString() + " update(s) processed.");
                }
        }
    }
}
