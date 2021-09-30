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

namespace Video_Detection_Consumer
{
    class Video_Detection_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstSensorZones)
        {
            SensorZone _SensorZone = new SensorZone();
            JObject jVDSDataObj = new JObject();
            StringBuilder sbVDSUpdates = new StringBuilder();
            bool bDoUpdate = true;
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";

            try
            {

                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmss.fff");

                Video_Detection_Consumer_Globals.LogFile.Log("Begin processing " + lstSensorZones.Count.ToString() + " Sensor Zones.");

                foreach (string sLstItem in lstSensorZones)
                {

                    try
                    {
                        bDoUpdate = true;
                        _SensorZone = new SensorZone();

                        jVDSDataObj = JObject.Parse(sLstItem);

                        if (bDoUpdate)
                        {
                            _SensorZone.DeviceID = Convert.ToInt32(jVDSDataObj.SelectToken("DeviceID"));
                            _SensorZone.NativeDeviceID = (string)jVDSDataObj.SelectToken("NativeDeviceID");
                            _SensorZone.AgencyID = Convert.ToInt32(jVDSDataObj.SelectToken("AgencyID"));

                            if (Video_Detection_Consumer_Globals.dictSensorZones.ContainsKey(_SensorZone.NativeDeviceID + "|" + _SensorZone.AgencyID) &&
                                Convert.ToInt32(jVDSDataObj.SelectToken("DeviceID")) != 0) {
                                //device exists, update existing
                                _SensorZone = Video_Detection_Consumer_Globals.dictSensorZones[_SensorZone.NativeDeviceID + "|" + _SensorZone.AgencyID];
                            }
                            
                            _SensorZone.DeviceTypeID = Convert.ToInt32(jVDSDataObj.SelectToken("DeviceTypeID"));
                            _SensorZone.DeviceType = (string)jVDSDataObj.SelectToken("DeviceType");
                            _SensorZone.DeviceSubTypeID = Convert.ToInt32(jVDSDataObj.SelectToken("DeviceSubTypeID"));
                            _SensorZone.DeviceName = (string)jVDSDataObj.SelectToken("DeviceName");
                            _SensorZone.ReportingIntervalInMinutes = Convert.ToInt32(jVDSDataObj.SelectToken("ReportingIntervalInMinutes"));
                            _SensorZone.Occupancy = Convert.ToInt32(jVDSDataObj.SelectToken("ReportingIntervalInMinutes"));
                            _SensorZone.Volume = Convert.ToInt32(jVDSDataObj.SelectToken("Volume"));
                            _SensorZone.SpeedInMph = Convert.ToInt32(jVDSDataObj.SelectToken("SpeedInMph"));
                            _SensorZone.DirectionId = Convert.ToInt32(jVDSDataObj.SelectToken("DirectionId"));
                            _SensorZone.IntervalTimestamp = Convert.ToDateTime(jVDSDataObj.SelectToken("LastUpdate"));

                            sbVDSUpdates.Append(JsonConvert.SerializeObject(_SensorZone, Newtonsoft.Json.Formatting.None).Trim());

                            if (!Video_Detection_Consumer_ProcessVRC.UpdateVDSRecord(_SensorZone))
                            {
                                Video_Detection_Consumer_Globals.LogFile.Log("Vehicle Movement update failed. See logs for details.");
                            }
                            
                        }
                    }
                    catch (Exception ex1)
                    {
                        Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error processing individual record in HandleEventResponse(). Continue after error.");
                        Video_Detection_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        Video_Detection_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                Video_Detection_Consumer_Globals.LogFile.Log("Vehicle Movement Data processing finished. Processed " + lstSensorZones.Count.ToString() + " Video Sensor Zones.");

                if (sbVDSUpdates.Length > 0)
                {
                    if (!Directory.Exists(Video_Detection_Consumer_Globals.sReceivedMessagesDir))
                    {
                        Directory.CreateDirectory(Video_Detection_Consumer_Globals.sReceivedMessagesDir);
                    }

                    using (StreamWriter writer = new StreamWriter(Video_Detection_Consumer_Globals.sReceivedMessagesDir + "\\VDSDataUpdates_" + sCurrDate + ".log", true))
                    {
                        sbVDSUpdates.Append(Environment.NewLine);
                        sbVDSUpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss.fff tt") + ": " + Environment.NewLine);
                        writer.Write(sbVDSUpdates.ToString());
                        
                    }

                    if (File.Exists(Video_Detection_Consumer_Globals.sReceivedMessagesDir + "\\VDSDataUpdates_" + sCurrDate + ".log"))
                    {
                        if (Video_Detection_Consumer_Globals.DoGZIP(Video_Detection_Consumer_Globals.sReceivedMessagesDir + "\\VDSDataUpdates_" + sCurrDate + ".log"))
                        {
                            Video_Detection_Consumer_Globals.LogFile.Log("Successfully GZipped and deleted original log file.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error in HandleResponse()");
                Video_Detection_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Video_Detection_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
