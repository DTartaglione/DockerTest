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

namespace Marine_Traffic_Data_Consumer
{
    class MTDC_HandleResponse
    {
        public static void HandleResponse(List<string> lstBerthCallData)
        {
            JObject jBerthDataObj = new JObject();
            JToken jPortInfoTok = null;
            JToken jTerminalInfoTok = null;
            JToken jBerthInfoTok = null;
            JToken jShipInfoTok = null;
            BerthItem _BerthItem = new BerthItem();
            PortInfo _PortInfo = new PortInfo();
            TerminalInfo _TerminalInfo = new TerminalInfo();
            BerthInfo _BerthInfo = new BerthInfo();
            ShipInfo _ShipInfo = new ShipInfo();
            List<BerthItem> lstBerthItems = new List<BerthItem>();
            StringBuilder sbBerthCallUpdates = new StringBuilder();
            bool bDoUpdate = true;

            try
            {

                MTDC_Globals.LogFile.Log("Begin processing " + lstBerthCallData.Count.ToString() + " Berth Call Records.");

                foreach (string sLstItem in lstBerthCallData)
                {
                    try { 
                        bDoUpdate = true;
                        jBerthDataObj = JObject.Parse(sLstItem);
                        _BerthItem = new BerthItem();
                        _PortInfo = new PortInfo();
                        _TerminalInfo = new TerminalInfo();
                        _BerthInfo = new BerthInfo();
                        _ShipInfo = new ShipInfo();

                        jBerthDataObj = JObject.Parse(sLstItem);
                        jPortInfoTok = jBerthDataObj["PortInfo"];
                        jTerminalInfoTok = jBerthDataObj["TerminalInfo"];
                        jBerthInfoTok = jBerthDataObj["BerthInfo"];
                        jShipInfoTok = jBerthDataObj["ShipInfo"];

                        if (!MTDC_Globals.lstSubscribeToAgencyIDs.Contains((int)jPortInfoTok["AgencyID"])){
                            MTDC_Globals.LogFile.Log("Agency ID " + ((int)jPortInfoTok["AgencyID"]).ToString() + " not in list of subscribe to agency IDs." +
                                "Adjust config file if needed. Skipping record.");
                            MTDC_Globals.LogFile.Log("Full JSON is " + jBerthDataObj.ToString());
                            bDoUpdate = false;
                            continue;
                        }

                        if (bDoUpdate)
                        {
                            _PortInfo.ID = (int)jPortInfoTok["ID"];
                            _PortInfo.AgencyID = (int)jPortInfoTok["AgencyID"];
                            _PortInfo.UNLOCODE = (string)jPortInfoTok["UNLOCODE"];
                            _PortInfo.Name = (string)jPortInfoTok["Name"];
                            _PortInfo.State = (string)jPortInfoTok["State"];

                            _TerminalInfo.ID = (int)jTerminalInfoTok["ID"];
                            _TerminalInfo.Name = (string)jTerminalInfoTok["Name"];

                            _BerthInfo.ID = (int)jBerthInfoTok["ID"];
                            _BerthInfo.Name = (string)jBerthInfoTok["Name"];
                            _BerthInfo.Latitude = (double)jBerthInfoTok["Latitude"];
                            _BerthInfo.Longitude = (double)jBerthInfoTok["Longitude"];

                            _ShipInfo.ID = (int)jShipInfoTok["ID"];
                            _ShipInfo.SourceShipID = (int)jShipInfoTok["SourceShipID"];
                            _ShipInfo.MMSI_ID = (int)jShipInfoTok["MMSI_ID"];
                            _ShipInfo.IMO_ID = (int)jShipInfoTok["IMO_ID"];
                            _ShipInfo.Name = (string)jShipInfoTok["Name"];
                            _ShipInfo.TypeName = (string)jShipInfoTok["TypeName"];
                            _ShipInfo.DWT = (int)jShipInfoTok["DWT"];
                            _ShipInfo.GRT = (int)jShipInfoTok["GRT"];
                            _ShipInfo.YearBuilt = (int)jShipInfoTok["YearBuilt"];

                            if ((string)jShipInfoTok["ArrivalTimestamp"] != null)
                            {
                                _ShipInfo.ArrivalTimestamp = Convert.ToDateTime(jShipInfoTok["ArrivalTimestamp"]);
                            }
                            if ((string)jShipInfoTok["DepartureTimestamp"] != null)
                            {
                                _ShipInfo.DepartureTimestamp = Convert.ToDateTime(jShipInfoTok["DepartureTimestamp"]);
                            }
                            if ((string)jShipInfoTok["DockTimestamp"] != null)
                            {
                                _ShipInfo.DockTimestamp = Convert.ToDateTime(jShipInfoTok["DockTimestamp"]);
                            }
                            if ((string)jShipInfoTok["UndockTimestamp"] != null)
                            {
                                _ShipInfo.UndockTimestamp = Convert.ToDateTime(jShipInfoTok["UndockTimestamp"]);
                            }
                            
                            _ShipInfo.TimeAtBerth = (int)jShipInfoTok["TimeAtBerth"];
                            _ShipInfo.TimeAtPort = (int)jShipInfoTok["TimeAtPort"];
                            _ShipInfo.ArrivalLoadStatus = (int)jShipInfoTok["ArrivalLoadStatus"];
                            _ShipInfo.DepartureLoadStatus = (int)jShipInfoTok["DepartureLoadStatus"];

                            _BerthItem.PortInfo = _PortInfo;
                            _BerthItem.TerminalInfo = _TerminalInfo;
                            _BerthItem.BerthInfo = _BerthInfo;
                            _BerthItem.ShipInfo = _ShipInfo;

                            if (!MTDC_ProcessBerthCalls.UpdateBerthCallRecord(_BerthItem))
                            {
                                MTDC_Globals.LogFile.Log("ATR record update failed. See logs for details.");
                            }

                            lstBerthItems.Add(_BerthItem);
                        }
                    }
                    catch (Exception ex1)
                    {
                        MTDC_Globals.LogFile.LogErrorText("Error processing individual event in HandleEventResponse(). Continue after error.");
                        MTDC_Globals.LogFile.LogErrorText(ex1.ToString());
                        MTDC_Globals.LogFile.LogError(ex1);
                    }
                }

                if (lstBerthItems.Count > 0)
                {
                    MTDC_Globals.LogFile.Log(lstBerthItems.Count.ToString() + " records to update in NoSQL storage. Begin processing.");

                    if (MTDC_Globals.bSendToMongoDB)
                    {
                        MTDC_MongoCode.DoBatchUpdate(lstBerthItems);
                    }
                }
                else
                {
                    MTDC_Globals.LogFile.Log("No records to process into NoSQL storage.");
                }

                MTDC_Globals.LogFile.Log("Data processing finished. Processed " + lstBerthCallData.Count.ToString() + " Records.");

            }
            catch (Exception ex)
            {
                MTDC_Globals.LogFile.LogErrorText("Error in HandleResponse()");
                MTDC_Globals.LogFile.LogErrorText(ex.ToString());
                MTDC_Globals.LogFile.LogError(ex);
            }
        }
    }
}
