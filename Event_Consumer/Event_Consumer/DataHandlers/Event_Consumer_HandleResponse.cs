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

namespace Event_Consumer
{
    class Event_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstEventData)
        {
            JObject jEventObj = new JObject();
            List<string> lstDeletes = new List<string>();
            List<string> lstNewIDs = new List<string>();
            StringBuilder sbEventUpdates = new StringBuilder();
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";
            string sOutFileName = "";

            int iNumClosedEvents = 0;
            int iNumInsertedEvents = 0;
            int iNumUpdatedEvents = 0;

            EventData _Event;
           // StoredEventData _StoredEvent;
            Event_Consumer_ProcessEvent clProcEvt = new Event_Consumer_ProcessEvent();
            //DateTime dtFeedLastUpdate;
            bool bDoUpdate = true;

            try
            {
                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmss.fff");
                sOutFileName = Event_Consumer_Globals.sReceivedMessagesDir + "\\EventUpdates_" + sCurrDate;

                Event_Consumer_Globals.LogFile.Log("In HandleResponse(). Begin processing " + lstEventData.Count.ToString() + " event data objects.");
                lstNewIDs.Clear();
                lstDeletes.Clear();

                if (Event_Consumer_Globals.bSimulationEnabled)
                {
                    Event_Consumer_Globals.LogFile.Log("Simulation mode enabled. Events will NOT be closed.");
                }

                foreach (string sLstItem in lstEventData)
                {
                    try { 
                        bDoUpdate = true;
                        jEventObj = JObject.Parse(sLstItem);
                        _Event = new EventData();

                        _Event.sSourceEventID = (string)jEventObj.SelectToken("SourceEventID");
                        _Event.iAgencyID = (int)jEventObj.SelectToken("AgencyID");
                        _Event.iEventStatus = (int)jEventObj.SelectToken("EventStatus");

                        if (Event_Consumer_Globals.dictEventIDMap.ContainsKey(_Event.iAgencyID.ToString() + "|" + _Event.sSourceEventID.ToString())){
                            //Event exists in map. Check if do update needed. If no change, ignore.
                            
                            _Event.sLastUpdate = (string)jEventObj.SelectToken("LastUpdate");

                            if (Convert.ToDateTime(_Event.sLastUpdate) <= Event_Consumer_Globals.dictEventIDMap[_Event.iAgencyID.ToString() + "|" + _Event.sSourceEventID].dtLastUpdate)
                            {
                                //event is not newer than our record. no need to update
                                bDoUpdate = false;
                                continue;
                            }

                            _Event.sLocalEventID = Event_Consumer_Globals.dictEventIDMap[_Event.iAgencyID.ToString() + "|" + _Event.sSourceEventID].sLocalEventID;

                            if (_Event.iEventStatus == 6)
                            {
                                _Event.iEventStatus = Event_Consumer_Globals.dictEventIDMap[_Event.iAgencyID.ToString() + "|" + _Event.sSourceEventID].iEventStatus;
                                
                            }
                            //_Event.iEventStatus = 9;
                        }
                        else
                        {
                            _Event.sLocalEventID = Event_Consumer_EventTranslationFunctions.GenerateLocalEventID(_Event.iAgencyID);
                            //_Event.iEventStatus = 6;
                            _Event.sCreateTime = Event_Consumer_EventTranslationFunctions.GetTimestamp(DateTime.Now);
                        }

                        _Event.sLastUpdate = Event_Consumer_EventTranslationFunctions.GetTimestamp(DateTime.Now);

                        if (bDoUpdate)
                        {
                            //sleep for 2 milliseconds to avoid duplicating event IDs
                            System.Threading.Thread.Sleep(2);

                            _Event.iEventCategoryID = (int)jEventObj.SelectToken("EventCategoryID");
                            _Event.sEventCategory = (string)jEventObj.SelectToken("EventCategory");
                            _Event.sEventTypes = (string)jEventObj.SelectToken("EventTypes");
                            _Event.iFacilityID = (int)jEventObj.SelectToken("FacilityID");
                            _Event.sFacilityName = (string)jEventObj.SelectToken("FacilityName");
                            _Event.FacilityNameAlt =(string)jEventObj.SelectToken("FacilityNameAlt");
                            _Event.alDirection = Event_Consumer_EventTranslationFunctions.GetDirection((string)jEventObj.SelectToken("Direction"), _Event.iAgencyID); 
                            _Event.sArticle = (string)jEventObj.SelectToken("Article");
                            _Event.iFromNodeID = (int)jEventObj.SelectToken("FromNodeID");
                            _Event.sFromNodeName = (string)jEventObj.SelectToken("FromNodeName");
                            _Event.dFromLat = Convert.ToDouble(jEventObj.SelectToken("FromLat"));
                            _Event.dFromLon = Convert.ToDouble(jEventObj.SelectToken("FromLon"));
                            _Event.iToNodeID = (int)jEventObj.SelectToken("ToNodeID");
                            _Event.sToNodeName = (string)jEventObj.SelectToken("ToNodeName");
                            _Event.dToLat = Convert.ToDouble(jEventObj.SelectToken("ToLat"));
                            _Event.dToLon = Convert.ToDouble(jEventObj.SelectToken("ToLon"));
                            _Event.iEstimatedDuration = (int)jEventObj.SelectToken("EstimatedDuration");
                            _Event.sCity = (string)jEventObj.SelectToken("City");
                            _Event.sCounty = (string)jEventObj.SelectToken("County");
                            _Event.sState = (string)jEventObj.SelectToken("State");
                            _Event.sExtraInfo = (string)jEventObj.SelectToken("ExtraInfo");
                            _Event.sLaneType = (string)jEventObj.SelectToken("LaneType");
                            _Event.sLaneStatus = (string)jEventObj.SelectToken("LaneStatus");
                            _Event.sEventDescription = (string)jEventObj.SelectToken("EventDescription");
                            _Event.CloseTime = string.IsNullOrEmpty((string)jEventObj.SelectToken("CloseTime")) ? "" : Event_Consumer_EventTranslationFunctions.GetTimestamp(Convert.ToDateTime((string)jEventObj.SelectToken("CloseTime")));

                            _Event.sStartDateTime = Event_Consumer_EventTranslationFunctions.GetTimestamp(string.IsNullOrEmpty(jEventObj.SelectToken("StartDateTime").ToString()) ? DateTime.Now : Convert.ToDateTime((string)jEventObj.SelectToken("StartDateTime")));

                            if (!string.IsNullOrEmpty(jEventObj.SelectToken("EndDateTime").ToString())){
                                _Event.sEndDateTime = Event_Consumer_EventTranslationFunctions.GetTimestamp(Convert.ToDateTime((string)jEventObj.SelectToken("EndDateTime")));
                            }

                            if (_Event.iEventStatus == 6)
                            {
                                if (clProcEvt.InsertEvent(_Event))
                                {
                                    Event_Consumer_Globals.LogFile.Log("Event " + _Event.sLocalEventID + " successfully inserted.");
                                    iNumInsertedEvents++;
                                }
                                else
                                {
                                    Event_Consumer_Globals.LogFile.LogErrorText("Error inserting event with source ID " + _Event.sSourceEventID + ". See error logs and invalid XML logs for details.");
                                    Event_Consumer_Globals.LogFile.LogToFile(jEventObj.ToString(), "Events\\InvalidEventXML");
                                }
                            }
                            else if (_Event.iEventStatus == 9)
                            {
                                if (clProcEvt.UpdateEvent(_Event))
                                {
                                    Event_Consumer_Globals.LogFile.Log("Event " + _Event.sLocalEventID + " successfully updated.");
                                    iNumUpdatedEvents++;
                                }
                                else
                                {
                                    Event_Consumer_Globals.LogFile.LogErrorText("Error updating event with source ID " + _Event.sSourceEventID + ". See error logs and invalid XML logs for details.");
                                    Event_Consumer_Globals.LogFile.LogToFile(jEventObj.ToString(), "Events\\InvalidEventXML");
                                }
                            }
                            else if (_Event.iEventStatus == 11)
                            {
                                if (!Event_Consumer_Globals.bSimulationEnabled) { 
                                    if (clProcEvt.CloseEvent(_Event.sSourceEventID, _Event.sLocalEventID, _Event.iAgencyID, _Event.CloseTime))
                                    {
                                        Event_Consumer_Globals.LogFile.Log("Event local ID: " + _Event.sLocalEventID  + ", source ID: " + _Event.sSourceEventID + " successfully closed.");
                                        iNumClosedEvents++;
                                    }
                                    else
                                    {
                                        Event_Consumer_Globals.LogFile.LogErrorText("Error closing event with source ID " + _Event.sSourceEventID + ". See error logs and invalid XML logs for details.");
                                        Event_Consumer_Globals.LogFile.LogToFile(jEventObj.ToString(), "Events\\InvalidEventXML");
                                    }
                                }
                            }

                            sbEventUpdates.Append(JsonConvert.SerializeObject(_Event, Newtonsoft.Json.Formatting.None).Trim());
                        }
                    }
                    catch (Exception ex1)
                    {
                        Event_Consumer_Globals.LogFile.LogErrorText("Error processing individual event in HandleEventResponse(). Continue after error.");
                        Event_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        Event_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                Event_Consumer_Globals.LogFile.Log("Event processing finished.");
                Event_Consumer_Globals.LogFile.Log(iNumClosedEvents.ToString() + " event(s) closed.");
                Event_Consumer_Globals.LogFile.Log(iNumInsertedEvents.ToString() + " event(s) inserted.");
                Event_Consumer_Globals.LogFile.Log(iNumUpdatedEvents.ToString() + " event(s) updated.");

                if (sbEventUpdates.Length > 0)
                {
                    
                    using (StreamWriter writer = new StreamWriter(sOutFileName, false))
                    {
                        sbEventUpdates.Append(Environment.NewLine);
                        sbEventUpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss tt") + ": " + Environment.NewLine);
                        writer.Write(sbEventUpdates.ToString());
                        sbEventUpdates.Remove(0, sbEventUpdates.Length);

                    }
                    sbEventUpdates.Remove(0, sbEventUpdates.Length);

                    if (File.Exists(sOutFileName))
                    {
                        if (Event_Consumer_Globals.DoGZIP(sOutFileName))
                        {
                            Event_Consumer_Globals.LogFile.Log("Successfully zipped Event Output file");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in HandleEventResponse()");
                Event_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Event_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
