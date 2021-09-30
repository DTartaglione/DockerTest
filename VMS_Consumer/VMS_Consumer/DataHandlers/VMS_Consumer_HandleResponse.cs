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

namespace VMS_Consumer
{
    class VMS_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstVMSData)
        {
            JObject jVMSObj = new JObject();
            List<string> lstDeletes = new List<string>();
            List<string> lstNewIDs = new List<string>();
            List<VMSData> lstVMSUpdates = new List<VMSData>();
            StringBuilder sbVMSUpdates = new StringBuilder();
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";
            Double dLat = 0;
            Double dLon = 0;

            int iNumProcessedVMSs = 0;

            VMSData _VMS;
           // StoredVMSData _StoredVMS;
            VMS_Consumer_ProcessVMS clProcVMS = new VMS_Consumer_ProcessVMS();
            //DateTime dtFeedLastUpdate;
            bool bDoUpdate = true;
            string sLastUpdate = "";
           

            try
            {
                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmssfff");

                VMS_Consumer_Globals.LogFile.Log("In HandleResponse(). Begin processing " + lstVMSData.Count.ToString() + " VMS data objects.");
                lstNewIDs.Clear();
                lstDeletes.Clear();
                lstVMSUpdates.Clear();

                if (VMS_Consumer_Globals.bSimulationEnabled)
                {
                    VMS_Consumer_Globals.LogFile.Log("Simulation mode enabled. VMSs will NOT be removed.");
                }

                foreach (string sLstItem in lstVMSData)
                {
                    try { 
                        bDoUpdate = true;
                        jVMSObj = JObject.Parse(sLstItem);
                        _VMS = new VMSData();
                        dLat = 0;
                        dLon = 0;

                        _VMS.SourceVMSID = (string)jVMSObj.SelectToken("SourceVMSID");
                        _VMS.AgencyID = (int)jVMSObj.SelectToken("AgencyID");

                        sLastUpdate = (string)jVMSObj.SelectToken("LastUpdate");

                        _VMS.LastUpdate = Convert.ToDateTime((string)jVMSObj.SelectToken("LastUpdate")).ToUniversalTime();

                        if (VMS_Consumer_Globals.dictVMSIDMap.ContainsKey(_VMS.AgencyID.ToString() + "|" + _VMS.SourceVMSID.ToString())){
                            //VMS exists in map. Check if do update needed. If no change, ignore.
                            
                            if (_VMS.LastUpdate <= VMS_Consumer_Globals.dictVMSIDMap[_VMS.AgencyID.ToString() + "|" + _VMS.SourceVMSID].LastUpdate)
                            {
                                //VMS is not newer than our record. no need to update
                                bDoUpdate = false;
                                continue;
                            }

                            _VMS.LocalVMSID = VMS_Consumer_Globals.dictVMSIDMap[_VMS.AgencyID.ToString() + "|" + _VMS.SourceVMSID].LocalVMSID;
                        }

                        if (bDoUpdate)
                        {
                            _VMS.VMSName = (string)jVMSObj.SelectToken("VMSName");
                            _VMS.Roadway = (string)jVMSObj.SelectToken("Roadway");
                            _VMS.Direction = (string)jVMSObj.SelectToken("Direction");

                            _VMS.Coords = new Point();
                            if (Double.TryParse((string)jVMSObj.SelectToken("Point").SelectToken("Latitude"), out dLat))
                            {
                                _VMS.Coords.Latitude = dLat;
                            }

                            if (Double.TryParse((string)jVMSObj.SelectToken("Point").SelectToken("Longitude"), out dLon))
                            {
                                _VMS.Coords.Longitude = dLon;
                            }

                            _VMS.Message = (string)jVMSObj.SelectToken("Message");

                            if (clProcVMS.UpdateVMS(_VMS))
                            {
                                //VMS_Consumer_Globals.LogFile.Log("VMS with source ID " + _VMS.SourceVMSID + " successfully processed.");
                                iNumProcessedVMSs++;
                                lstVMSUpdates.Add(_VMS);
                            }
                            else
                            {
                                VMS_Consumer_Globals.LogFile.LogErrorText("Error processing VMS with source ID " + _VMS.SourceVMSID + ". See error logs and invalid XML logs for details.");
                                VMS_Consumer_Globals.LogFile.LogToFile(jVMSObj.ToString(), "VMSs\\InvalidVMSXML");
                            }

                            sbVMSUpdates.Append(JsonConvert.SerializeObject(_VMS, Newtonsoft.Json.Formatting.None).Trim());
                        }
                    }
                    catch (Exception ex1)
                    {
                        VMS_Consumer_Globals.LogFile.LogErrorText("Error processing individual VMS in HandleVMSResponse(). Continue after error.");
                        VMS_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        VMS_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                if (lstVMSUpdates.Count > 0)
                {
                    VMS_Consumer_Globals.LogFile.Log(lstVMSUpdates.Count.ToString() + " records to update in NoSQL storage. Begin processing.");

                    if (VMS_Consumer_Globals.bSendToMongoDB)
                    {
                        VMS_Consumer_MongoCode.DoBatchUpdate(lstVMSUpdates);
                    }
                }
                else
                {
                    VMS_Consumer_Globals.LogFile.Log("No records to process into NoSQL storage.");
                }

                //if set to true in app.config, log all messages received. for debugging.
                if (VMS_Consumer_Globals.bLogAllIncomingMessages)
                {
                    if (!Directory.Exists(VMS_Consumer_Globals.sReceivedMessagesDir)) {
                        Directory.CreateDirectory(VMS_Consumer_Globals.sReceivedMessagesDir);
                    }

                    if (sbVMSUpdates.Length > 0)
                    {
                        using (StreamWriter writer = new StreamWriter(VMS_Consumer_Globals.sReceivedMessagesDir + "\\VMSUpdates_" + sCurrDate + ".log", true))
                        {
                            sbVMSUpdates.Append(Environment.NewLine);
                            sbVMSUpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss tt") + ": " + Environment.NewLine);
                            writer.Write(sbVMSUpdates.ToString());
                            sbVMSUpdates.Remove(0, sbVMSUpdates.Length);
                        }

                        sbVMSUpdates.Remove(0, sbVMSUpdates.Length);

                        if (VMS_Consumer_Globals.DoGZIP(VMS_Consumer_Globals.sReceivedMessagesDir + "\\VMSUpdates_" + sCurrDate + ".log"))
                        {
                            VMS_Consumer_Globals.LogFile.Log("Successfully dumped VMS messages output to file on disk.");
                        }
                    }
                }


                if (VMS_Consumer_PublicDataFunctions.GetCurrentVMSData())
                {
                    VMS_Consumer_Globals.LogFile.Log("Successfully refreshed stored VMS data from database.");
                }

                VMS_Consumer_Globals.LogFile.Log("VMS processing finished.");
                VMS_Consumer_Globals.LogFile.Log(iNumProcessedVMSs.ToString() + " VMS(s) processed");

            }
            catch (Exception ex)
            {
                VMS_Consumer_Globals.LogFile.LogErrorText("Error in HandleVMSResponse()");
                VMS_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                VMS_Consumer_Globals.LogFile.LogError(ex);
            }

        }
    }
}
