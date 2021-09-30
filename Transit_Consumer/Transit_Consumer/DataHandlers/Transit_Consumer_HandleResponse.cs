using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Transit_Consumer
{
    class Transit_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstTransitData)
        {
            JObject jTransitObj = new JObject();
            StringBuilder sbTransitUpdates = new StringBuilder();
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";
            string sDataType = "";

            List<VehicleLocations> lstVehicleLocations = new List<VehicleLocations>();
            List<TripUpdates> lstTripUpdates = new List<TripUpdates>();
            Config _Config = new Config();
           // StoredEventData _StoredEvent;
            //Transit_Consumer_ProcessData clProcData = new Transit_Consumer_ProcessData();

            try
            {
                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy");

                lstVehicleLocations.Clear();

                Transit_Consumer_Globals.LogFile.Log("In HandleResponse(). Begin processing " + lstTransitData.Count.ToString() + " Transit data objects.");
               
                foreach (string sLstItem in lstTransitData)
                {
                    try {
                        jTransitObj = JObject.Parse(sLstItem);
                        sDataType = (string)jTransitObj.SelectToken("DataType");

                        if (sDataType.ToLower() == "config")
                        {
                            if (!ProcessConfig(jTransitObj))
                            {
                                Transit_Consumer_Globals.LogFile.LogErrorText("Error processing Config record. See logs for details.");
                            }
                        }
                        else if (sDataType.ToLower() == "predictions")
                        {
                            if (!ProcessPredictions(jTransitObj))
                            {
                                Transit_Consumer_Globals.LogFile.LogErrorText("Error processing Predictions record. See logs for details.");
                            }
                        }
                        else if (sDataType.ToLower() == "vehiclelocations")
                        {
                            lstVehicleLocations.AddRange(ProcessVehicleLocations(jTransitObj));
                            //if (!ProcessVehicleLocations(jTransitObj))
                            //{
                            //    Transit_Consumer_Globals.LogFile.LogErrorText("Error processing Vehicle Locations record. See logs for details.");
                            //}                        
                        }
                        else if (sDataType.ToLower() == "tripupdate")
                        {
                            lstTripUpdates.AddRange(ProcessTripUpdates(jTransitObj));

                            //if (!ProcessTripUpdates(jTransitObj))
                            //{
                            //    Transit_Consumer_Globals.LogFile.LogErrorText("Error processing Trip Update record. See logs for details.");
                            //}
                        }
                        else if (sDataType.ToLower() == "messages")
                        {
                            if (!ProcessMessages(jTransitObj))
                            {
                                Transit_Consumer_Globals.LogFile.LogErrorText("Error processing Messages record. See logs for details.");
                            }
                        }                       
                        else
                        {
                            Transit_Consumer_Globals.LogFile.Log("Transit data type " + sDataType + " not recognized. Skipping record.");
                            continue;
                        }
                    }
                    catch (Exception ex1)
                    {
                        Transit_Consumer_Globals.LogFile.LogErrorText("Error processing individual transit item in HandleEventResponse(). Continue after error.");
                        Transit_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        Transit_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                if (lstVehicleLocations.Count > 0)
                {
                    //now insert into database valid records
                    Transit_Consumer_Globals.LogFile.Log("Begin processing Vehicle Locations.");
                    if (Transit_Consumer_ProcessVehicleLocations.ProcessVehicleLocations(lstVehicleLocations))
                    {
                        Transit_Consumer_Globals.LogFile.Log("Successfully processed Vehicle Locations.");
                    }
                }

                if (lstTripUpdates.Count > 0)
                {
                    //now insert into database valid records
                    Transit_Consumer_Globals.LogFile.Log("Begin processing Trip Updates.");
                    if (Transit_Consumer_ProcessTripUpdates.ProcessTripUpdates(lstTripUpdates))
                    {
                        Transit_Consumer_Globals.LogFile.Log("Successfully processed Trip Updates.");
                    }
                }


                Transit_Consumer_Globals.LogFile.Log("Transit data processing finished. Successfully processed " + lstTransitData.Count.ToString() + " transit objects.");

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in HandleEventResponse()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        private static bool ProcessConfig(JObject jTransitObj)
        {
            bool bResult = false;
            Config _Config = new Config();
            Route _Route = new Route();
            Stop _Stop = new Stop();
            List<Stop> lstStopList = new List<Stop>();
            Coordinate _Coords = new Coordinate();
            JObject jRteConfig = null;
            JArray jStopList = null;
            JArray jRoutePath = null;
            List<Coordinate> lstCoords = new List<Coordinate>();


            try
            {
                jRteConfig = (JObject)jTransitObj.SelectToken("RouteConfig");
                jStopList = (JArray)jTransitObj.SelectToken("StopList");
                jRoutePath = (JArray)jRteConfig.SelectToken("Route_Path");

                _Config.DataType = (string)jTransitObj.SelectToken("DataType");
                _Route.Local_Route_Id = Convert.ToInt32(jRteConfig.SelectToken("Local_Route_Id"));
                _Route.Agency_Id = (string)jRteConfig.SelectToken("Agency_Id");
                _Route.Route_Id = (string)jRteConfig.SelectToken("Route_Id");
                _Route.Name = (string)jRteConfig.SelectToken("Name");
                _Route.Short_Name = (string)jRteConfig.SelectToken("Short_Name");
                _Route.Description = (string)jRteConfig.SelectToken("Description");
                _Route.Direction = (string)jRteConfig.SelectToken("Direction");
                _Route.Type = Convert.ToInt16(jRteConfig.SelectToken("Type"));
                _Route.URL = (string)jRteConfig.SelectToken("URL");
                _Route.Color = (string)jRteConfig.SelectToken("Color");
                _Route.Text_Color = (string)jRteConfig.SelectToken("Text_Color");
                _Route.Sort_Order = Convert.ToInt16(jRteConfig.SelectToken("Sort_Order"));

                if (_Route.Color != "" && _Route.Color.IndexOf("#") != 0)
                {
                    _Route.Color = "#" + _Route.Color;
                }

                if (_Route.Text_Color != "" && _Route.Text_Color.IndexOf("#") != 0)
                {
                    _Route.Text_Color = "#" + _Route.Text_Color;
                }


                foreach (JToken jCoordsToken in jRoutePath)
                {
                    _Coords = new Coordinate();
                    _Coords.Latitude = Convert.ToDouble(jCoordsToken.SelectToken("Latitude"));
                    _Coords.Longitude = Convert.ToDouble(jCoordsToken.SelectToken("Longitude"));
                    lstCoords.Add(_Coords);
                }

                _Route.Route_Path = lstCoords;
                _Config.RouteConfig = _Route;

                foreach (JToken jStopToken in jStopList)
                {
                    _Stop = new Stop();
                    _Coords = new Coordinate();
                    
                    _Stop.Local_Stop_Id = Convert.ToInt32(jStopToken.SelectToken("Local_Stop_Id"));
                    _Stop.Stop_Id = Convert.ToInt32(jStopToken.SelectToken("Stop_Id"));
                    _Stop.Name = (string)jStopToken.SelectToken("Name");
                    _Stop.Code = (string)jStopToken.SelectToken("Code");
                    _Stop.Description = (string)jStopToken.SelectToken("Description");
                    _Coords.Latitude = Convert.ToDouble(jStopToken.SelectToken("Coords").SelectToken("Latitude"));
                    _Coords.Longitude = Convert.ToDouble(jStopToken.SelectToken("Coords").SelectToken("Longitude"));
                    _Stop.Coords = _Coords;
                    _Stop.Zone_Id = Convert.ToInt32(jStopToken.SelectToken("Zone_Id"));
                    _Stop.URL = (string)jStopToken.SelectToken("URL");
                    _Stop.Location_Type = Convert.ToInt32(jStopToken.SelectToken("Location_Type"));
                    _Stop.Parent_Station_Id = Convert.ToInt32(jStopToken.SelectToken("Parent_Station_Id"));
                    _Stop.Stop_Timezone = (string)jStopToken.SelectToken("Stop_Timezone");
                    _Stop.Wheelchair_Boarding = Convert.ToInt32(jStopToken.SelectToken("Wheelchair_Boarding"));
                    _Stop.Level_Id = Convert.ToInt32(jStopToken.SelectToken("Level_Id"));
                    _Stop.Platform_Code = (string)jStopToken.SelectToken("Platform_Code");
                    _Stop.Stop_Sequence = Convert.ToInt32(jStopToken.SelectToken("Stop_Sequence"));

                    lstStopList.Add(_Stop);
                }

                _Config.StopList = lstStopList;

                if ((_Route.Local_Route_Id = Transit_Consumer_ProcessConfigData.InsertRouteConfig(_Config)) != -1)
                {
                    _Config.RouteConfig.Local_Route_Id = _Route.Local_Route_Id;

                    if (Transit_Consumer_ProcessConfigData.InsertRouteShapeConfig(_Config))
                    {
                        bResult = Transit_Consumer_ProcessConfigData.InsertStopConfig(_Config);

                    }
                }

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in ProcessConfig()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                bResult = false;
            }

            return bResult;
        }

        private static bool ProcessPredictions(JObject jTransitObj)
        {
            bool bResult = false;
            Predictions _Prediction = new Predictions();

            try
            {
                _Prediction.DataType = (string)jTransitObj.SelectToken("DataType");
                _Prediction.Agency = (string)jTransitObj.SelectToken("Agency");
                _Prediction.Route = (string)jTransitObj.SelectToken("Route");
                _Prediction.LastUpdate = Convert.ToDateTime(jTransitObj.SelectToken("LastUpdate"));
                _Prediction.StopTag = (string)jTransitObj.SelectToken("StopTag");
                _Prediction.IsDeparture = Convert.ToBoolean(jTransitObj.SelectToken("IsDeparture"));
                _Prediction.Minutes = Convert.ToInt32(jTransitObj.SelectToken("Minutes"));
                _Prediction.Seconds = Convert.ToInt32(jTransitObj.SelectToken("Seconds"));
                _Prediction.TripTag = (string)jTransitObj.SelectToken("TripTag");
                _Prediction.Vehicle = (string)jTransitObj.SelectToken("Vehicle");
                _Prediction.Block = (string)jTransitObj.SelectToken("Block");
                _Prediction.DirTag = (string)jTransitObj.SelectToken("DirTag");
                _Prediction.EpochTime = (long)jTransitObj.SelectToken("EpochTime");

                bResult = Transit_Consumer_ProcessPredictions.InsertPrediction(_Prediction);
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in ProcessPredictions()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        private static List<VehicleLocations> ProcessVehicleLocations(JObject jTransitObj)
        {
            VehicleLocations _VehicleLocation = new VehicleLocations();
            Coordinate _Coords = new Coordinate();
            List<VehicleLocations> lstVehicleLocations = new List<VehicleLocations>();

            try
            {
                _VehicleLocation.DataType = (string)jTransitObj.SelectToken("DataType");

                _VehicleLocation.Id = (string)jTransitObj.SelectToken("Id");
                _VehicleLocation.TripId= (string)jTransitObj.SelectToken("TripId");
                _VehicleLocation.Agency = (string)jTransitObj.SelectToken("Agency");
                _VehicleLocation.Route = (string)jTransitObj.SelectToken("Route");
                _VehicleLocation.StopId = Convert.ToInt32(jTransitObj.SelectToken("StopId"));
                _VehicleLocation.LastUpdate = Convert.ToDateTime(jTransitObj.SelectToken("LastUpdate"));
                _Coords.Latitude = Convert.ToDouble(jTransitObj.SelectToken("Coords").SelectToken("Latitude"));
                _Coords.Longitude = Convert.ToDouble(jTransitObj.SelectToken("Coords").SelectToken("Longitude"));
                _VehicleLocation.Coords = _Coords;                
                _VehicleLocation.Predictable = Convert.ToBoolean(jTransitObj.SelectToken("Predictable"));
                _VehicleLocation.SpeedKmHr = Convert.ToInt32(jTransitObj.SelectToken("SpeedKmHr"));
                _VehicleLocation.DirTag = (string)jTransitObj.SelectToken("DirTag");
                _VehicleLocation.Heading = Convert.ToInt32(jTransitObj.SelectToken("Heading"));
                _VehicleLocation.SecsSinceReport = Convert.ToInt32(jTransitObj.SelectToken("SecsSinceReport"));
                _VehicleLocation.DirectionId = Convert.ToInt16(jTransitObj.SelectToken("DirectionId"));
                _VehicleLocation.StartDate = (string)jTransitObj.SelectToken("StartDate");

                lstVehicleLocations.Add(_VehicleLocation);
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in ProcessVehicleLocations()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }
            return lstVehicleLocations;
        }

        private static List<TripUpdates> ProcessTripUpdates(JObject jTransitObj)
        {
            TripUpdates _TripUpdate = new TripUpdates();
            StopTimeUpdates _StopTimeUpdate = new StopTimeUpdates();
            List<StopTimeUpdates> lstStopTimeUpdates = new List<StopTimeUpdates>();
            List<TripUpdates> lstTripUpdates = new List<TripUpdates>();

            try
            {
                _TripUpdate.DataType = (string)jTransitObj.SelectToken("DataType");
                _TripUpdate.Agency = (string)jTransitObj.SelectToken("Agency");
                _TripUpdate.TripId = (string)jTransitObj.SelectToken("TripId");
                _TripUpdate.Route = (string)jTransitObj.SelectToken("RouteId");
                _TripUpdate.DirectionId = (int)jTransitObj.SelectToken("DirectionId");
                _TripUpdate.Vehicle = (string)jTransitObj.SelectToken("VehicleId");
                _TripUpdate.StartDate = (string)jTransitObj.SelectToken("StartDate");
                _TripUpdate.LastUpdate = Convert.ToDateTime(jTransitObj.SelectToken("LastUpdate")).ToLocalTime();
                _TripUpdate.Delay = (int)jTransitObj.SelectToken("Delay");

                foreach (JToken jStopTimeTok in jTransitObj.SelectToken("StopTimeUpdates"))
                {
                    _StopTimeUpdate = new StopTimeUpdates();
                    _StopTimeUpdate.Stop_Sequence = (int)jStopTimeTok.SelectToken("StopSequence");
                    _StopTimeUpdate.Stop_Id = (int)jStopTimeTok.SelectToken("StopId");
                    _StopTimeUpdate.Arrival = (long)jStopTimeTok.SelectToken("Arrival");
                    _StopTimeUpdate.ArrivalDelay = (int)jStopTimeTok.SelectToken("ArrivalDelay");
                    _StopTimeUpdate.ArrivalUncertainty = (int)jStopTimeTok.SelectToken("ArrivalUncertainty");
                    _StopTimeUpdate.Departure =(long)jStopTimeTok.SelectToken("Departure");
                    _StopTimeUpdate.DepartureDelay = (int)jStopTimeTok.SelectToken("DepartureDelay");
                    _StopTimeUpdate.DepartureUncertainty = (int)jStopTimeTok.SelectToken("DepartureUncertainty");
                    lstStopTimeUpdates.Add(_StopTimeUpdate);
                }

                _TripUpdate.StopTimeUpdates = lstStopTimeUpdates;

                lstTripUpdates.Add(_TripUpdate);
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in ProcessTripUpdates()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }

            return lstTripUpdates;
        }

        private static bool ProcessMessages(JObject jTransitObj)
        {
            bool bResult = false;
            Messages _Message = new Messages();

            try
            {
                
                _Message.Agency = (string)jTransitObj.SelectToken("Agency");
                _Message.Route = (string)jTransitObj.SelectToken("Route");
                _Message.LastUpdate = Convert.ToDateTime(jTransitObj.SelectToken("LastUpdate"));
                _Message.Id = (string)jTransitObj.SelectToken("Id");
                _Message.DataType = (string)jTransitObj.SelectToken("DataType");
                _Message.Text = (string)jTransitObj.SelectToken("Text");
                _Message.StartBoundary = (long)jTransitObj.SelectToken("StartBoundary");
                _Message.EndBoundary = (long)jTransitObj.SelectToken("EndBoundary");
                _Message.Priority = (string)jTransitObj.SelectToken("Priority");

                bResult = Transit_Consumer_ProcessMessages.InsertMessage(_Message);
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in ProcessMessages()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }
    }
}
