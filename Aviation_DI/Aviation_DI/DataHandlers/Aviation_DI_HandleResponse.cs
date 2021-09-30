using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;

namespace Aviation_DI
{
    class Aviation_DI_HandleResponse
    {
        private static string sPreparedStatement = "INSERT INTO db_PA_MOD.tblflightstatus (agency_id, airline_code, airline_name, flight_number, origin_airport_code, " +
           "destination_airport_code, aircraft_type, current_speed, current_altitude, flight_status, scheduled_departure_datetime, estimated_departure_datetime, " +
           "actual_departure_datetime, departure_gate, scheduled_arrival_datetime, estimated_arrival_datetime, actual_arrival_datetime, arrival_gate, latitude, longitude, " +
           "direction, departure_delay, arrival_delay, geom, last_update) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

        public static void HandleAviationResponse(List<string> lstAviationData)
        {
            List<Aviation_DI_FlightData> lstFlights = new List<Aviation_DI_FlightData>();
            Aviation_DI_FlightData _FlightData = new Aviation_DI_FlightData();
            Aviation_DI_StoredFlightData _StoredFlightData = new Aviation_DI_StoredFlightData();
           // List<string> lstAviationData = new List<string>();
            int iNumUpdates = 0;
            int iNumDeletes = 0;
            List<string> lstDeletes = new List<string>();
            List<string> lstNewIDs = new List<string>();

            try
            {
                //lstAviationData = (List<string>)objAviationData;
                Aviation_DI_Globals.LogFile.Log("In Aviation_DI_HandleResponse.HandleAviationResponse(). Begin processing " + lstAviationData.Count.ToString() + " flight data objects.");
                lstNewIDs.Clear();
                lstDeletes.Clear();

                foreach (string sLstItem in lstAviationData)
                {
                    _FlightData = new Aviation_DI_FlightData();
                    _StoredFlightData = new Aviation_DI_StoredFlightData();
                    JObject jAviationObj = JObject.Parse(sLstItem);

                    try
                    {
                        lstNewIDs.Add(jAviationObj["sFlightNum"].ToString());

                        _FlightData.SourceFlightID = jAviationObj["sSourceFlightID"].ToString();
                        _FlightData.FlightNum = jAviationObj["sFlightNum"].ToString();

                        _FlightData.LastUpdate = Convert.ToDateTime(jAviationObj["dtLastUpdate"].ToString());
                        _FlightData.AgencyID = Convert.ToInt32(jAviationObj["iAgencyID"].ToString());
                        _FlightData.Status = jAviationObj["sStatus"].ToString();
                        _FlightData.Type = jAviationObj["sType"].ToString();
                        _FlightData.FlightTypeID = Convert.ToInt32(jAviationObj["FlightTypeID"]);
                        _FlightData.OriginAirportCode = jAviationObj["sOriginAirportCode"].ToString();
                        _FlightData.DestinationAirportCode = jAviationObj["sDestinationAirportCode"].ToString();
                        _FlightData.AircraftType = jAviationObj["sAircraftType"].ToString();
                        _FlightData.ScheduledDeparture = jAviationObj["sScheduledDeparture"].ToString();
                        _FlightData.EstimatedDeparture = jAviationObj["sEstimatedDeparture"].ToString();
                        _FlightData.ActualDeparture = jAviationObj["sActualDeparture"].ToString();

                        if (_FlightData.ScheduledDeparture == "" && _FlightData.EstimatedDeparture != "")
                        {
                            _FlightData.ScheduledDeparture = _FlightData.EstimatedDeparture;
                        }

                        _FlightData.ScheduledArrival = jAviationObj["sScheduledArrival"].ToString();
                        _FlightData.EstimatedArrival = jAviationObj["sEstimatedArrival"].ToString();
                        _FlightData.ActualArrival = jAviationObj["sActualArrival"].ToString();

                        if (_FlightData.ScheduledArrival == "" && _FlightData.EstimatedArrival != "")
                        {
                            _FlightData.ScheduledArrival = _FlightData.EstimatedArrival;
                        }

                        _FlightData.DepartureGate = jAviationObj["sDepartureGate"].ToString();
                        _FlightData.ArrivalGate = jAviationObj["sArrivalGate"].ToString();
                        _FlightData.AirlineName = jAviationObj["sAirlineName"].ToString();
                        _FlightData.AirlineCode = jAviationObj["sAirlineCode"].ToString();
                        _FlightData.CodeShares = jAviationObj["sCodeShares"].ToString();
                        _FlightData.Latitude = Convert.ToDouble(jAviationObj["dLatitude"].ToString());
                        _FlightData.Longitude = Convert.ToDouble(jAviationObj["dLongitude"].ToString());
                        _FlightData.Altitude = Convert.ToDouble(jAviationObj["dAltitude"].ToString());
                        _FlightData.Speed = Convert.ToDouble(jAviationObj["dSpeed"].ToString());
                        _FlightData.Direction = Convert.ToDouble(jAviationObj["dDirection"].ToString());
                        _FlightData.DepartureDelay = Convert.ToInt32(jAviationObj["iDepartureDelay"].ToString());
                        _FlightData.ArrivalDelay = Convert.ToInt32(jAviationObj["iArrivalDelay"].ToString());
                        _FlightData.NumSeats = Convert.ToInt32(jAviationObj["iNumSeats"]);

                        if (Aviation_DI_Globals.dictStoredFlightData.ContainsKey(_FlightData.FlightNum))
                        {
                            _StoredFlightData = Aviation_DI_Globals.dictStoredFlightData[_FlightData.FlightNum];
                            _StoredFlightData.DoUpdate = false;

                            if (_FlightData.LastUpdate > _StoredFlightData.StoredFlight.LastUpdate)
                            {
                                _StoredFlightData.DoUpdate = true;
                                _StoredFlightData.StoredFlight = _FlightData;

                                //add to storage for processing
                                Aviation_DI_Globals.dictStoredFlightData[_FlightData.FlightNum] = _StoredFlightData;
                            }
                            else
                            {
                                //not newer, make sure to set DoUpdate to false
                                _StoredFlightData.DoUpdate = false;
                                Aviation_DI_Globals.dictStoredFlightData[_FlightData.FlightNum] = _StoredFlightData;
                            }
                        }
                        else
                        {
                            _StoredFlightData.StoredFlight = _FlightData;
                            _StoredFlightData.DoUpdate = true;

                            //add to storage for processing
                            Aviation_DI_Globals.dictStoredFlightData.Add(_FlightData.FlightNum, _StoredFlightData);
                        }

                        //dictFlightData.Add(_FlightData.FlightNum, _FlightData);
                        lstFlights.Add(_FlightData);
                    }
                    catch (Exception ex)
                    {
                        Aviation_DI_Globals.LogFile.LogErrorText("Error parsing aviation object in HandleAviationResponse().");
                        Aviation_DI_Globals.LogFile.LogError(ex);
                    }
                    finally
                    {
                        jAviationObj = null;
                    }

                }

                if (lstFlights.Count > 0)
                {
                    Aviation_DI_Globals.LogFile.Log(lstFlights.Count.ToString() + " flight records to update in NoSQL storage. Begin processing.");
                    if (Aviation_DI_Globals.bSendToCassandra)
                    {
                        //Aviation_DI_CassandraCode.DoBatchUpdate(sPreparedStatement, lstFlights);
                    }

                    if (Aviation_DI_Globals.bSendToMongoDB)
                    {
                        Aviation_DI_MongoCode.DoBatchUpdate(lstFlights);
                    }
                }
                else
                {
                    Aviation_DI_Globals.LogFile.Log("No flight records to process into NoSQL storage.");
                }

                Aviation_DI_Globals.LogFile.Log("Now do work in PostgreSQL.");

                foreach (KeyValuePair<string, Aviation_DI_StoredFlightData> kvpFlightData in Aviation_DI_Globals.dictStoredFlightData)
                {
                    if (!lstNewIDs.Contains(kvpFlightData.Value.StoredFlight.FlightNum))
                    {
                        lstDeletes.Add(kvpFlightData.Value.StoredFlight.FlightNum);
                        continue;
                    }

                    if (kvpFlightData.Value.DoUpdate)
                    {
                        if (Aviation_DI_ProcessFlightData.UpdateFlightData(kvpFlightData.Value.StoredFlight))
                        {
                            iNumUpdates++;
                            //Aviation_DI_Globals.LogFile.Log("Updated " + iNumUpdates.ToString() + " records in PostgreSQL");
                        }
                        else
                        {
                            Aviation_DI_Globals.LogFile.LogErrorText("Error inserting flight with source flight num " + kvpFlightData.Key.ToString() + ". See error logs and invalid XML logs for details.");
                            Aviation_DI_Globals.LogFile.LogToFile(GetLogData(kvpFlightData.Value.StoredFlight), "FlightData\\InvalidJSON");
                        }
                    }
                }

                if (iNumUpdates > 0)
                {
                    Aviation_DI_Globals.LogFile.Log("Successfully updated or inserted " + iNumUpdates.ToString() + " records into PostgreSQL.");
                }
                else
                {
                    Aviation_DI_Globals.LogFile.Log("No flight data records were processed in HandleAviationResponse().");
                }

                if (lstDeletes.Count > 0)
                {
                    Aviation_DI_Globals.LogFile.Log("Now do removal of " + lstDeletes.Count.ToString() + " flight records from memory.");
                    foreach (string s in lstDeletes)
                    {
                        if (Aviation_DI_Globals.dictStoredFlightData.ContainsKey(s))
                        {
                            Aviation_DI_Globals.dictStoredFlightData.Remove(s);
                            iNumDeletes++;
                        }
                    }
                    Aviation_DI_Globals.LogFile.Log("Removed " + iNumDeletes.ToString() + " flight records from memory.");
                }

            }
            catch (Exception ex)
            {
                Aviation_DI_Globals.LogFile.LogErrorText("Error parsing aviation object in HandleAviationResponse().");
                Aviation_DI_Globals.LogFile.LogError(ex);
            }
        }

