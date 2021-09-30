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

namespace Vehicle_Record_Consumer
{
    class Vehicle_Record_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstVehicleRecords)
        {
            VehicleRecord _VehicleRecord = new VehicleRecord();
            JObject jVRCDataObj = new JObject();
            StringBuilder sbVRCpdates = new StringBuilder();
            bool bDoUpdate = true;
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";

            try
            {

                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmss.fff");

                Vehicle_Record_Consumer_Globals.LogFile.Log("Begin processing " + lstVehicleRecords.Count.ToString() + " Vehicle Records.");

                foreach (string sLstItem in lstVehicleRecords)
                {
                    try { 
                        bDoUpdate = true;
                        jVRCDataObj = JObject.Parse(sLstItem);
                        _VehicleRecord = new VehicleRecord();

                        if (bDoUpdate)
                        {
                            _VehicleRecord.DeviceID = Convert.ToInt32(jVRCDataObj.SelectToken("DeviceID"));
                            _VehicleRecord.AgencyID = Convert.ToInt16(jVRCDataObj.SelectToken("AgencyID"));
                            _VehicleRecord.DeviceTypeID = Convert.ToInt32(jVRCDataObj.SelectToken("DeviceTypeID"));
                            _VehicleRecord.DeviceSubTypeID = Convert.ToInt32(jVRCDataObj.SelectToken("DeviceSubTypeID"));

                            _VehicleRecord.Speed =  Convert.ToInt32(jVRCDataObj.SelectToken("Speed"));
                            _VehicleRecord.VehicleLength = Convert.ToDouble(jVRCDataObj.SelectToken("VehicleLength"));
                            _VehicleRecord.Gap = Convert.ToDouble(jVRCDataObj.SelectToken("Gap"));
                            _VehicleRecord.RecordTimestamp = Convert.ToDateTime(jVRCDataObj.SelectToken("RecordTimestamp"));
                            _VehicleRecord.LastUpdate = DateTime.Now;

                            sbVRCpdates.Append(JsonConvert.SerializeObject(_VehicleRecord, Newtonsoft.Json.Formatting.None).Trim());

                            if (!Vehicle_Record_Consumer_ProcessVRC.UpdateVRCRecord(_VehicleRecord))
                            {
                                Vehicle_Record_Consumer_Globals.LogFile.Log("_Vehicle Record update failed. See logs for details.");
                            }

                            
                        }
                    }
                    catch (Exception ex1)
                    {
                        Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error processing individual record in HandleEventResponse(). Continue after error.");
                        Vehicle_Record_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        Vehicle_Record_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                Vehicle_Record_Consumer_Globals.LogFile.Log("Vehicle Record Data processing finished. Processed " + lstVehicleRecords.Count.ToString() + " Vehicle Data Records.");

                if (sbVRCpdates.Length > 0)
                {
                    if (!Directory.Exists(Vehicle_Record_Consumer_Globals.sReceivedMessagesDir))
                    {
                        Directory.CreateDirectory(Vehicle_Record_Consumer_Globals.sReceivedMessagesDir);
                    }

                    using (StreamWriter writer = new StreamWriter(Vehicle_Record_Consumer_Globals.sReceivedMessagesDir + "\\VRCDataUpdates_" + sCurrDate + ".log", true))
                    {
                        sbVRCpdates.Append(Environment.NewLine);
                        sbVRCpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss.fff tt") + ": " + Environment.NewLine);
                        writer.Write(sbVRCpdates.ToString());
                        
                    }

                    if (File.Exists(Vehicle_Record_Consumer_Globals.sReceivedMessagesDir + "\\VRCDataUpdates_" + sCurrDate + ".log"))
                    {
                        if (Vehicle_Record_Consumer_Globals.DoGZIP(Vehicle_Record_Consumer_Globals.sReceivedMessagesDir + "\\VRCDataUpdates_" + sCurrDate + ".log"))
                        {
                            Vehicle_Record_Consumer_Globals.LogFile.Log("Successfully GZipped and deleted original log file.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error in HandleResponse()");
                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Vehicle_Record_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
