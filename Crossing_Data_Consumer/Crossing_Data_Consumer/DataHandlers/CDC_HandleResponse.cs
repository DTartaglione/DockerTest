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

namespace Crossing_Data_Consumer
{
    class Crossing_Data_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstCrossingData)
        {
            CrossingData _CrossingData = new CrossingData();
            JObject jCrossingDataObj = new JObject();
            StringBuilder sCrossingUpdates = new StringBuilder();
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";
            int iNumUpdates = 0;

            try
            {

                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmss.fff");

                Crossing_Data_Consumer_Globals.LogFile.Log("Begin processing " + lstCrossingData.Count.ToString() + " Crossing Data Records.");

                foreach (string sLstItem in lstCrossingData)
                {
                    try { 
                        jCrossingDataObj = JObject.Parse(sLstItem);

                        if (Crossing_Data_Consumer_Globals.lstSubscribeToAgencyIDs.Contains((string)jCrossingDataObj.SelectToken("AgencyId"))) {
                            if (!Crossing_Data_Consumer_Globals.dictCrossingLookup.ContainsKey(Convert.ToInt32((string)jCrossingDataObj.SelectToken("LocalFacilityId"))))
                            {
                                Crossing_Data_Consumer_Globals.LogFile.LogErrorText("No matching local facility ID for facility " +
                                    Convert.ToInt32((string)jCrossingDataObj.SelectToken("LocalFacilityName")).ToString() + ". Skipping.");
                                continue;
                            }

                            _CrossingData = new CrossingData();
                            _CrossingData.AgencyId = Convert.ToInt32((string)jCrossingDataObj.SelectToken("AgencyId"));
                            _CrossingData.LocalFacilityId = Convert.ToInt32((string)jCrossingDataObj.SelectToken("LocalFacilityId"));
                            _CrossingData.LocalFacilityName = (string)jCrossingDataObj.SelectToken("LocalFacilityName");
                            _CrossingData.CAWaitTimeMins = Convert.ToInt32(jCrossingDataObj.SelectToken("CAWaitTimeMins"));
                            _CrossingData.USWaitTimeMins = Convert.ToInt32(jCrossingDataObj.SelectToken("USWaitTimeMins"));
                            _CrossingData.LastUpdate = Convert.ToDateTime((string)jCrossingDataObj.SelectToken("LastUpdate"));

                            sCrossingUpdates.Append(JsonConvert.SerializeObject(_CrossingData, Newtonsoft.Json.Formatting.None).Trim());

                            if (!Crossing_Data_Consumer_ProcessCrossingRecords.ProcessCrossingRecords(_CrossingData))
                            {
                                Crossing_Data_Consumer_Globals.LogFile.Log("Crossing Data Record update failed. See logs for details.");
                            }
                            else
                            {
                                iNumUpdates++;
                            }
                        }
                        else
                        {
                            Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Consumer not currently configured to subscribe to agency ID " +
                                (string)jCrossingDataObj.SelectToken("AgencyId") + ". Add to app.config file SubscribeToAgencyIDs comma separated parameter list.");
                        }
                    }
                    catch (Exception ex1)
                    {
                        Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error processing individual record in HandleEventResponse(). Continue after error.");
                        Crossing_Data_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        Crossing_Data_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                Crossing_Data_Consumer_Globals.LogFile.Log("Crossing Data Record processing finished. Processed " + iNumUpdates.ToString() + " Crossing Data Records.");

                if (sCrossingUpdates.Length > 0 && Crossing_Data_Consumer_Globals.bStoreReceivedMessages)
                {
                    if (!Directory.Exists(Crossing_Data_Consumer_Globals.sReceivedMessagesDir))
                    {
                        Directory.CreateDirectory(Crossing_Data_Consumer_Globals.sReceivedMessagesDir);
                    }

                    using (StreamWriter writer = new StreamWriter(Crossing_Data_Consumer_Globals.sReceivedMessagesDir + "\\CrossingDataUpdates_" + sCurrDate + ".log", true))
                    {
                        sCrossingUpdates.Append(Environment.NewLine);
                        sCrossingUpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss.fff tt") + ": " + Environment.NewLine);
                        writer.Write(sCrossingUpdates.ToString());
                        
                    }

                    if (File.Exists(Crossing_Data_Consumer_Globals.sReceivedMessagesDir + "\\CrossingDataUpdates_" + sCurrDate + ".log"))
                    {
                        if (Crossing_Data_Consumer_Globals.DoGZIP(Crossing_Data_Consumer_Globals.sReceivedMessagesDir + "\\CrossingDataUpdates_" + sCurrDate + ".log"))
                        {
                            Crossing_Data_Consumer_Globals.LogFile.Log("Successfully GZipped and deleted original log file.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error in HandleResponse()");
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Crossing_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