        //private static bool FlightDataChanged(Aviation_DI_FlightData _OriginalFlightData, Aviation_DI_FlightData _NewFlightData)
        //{
        //    //generic function to check if stored flight data has changed from new feed
        //    bool bIsChanged = false;
        //    string sPropertyName = "";
        //    string sOriginalValue = "";
        //    string sNewValue = "";

        //    PropertyInfo[] properties = typeof(Aviation_DI_FlightData).GetProperties();
        //    foreach (PropertyInfo property in properties)
        //    {
        //        try
        //        {
        //            sPropertyName = property.Name;

        //            sOriginalValue = _OriginalFlightData.GetType().GetProperty(sPropertyName).GetValue(_OriginalFlightData, null)?.ToString();
        //            sNewValue = _NewFlightData.GetType().GetProperty(sPropertyName).GetValue(_NewFlightData, null)?.ToString();

        //            if (sOriginalValue != sNewValue)
        //            {
        //                bIsChanged = true;
        //                break;
        //            }
        //        }
        //        catch (Exception ex1)
        //        {
        //            Aviation_DI_Globals.LogFile.LogErrorText("Error in Aviation_DI_HandleResponse.FlightDataChanged(). Skipping comparison and returning true. Will update record anyway.");
        //            Aviation_DI_Globals.LogFile.LogError(ex1);
        //            bIsChanged = true;
        //        }
        //    }

        //    return bIsChanged;
        //}
        private static string GetLogData(Aviation_DI_FlightData _FlightData)
        {
            //generic function to check if stored flight data has changed from new feed
            string sPropertyName = "";
            string sPropertyValue = "";
            string sResult = "";

            PropertyInfo[] properties = typeof(Aviation_DI_FlightData).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                sPropertyName = property.Name;
                sPropertyValue = _FlightData.GetType().GetProperty(sPropertyName).GetValue(_FlightData, null).ToString();

                sResult = sPropertyName + ": " + sPropertyValue + Environment.NewLine;
            }

            return sResult;
        }
    }
}
