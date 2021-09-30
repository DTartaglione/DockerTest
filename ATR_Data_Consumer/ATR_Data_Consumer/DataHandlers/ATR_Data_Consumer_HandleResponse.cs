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

namespace ATR_Data_Consumer
{
    class ATR_Data_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstATRData)
        {
            ATRData _ATRData = new ATRData();
            JObject jATRDataObj = new JObject();
            StringBuilder sbATRUpdates = new StringBuilder();
            bool bDoUpdate = true;
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";

            try
            {

                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmss.fff");

                ATR_Data_Consumer_Globals.LogFile.Log("Begin processing " + lstATRData.Count.ToString() + " ATR Data Records.");

                foreach (string sLstItem in lstATRData)
                {
                    try { 
                        bDoUpdate = true;
                        jATRDataObj = JObject.Parse(sLstItem);
                        _ATRData = new ATRData();


                        if (bDoUpdate)
                        {
                            _ATRData.DeviceID = Convert.ToInt32(jATRDataObj.SelectToken("DeviceID"));
                            _ATRData.DeviceName = (string)jATRDataObj.SelectToken("DeviceName");
                            _ATRData.AgencyID = Convert.ToInt16(jATRDataObj.SelectToken("AgencyID"));
                            _ATRData.DeviceTypeID = Convert.ToInt32(jATRDataObj.SelectToken("DeviceTypeID"));
                            _ATRData.DeviceSubTypeID = Convert.ToInt32(jATRDataObj.SelectToken("DeviceSubTypeID"));

                            if (ATR_Data_Consumer_Globals.dictSensorMapping.ContainsKey(_ATRData.AgencyID + "|" + _ATRData.DeviceName
                                + "|" + _ATRData.DeviceTypeID + "|" + _ATRData.DeviceSubTypeID)){

                                _ATRData.DeviceID = ATR_Data_Consumer_Globals.dictSensorMapping[_ATRData.AgencyID + "|" + _ATRData.DeviceName
                                + "|" + _ATRData.DeviceTypeID + "|" + _ATRData.DeviceSubTypeID];

                            }
                            
                            _ATRData.DeviceType = (string)jATRDataObj.SelectToken("DeviceType");
                            
                            _ATRData.DeviceSubType = (string)jATRDataObj.SelectToken("DeviceSubTypeID");
                            _ATRData.DeviceParentID = Convert.ToInt32(jATRDataObj.SelectToken("DeviceParentID"));
                            _ATRData.ReportingIntervalInMinutes = Convert.ToInt32(jATRDataObj.SelectToken("ReportingIntervalInMinutes"));
                            _ATRData.Occupancy =  Convert.ToDouble(jATRDataObj.SelectToken("Occupancy"));
                            _ATRData.Volume = Convert.ToInt16(jATRDataObj.SelectToken("Volume"));
                            _ATRData.SpeedInMph = Convert.ToInt16(jATRDataObj.SelectToken("SpeedInMph"));
                            _ATRData.IntervalEnd = Convert.ToDateTime(jATRDataObj.SelectToken("IntervalEnd"));
                            _ATRData.LastUpdate = Convert.ToDateTime(jATRDataObj.SelectToken("LastUpdate"));

                            sbATRUpdates.Append(JsonConvert.SerializeObject(_ATRData, Newtonsoft.Json.Formatting.None).Trim());

                            if (!ATR_Data_Consumer_ProcessATR.UpdateATRRecord(_ATRData))
                            {
                                ATR_Data_Consumer_Globals.LogFile.Log("ATR record update failed. See logs for details.");
                            }

                            
                        }
                    }
                    catch (Exception ex1)
                    {
                        ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error processing individual event in HandleEventResponse(). Continue after error.");
                        ATR_Data_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        ATR_Data_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                ATR_Data_Consumer_Globals.LogFile.Log("ATR Data processing finished. Processed " + lstATRData.Count.ToString() + " ATR Data Records.");

                if (sbATRUpdates.Length > 0)
                {
                    if (!Directory.Exists(ATR_Data_Consumer_Globals.sReceivedMessagesDir))
                    {
                        Directory.CreateDirectory(ATR_Data_Consumer_Globals.sReceivedMessagesDir);
                    }

                    using (StreamWriter writer = new StreamWriter(ATR_Data_Consumer_Globals.sReceivedMessagesDir + "\\ATRDataUpdates_" + sCurrDate + ".log", true))
                    {
                        sbATRUpdates.Append(Environment.NewLine);
                        sbATRUpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss.fff tt") + ": " + Environment.NewLine);
                        writer.Write(sbATRUpdates.ToString());
                        
                    }

                    if (File.Exists(ATR_Data_Consumer_Globals.sReceivedMessagesDir + "\\ATRDataUpdates_" + sCurrDate + ".log"))
                    {
                        if (ATR_Data_Consumer_Globals.DoGZIP(ATR_Data_Consumer_Globals.sReceivedMessagesDir + "\\ATRDataUpdates_" + sCurrDate + ".log"))
                        {
                            ATR_Data_Consumer_Globals.LogFile.Log("Successfully GZipped and deleted original log file.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error in HandleResponse()");
                ATR_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                ATR_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
