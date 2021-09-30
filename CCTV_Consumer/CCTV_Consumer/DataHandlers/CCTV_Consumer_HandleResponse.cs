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

namespace CCTV_Consumer
{
    class CCTV_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstCCTVData)
        {
            JObject jCCTVObj = new JObject();
            List<string> lstDeletes = new List<string>();
            List<string> lstNewIDs = new List<string>();
            List<CCTVData> lstCCTVUpdates = new List<CCTVData>();
            StringBuilder sbCCTVUpdates = new StringBuilder();
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";
            Double dLat = 0;
            Double dLon = 0;

            int iNumProcessedCCTVs = 0;

            CCTVData _CCTV;
            StoredCCTVData _StoredCCTV;
            CCTV_Consumer_ProcessCCTV clProcCCTV = new CCTV_Consumer_ProcessCCTV();
            //DateTime dtFeedLastUpdate;
            bool bDoUpdate = true;
            string sLastUpdate = "";
            StringBuilder sbSerializedJSON = new StringBuilder();
            StringBuilder sbStoredJSON = new StringBuilder();

            try
            {
                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmssfff");

                CCTV_Consumer_Globals.LogFile.Log("In HandleResponse(). Begin processing " + lstCCTVData.Count.ToString() + " CCTV data objects.");
                lstNewIDs.Clear();
                lstDeletes.Clear();
                lstCCTVUpdates.Clear();

                if (CCTV_Consumer_Globals.bSimulationEnabled)
                {
                    CCTV_Consumer_Globals.LogFile.Log("Simulation mode enabled. CCTVs will NOT be removed.");
                }

                foreach (string sLstItem in lstCCTVData)
                {
                    try { 
                        bDoUpdate = true;
                        jCCTVObj = JObject.Parse(sLstItem);
                        _CCTV = new CCTVData();
                        _StoredCCTV = new StoredCCTVData();
                        dLat = 0;
                        dLon = 0;

                        sbSerializedJSON = new StringBuilder();
                        sbStoredJSON = new StringBuilder();

                        _CCTV.SourceCCTVID = (string)jCCTVObj.SelectToken("SourceCCTVID");
                        _CCTV.AgencyID = (int)jCCTVObj.SelectToken("AgencyID");
                        sLastUpdate = (string)jCCTVObj.SelectToken("LastUpdate");

                        
                        _CCTV.CCTVName = (string)jCCTVObj.SelectToken("CCTVName");
                        _CCTV.Roadway = string.IsNullOrEmpty((string)jCCTVObj.SelectToken("Roadway")) ? DBNull.Value.ToString() : (string)jCCTVObj.SelectToken("Roadway");
                        _CCTV.Direction = string.IsNullOrEmpty((string)jCCTVObj.SelectToken("Direction")) ? DBNull.Value.ToString() : (string)jCCTVObj.SelectToken("Direction");
                        _CCTV.SnapshotURL = (string)jCCTVObj.SelectToken("SnapshotURL");
                        _CCTV.VideoURL = string.IsNullOrEmpty((string)jCCTVObj.SelectToken("VideoURL")) ? DBNull.Value.ToString() : (string)jCCTVObj.SelectToken("VideoURL");
                        _CCTV.Blocked = Convert.ToBoolean(jCCTVObj.SelectToken("Blocked"));
                        _CCTV.Status = Convert.ToInt16(jCCTVObj.SelectToken("Status"));

                        _CCTV.Coords = new Point();
                        if (Double.TryParse((string)jCCTVObj.SelectToken("Coords").SelectToken("Latitude"), out dLat))
                        {
                            _CCTV.Coords.Latitude = dLat;
                        }

                        if (Double.TryParse((string)jCCTVObj.SelectToken("Coords").SelectToken("Longitude"), out dLon))
                        {
                            _CCTV.Coords.Longitude = dLon;
                        }

                        sbSerializedJSON.Append(JsonConvert.SerializeObject(_CCTV, Newtonsoft.Json.Formatting.None).Trim());

                        if (CCTV_Consumer_Globals.dictStoredCCTV.ContainsKey(_CCTV.AgencyID.ToString() + "|" + _CCTV.SourceCCTVID.ToString())){
                            //CCTV exists in map. Check if do update needed. If no change, ignore.

                            sbStoredJSON.Append(CCTV_Consumer_Globals.dictStoredCCTV[_CCTV.AgencyID.ToString() + "|" + _CCTV.SourceCCTVID].StoredJSON);

                            if (sbSerializedJSON.Equals(sbStoredJSON))
                            {
                                //CCTV is not different than our record. no need to update
                                bDoUpdate = false;
                                continue;
                            }

                            //_CCTV.LocalCCTVID = CCTV_Consumer_Globals.dictStoredCCTV[_CCTV.AgencyID.ToString() + "|" + _CCTV.SourceCCTVID].LocalCCTVID;
                        }

                        if (bDoUpdate)
                        {
                            _StoredCCTV.SourceCCTVID = _CCTV.AgencyID.ToString() + "|" + _CCTV.SourceCCTVID;
                            _StoredCCTV.StoredJSON.Append(sbSerializedJSON);
                            _CCTV.LastUpdate = Convert.ToDateTime((string)jCCTVObj.SelectToken("LastUpdate")).ToUniversalTime();

                            if (clProcCCTV.UpdateCCTV(_CCTV, _StoredCCTV))
                            {
                                //CCTV_Consumer_Globals.LogFile.Log("CCTV with source ID " + _CCTV.SourceCCTVID + " successfully processed.");
                                iNumProcessedCCTVs++;
                                lstCCTVUpdates.Add(_CCTV);

                            }
                            else
                            {
                                CCTV_Consumer_Globals.LogFile.LogErrorText("Error processing CCTV with source ID " + _CCTV.SourceCCTVID + ". See error logs and invalid XML logs for details.");
                                CCTV_Consumer_Globals.LogFile.LogToFile(jCCTVObj.ToString(), "CCTVs\\InvalidCCTVXML");
                            }

                            sbCCTVUpdates.Append(JsonConvert.SerializeObject(_CCTV, Newtonsoft.Json.Formatting.None).Trim());
                            
                        }
                    }
                    catch (Exception ex1)
                    {
                        CCTV_Consumer_Globals.LogFile.LogErrorText("Error processing individual CCTV in HandleCCTVResponse(). Continue after error.");
                        CCTV_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        CCTV_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                if (lstCCTVUpdates.Count > 0)
                {
                    CCTV_Consumer_Globals.LogFile.Log(lstCCTVUpdates.Count.ToString() + " records to update in NoSQL storage. Begin processing.");

                    if (CCTV_Consumer_Globals.bSendToMongoDB)
                    {
                        CCTV_Consumer_MongoCode.DoBatchUpdate(lstCCTVUpdates);
                    }
                }
                else
                {
                    CCTV_Consumer_Globals.LogFile.Log("No records to process into NoSQL storage.");
                }

                //if set to true in app.config, log all messages received. for debugging.
                if (CCTV_Consumer_Globals.bLogAllIncomingMessages)
                {
                    if (!Directory.Exists(CCTV_Consumer_Globals.sReceivedMessagesDir)) {
                        Directory.CreateDirectory(CCTV_Consumer_Globals.sReceivedMessagesDir);
                    }

                    if (sbCCTVUpdates.Length > 0)
                    {
                        using (StreamWriter writer = new StreamWriter(CCTV_Consumer_Globals.sReceivedMessagesDir + "\\CCTVUpdates_" + sCurrDate + ".log", true))
                        {
                            sbCCTVUpdates.Append(Environment.NewLine);
                            sbCCTVUpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss tt") + ": " + Environment.NewLine);
                            writer.Write(sbCCTVUpdates.ToString());
                            sbCCTVUpdates.Remove(0, sbCCTVUpdates.Length);
                        }

                        sbCCTVUpdates.Remove(0, sbCCTVUpdates.Length);

                        if (CCTV_Consumer_Globals.DoGZIP(CCTV_Consumer_Globals.sReceivedMessagesDir + "\\CCTVUpdates_" + sCurrDate + ".log"))
                        {
                            CCTV_Consumer_Globals.LogFile.Log("Successfully dumped CCTV messages output to file on disk.");
                        }
                    }
                }


                //if (CCTV_Consumer_PublicDataFunctions.GetCurrentCCTVData())
                //{
                //    CCTV_Consumer_Globals.LogFile.Log("Successfully refreshed stored CCTV data from database.");
                //}

                CCTV_Consumer_Globals.LogFile.Log("CCTV processing finished.");
                CCTV_Consumer_Globals.LogFile.Log(iNumProcessedCCTVs.ToString() + " CCTV(s) processed");

            }
            catch (Exception ex)
            {
                CCTV_Consumer_Globals.LogFile.LogErrorText("Error in HandleCCTVResponse()");
                CCTV_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                CCTV_Consumer_Globals.LogFile.LogError(ex);
            }

        }
    }
}
