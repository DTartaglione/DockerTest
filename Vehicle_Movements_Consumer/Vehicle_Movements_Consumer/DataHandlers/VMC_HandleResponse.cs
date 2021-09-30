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

namespace Vehicle_Movements_Consumer
{
    class Vehicle_Movements_Consumer_HandleResponse
    {
        public static void HandleResponse(List<string> lstVehicleMovements)
        {
            VehicleMovement _VehicleMovement = new VehicleMovement();
            JObject jVMCDataObj = new JObject();
            StringBuilder sbVMCUpdates = new StringBuilder();
            bool bDoUpdate = true;
            DateTime dtCurrDate = new DateTime();
            string sCurrDate = "";
            JToken jTok = null;
            JToken jLocationTok = null;
            JToken jMetricsTok = null;
            JToken jVehicleTok = null;
            JToken jStatusTok = null;

            try
            {

                dtCurrDate = DateTime.Now;
                sCurrDate = dtCurrDate.ToString("MMddyyyy_HHmmss.fff");

                Vehicle_Movements_Consumer_Globals.LogFile.Log("Begin processing " + lstVehicleMovements.Count.ToString() + " Vehicle Records.");

                foreach (string sLstItem in lstVehicleMovements)
                {

                    try
                    {
                        bDoUpdate = true;
                        _VehicleMovement = new VehicleMovement();

                        jVMCDataObj = JObject.Parse(sLstItem);
                        jTok = jVMCDataObj.SelectToken("RawMovementData");

                        if (bDoUpdate)
                        {
                            _VehicleMovement.DataPointId = (string)jTok.SelectToken("dataPointId");
                            _VehicleMovement.JourneyId = (string)jTok.SelectToken("journeyId");
                            _VehicleMovement.CapturedTimestamp = Convert.ToDateTime((string)jTok.SelectToken("capturedTimestamp"));
                            _VehicleMovement.LastUpdate = dtCurrDate;
                            
                            if (jTok.SelectToken("location") != null) {
                                jLocationTok = jTok.SelectToken("location");
                                _VehicleMovement.VehicleLatitude = Convert.ToDouble(jLocationTok.SelectToken("latitude"));
                                _VehicleMovement.VehicleLongitude = Convert.ToDouble(jLocationTok.SelectToken("longitude"));
                                _VehicleMovement.GeoHash = (string)jLocationTok.SelectToken("geohash");
                            }

                            if (jTok.SelectToken("metrics") != null)
                            {
                                jMetricsTok = jTok.SelectToken("metrics");
                                _VehicleMovement.Speed = Convert.ToDouble(jMetricsTok.SelectToken("speed"));
                                _VehicleMovement.Heading = Convert.ToDouble(jMetricsTok.SelectToken("heading"));
                            }

                            if (jTok.SelectToken("vehicle") != null)
                            {
                                jVehicleTok = jTok.SelectToken("vehicle");
                                _VehicleMovement.VehicleIdNum = (string)jVehicleTok.SelectToken("squishVin");
                                _VehicleMovement.VehicleMake = (string)jVehicleTok.SelectToken("vehicleMake");
                                _VehicleMovement.VehicleModel = (string)jVehicleTok.SelectToken("vehicleModel");
                                _VehicleMovement.VehicleYear = String.IsNullOrEmpty((string)jVehicleTok.SelectToken("vehicleYear")) ? 0 : Convert.ToInt32(jVehicleTok.SelectToken("vehicleYear"));

                                if (jVehicleTok.SelectToken("status") != null)
                                {
                                    jStatusTok = jVehicleTok.SelectToken("status");
                                    _VehicleMovement.IgntitionStatus = (string)jStatusTok.SelectToken("ignitionStatus");
                                }
                            }

                            sbVMCUpdates.Append(JsonConvert.SerializeObject(_VehicleMovement, Newtonsoft.Json.Formatting.None).Trim());

                            if (!Vehicle_Movements_Consumer_ProcessVRC.UpdateVMCRecord(_VehicleMovement))
                            {
                                Vehicle_Movements_Consumer_Globals.LogFile.Log("Vehicle Movement update failed. See logs for details.");
                            }
                            
                        }
                    }
                    catch (Exception ex1)
                    {
                        Vehicle_Movements_Consumer_Globals.LogFile.LogErrorText("Error processing individual record in HandleEventResponse(). Continue after error.");
                        Vehicle_Movements_Consumer_Globals.LogFile.LogErrorText(ex1.ToString());
                        Vehicle_Movements_Consumer_Globals.LogFile.LogError(ex1);
                    }
                }

                Vehicle_Movements_Consumer_Globals.LogFile.Log("Vehicle Movement Data processing finished. Processed " + lstVehicleMovements.Count.ToString() + " Vehicle Data Movements.");

                if (sbVMCUpdates.Length > 0)
                {
                    if (!Directory.Exists(Vehicle_Movements_Consumer_Globals.sReceivedMessagesDir))
                    {
                        Directory.CreateDirectory(Vehicle_Movements_Consumer_Globals.sReceivedMessagesDir);
                    }

                    using (StreamWriter writer = new StreamWriter(Vehicle_Movements_Consumer_Globals.sReceivedMessagesDir + "\\VMCDataUpdates_" + sCurrDate + ".log", true))
                    {
                        sbVMCUpdates.Append(Environment.NewLine);
                        sbVMCUpdates.Insert(0, dtCurrDate.ToString("MMddyyyy HH:mm:ss.fff tt") + ": " + Environment.NewLine);
                        writer.Write(sbVMCUpdates.ToString());
                        
                    }

                    if (File.Exists(Vehicle_Movements_Consumer_Globals.sReceivedMessagesDir + "\\VMCDataUpdates_" + sCurrDate + ".log"))
                    {
                        if (Vehicle_Movements_Consumer_Globals.DoGZIP(Vehicle_Movements_Consumer_Globals.sReceivedMessagesDir + "\\VMCDataUpdates_" + sCurrDate + ".log"))
                        {
                            Vehicle_Movements_Consumer_Globals.LogFile.Log("Successfully GZipped and deleted original log file.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Vehicle_Movements_Consumer_Globals.LogFile.LogErrorText("Error in HandleResponse()");
                Vehicle_Movements_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Vehicle_Movements_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
