using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PAX_Consumer
{
    class PAX_Consumer_HandleResponse
    {

        public static void HandleResponse(List<dynamic> lstData)
        {

            List<PAX> lstPAX = new List<PAX>();
            List<PAX> lstEnplanements = new List<PAX>();
            string[] aDataRow = new string[] { };
            int iPAXType = 0;
            int iNumUpdates = 0;
            JToken jTok = null;

            foreach (dynamic lstItem in lstData)
            {
                try
                {
                    //first check if JSON or Comma Separated format
                    try
                    {
                        jTok = JToken.Parse(lstItem);
                        iPAXType = Convert.ToInt16(jTok.SelectToken("PAXClassification"));
                    }
                    catch (JsonReaderException jEx)
                    {
                        //PAX_Consumer_Globals.LogFile.Log("Item received from Kafka is not JSON, parse CSV. This is ok and is expected. Will parse CSV instead of JSON.");
                        //PAX_Consumer_Globals.LogFile.Log("ERROR MESSAGE FOR INFO ONLY! " + jEx.ToString());
                        aDataRow = lstItem.Split(new string[] { "," }, StringSplitOptions.None);
                        iPAXType = Convert.ToInt32(aDataRow[0]);
                    }

                    //Split row into string list. 

                    //PAX _PAX = new PAX();
                    //second PAX object in case one row has dept and arrv data (45-Day Delta Projection Does this
                    //PAX _PAX2 = new PAX();

                    if (iPAXType == PAX_Consumer_Globals.iAADailyProjectionsTypeID)
                    {
                        PAX _PAX = HandleAADailyProjections(aDataRow);
                        _PAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[iPAXType];
                        _PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                        lstPAX.Add(_PAX);
                    }
                    //else if (iPAXType == PAX_Consumer_Globals.iDeltaDailyProjectionsTypeID)
                    //{
                    //   PAX _PAX = HandleDeltaHourlyProjections(aDataRow);
                    //   _PAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[iPAXType];
                    //   _PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                    //   lstPAX.Add(_PAX);
                    // }
                    else if (iPAXType == PAX_Consumer_Globals.iDeltaMonthlyProjectionsTypeID)
                    {
                        //If arrival and departure from 45-day are combined, then process as original logic; otherwise handle as unique row
                        if (aDataRow.Length > 12)
                        {
                            //Arrivals first, then departures
                            PAX _PAX = HandleDeltaMonthlyProjections(aDataRow, PAX_Consumer_Globals.dictFlightType[PAX_Consumer_Globals.sArrivalsTypeName]);
                            PAX _PAX2 = HandleDeltaMonthlyProjections(aDataRow, PAX_Consumer_Globals.dictFlightType[PAX_Consumer_Globals.sDeparturesTypeName]);
                            _PAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[iPAXType];
                            _PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                            lstPAX.Add(_PAX);
                            _PAX2.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[iPAXType];
                            _PAX2.AirportID = PAX_Consumer_Globals.iAirportID;
                            lstPAX.Add(_PAX2);
                        }
                        else
                        {
                            PAX _PAX = HandleDeltaMonthlyProjections(aDataRow, -1);
                            _PAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[iPAXType];
                            _PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                            lstPAX.Add(_PAX);
                        }
                    }
                    //else if (iPAXType == PAX_Consumer_Globals.iDeltaThreeDayProjectionsTypeID)
                    //{
                    //    //Arrivals first, then departures
                    //    PAX _PAX = HandleDeltaMonthlyProjections(aDataRow, PAX_Consumer_Globals.dictFlightType[PAX_Consumer_Globals.sArrivalsTypeName]);
                    //    PAX _PAX2 = HandleDeltaMonthlyProjections(aDataRow, PAX_Consumer_Globals.dictFlightType[PAX_Consumer_Globals.sDeparturesTypeName]);
                    //    _PAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[iPAXType];
                    //    _PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                    //    lstPAX.Add(_PAX);
                    //    _PAX2.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[iPAXType];
                    //    _PAX2.AirportID = PAX_Consumer_Globals.iAirportID;
                    //    lstPAX.Add(_PAX2);

                    //}
                    else if (iPAXType == PAX_Consumer_Globals.iTSAEnplanementsTypeID || iPAXType == PAX_Consumer_Globals.iCBPPAXTypeID ||
                        iPAXType == PAX_Consumer_Globals.iDeltaDailyProjectionsTypeID || iPAXType == PAX_Consumer_Globals.iDeltaHourlyProjectionsTypeID
                        || iPAXType == PAX_Consumer_Globals.iTerminalBProjectionsID)
                    {
                        // PAX _PAX = HandleTSAEnplanements(aDataRow);
                        //lstPAX.AddRange(HandleEnplanements(jTok));
                        //PAX _PAX = HandleEnplanements(jTok);
                        //_PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                        //lstEnplanements.Add(_PAX);
                        lstEnplanements.AddRange(HandleJSONPax(jTok));
                    }
                    //else if (iPAXType == PAX_Consumer_Globals.iCBPPAXTypeID)
                    //{
                    //PAX _PAX = HandleEnplanements(jTok);
                    //_PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                    //lstEnplanements.Add(_PAX);
                    //    lstEnplanements.AddRange(HandleEnplanements(jTok));
                    // }
                    else if (iPAXType == PAX_Consumer_Globals.iJetBlueDailyProjectionsID)
                    {
                        //JetBlue row may contain two flights so a tuple is used for both objects

                        Tuple<PAX, PAX> tPAX = HandleJetBlueDailyProjections(aDataRow);
                        if (tPAX.Item1.Volume.Count > 0)
                        {
                            lstPAX.Add(tPAX.Item1);
                        }
                        if (tPAX.Item2.Volume.Count > 0)
                        {
                            lstPAX.Add(tPAX.Item2);
                        }
                        
                    }
                    else if (iPAXType == PAX_Consumer_Globals.iDeltaSixDayDetailedProjectionsID)
                    {
                        lstPAX.Add(HandleDeltaSixDayDetailedProjections(aDataRow));
                    }

                    else
                    {
                        PAX_Consumer_Globals.LogFile.Log("No matching data type for PAXType: " + iPAXType.ToString() + ". Please verify database and config settings. Skipping.");
                    }

                }
                catch (Exception ex)
                {
                    PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row from Kafka in HandleResponse().");
                    PAX_Consumer_Globals.LogFile.LogError(ex);
                }
            }

            //Finished processing Kafka rows. Send to database
            if (lstPAX.Count > 0 || lstEnplanements.Count > 0)
            {
                PAX_Consumer_Globals.LogFile.Log("All new PAX data finished processing. Begin sending to database.");

                foreach (PAX lstPAXItem in lstPAX)
                {
                    if (PAX_Consumer_ProcessData.UpdatePAXData(lstPAXItem))
                    {
                        iNumUpdates++;
                    }
                    else
                    {
                        PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row for flight type: " + lstPAXItem.PAXTypeID.ToString() + 
                            "for time period: " + lstPAXItem.PAXDate);
                    }
                }

                foreach (PAX lstEnplanementsItem in lstEnplanements)
                {
                    if (lstEnplanementsItem.PAXClassification == PAX_Consumer_Globals.iTSAEnplanementsTypeID)
                    {
                        if (PAX_Consumer_ProcessData.UpdateTSAEnplanementData(lstEnplanementsItem))
                        {
                            iNumUpdates++;
                        }
                        else
                        {
                            PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row for flight type: " + lstEnplanementsItem.PAXTypeID.ToString() +
                                "for time period: " + lstEnplanementsItem.PAXDate);
                        }
                    }
                    else if (lstEnplanementsItem.PAXClassification == PAX_Consumer_Globals.iCBPPAXTypeID)
                    {
                        if (PAX_Consumer_ProcessData.UpdateCBPEnplanementData(lstEnplanementsItem))
                        {
                            iNumUpdates++;
                        }
                        else
                        {
                            PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row for flight type: " + lstEnplanementsItem.PAXTypeID.ToString() +
                                "for time period: " + lstEnplanementsItem.PAXDate);
                        }
                    }
                    else if (lstEnplanementsItem.PAXClassification == PAX_Consumer_Globals.iDeltaHourlyProjectionsTypeID || lstEnplanementsItem.PAXClassification == PAX_Consumer_Globals.iTerminalBProjectionsID)
                    {
                        if (PAX_Consumer_ProcessData.UpdatePAXData(lstEnplanementsItem))
                        {
                            iNumUpdates++;
                        }
                        else
                        {
                            PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row for flight type: " + lstEnplanementsItem.PAXTypeID.ToString() +
                                "for time period: " + lstEnplanementsItem.PAXDate);
                        }
                    }
                }

                if (iNumUpdates > 0)
                {
                    PAX_Consumer_Globals.LogFile.Log("Sucessfully inserted " + iNumUpdates.ToString() + " rows to Postgres.");
                }
                        
                if (PAX_Consumer_Globals.bSendToMongoDB)
                {
                    PAX_Consumer_MongoCode.DoBatchUpdate(lstPAX);
                }
            }


            

        }

        public static PAX HandleDeltaHourlyProjections(string[] aDeltaHourlyProjData)
        {
            PAX deltaHourlyPAX = new PAX();
            PAXVolume _PAXVolume = new PAXVolume();
            List<PAXVolume> lstPAXVolumes = new List<PAXVolume>();
            //PAXType,FlightType,Hour,Volume,LastUpdate
            //1,2,1/21/2020 11:00:00 PM,806,1/15/2020 7:07:31 AM
            try
            {
                deltaHourlyPAX.PAXTypeID = Convert.ToInt32(aDeltaHourlyProjData[0]);
                deltaHourlyPAX.FlightType = Convert.ToInt32(aDeltaHourlyProjData[1]);
                deltaHourlyPAX.PAXDate = Convert.ToDateTime(aDeltaHourlyProjData[2]).Date;
                _PAXVolume.PAXHourTimestamp = Convert.ToDateTime(aDeltaHourlyProjData[2]);
                _PAXVolume.Volume = Convert.ToInt32(aDeltaHourlyProjData[3]);
                //deltaHourlyPAX.PAXHourTimestamp = Convert.ToDateTime(aDeltaHourlyProjData[2]);
                //deltaHourlyPAX.Volume = Convert.ToInt32(aDeltaHourlyProjData[3]);
                deltaHourlyPAX.LastUpdate = Convert.ToDateTime(aDeltaHourlyProjData[4]);
                lstPAXVolumes.Add(_PAXVolume);

                deltaHourlyPAX.Volume.AddRange(lstPAXVolumes); 
            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row in HandleDeltaHourlyProjections().");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return deltaHourlyPAX;
        }

        public static PAX HandleDeltaMonthlyProjections(string[] aDeltaMonthlyProjData, int iFlightType)
        {
            PAX deltaMonthlyPAX = new PAX();
            PAXVolume _PAXVolume = new PAXVolume();
            List<PAXVolume> lstPAXVolumes = new List<PAXVolume>();
            DateTime dtLastUpdate = new DateTime();
            DateTime dtPAXDate = new DateTime();
            int iFlightTypeID = 0;
            //PAXType,FlightType,Date,Day,Arrv Volume,Max Volume,Arrv Load Factor,Arrv Flight Count,Dept Volume before 1200,Dept Volume Between 12:00 and 17:00,Dept Volume After 17:00, Dept Cap, Dept Load Factor, Dept Flight Count
            //2,2,21JAN2020,TUE,14118,24832,57%,253,5860,3829,3964,13652,24824,55%,253,1/13/2020 10:37:13 AM
            //Table has recently been separated into arrivals and departures
            //2,0,12JAN,TUE,3043,5418,56,49,1/11/2021 2:47:07 PM
            //2,1,12JAN,TUE,1265,783,425,2473,5707,43,51,1/11/2021 2:47:07 PM
            
            //If param iFlightType is 0,1 use it for FlightTypeID; otherwise, use array value
            if (iFlightType < 0)
            {
                iFlightTypeID = Convert.ToInt32(aDeltaMonthlyProjData[1]);
            }
            else
            {
                iFlightTypeID = iFlightType;
            }

            try
            {
                
                if (iFlightTypeID == PAX_Consumer_Globals.dictFlightType[PAX_Consumer_Globals.sArrivalsTypeName])
                {
                    dtPAXDate = DateTime.Parse(aDeltaMonthlyProjData[2]).Date;
                    if (aDeltaMonthlyProjData.Length > 12)
                    {
                        dtLastUpdate = Convert.ToDateTime(aDeltaMonthlyProjData[15]);
                    }
                    else
                    {
                        dtLastUpdate = Convert.ToDateTime(aDeltaMonthlyProjData[8]);
                        //Since new tables do not provide year, check if last_update is 45 days greater than last update. This will determine which rows need to remove one year
                        if (dtPAXDate > dtLastUpdate.AddDays(45))
                        {
                            dtPAXDate = dtPAXDate.AddYears(-1);
                        }
                    }

                    deltaMonthlyPAX.PAXTypeID = Convert.ToInt32(aDeltaMonthlyProjData[0]);
                    deltaMonthlyPAX.FlightType = iFlightTypeID;
                    deltaMonthlyPAX.PAXDate = dtPAXDate;
                    _PAXVolume.Volume = Convert.ToInt32(aDeltaMonthlyProjData[4]);
                    //deltaMonthlyPAX.Volume = Convert.ToInt32(aDeltaHourlyProjData[4]);
                    deltaMonthlyPAX.LoadFactorPercentage = float.Parse(aDeltaMonthlyProjData[6].Replace("%", "").Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    _PAXVolume.NumFlights = Convert.ToInt32(aDeltaMonthlyProjData[7]);
                    //deltaMonthlyPAX.FlightCount = Convert.ToInt32(aDeltaHourlyProjData[7]);
                    deltaMonthlyPAX.ExtraInfo = "cap:" + aDeltaMonthlyProjData[5] + "|";
                    deltaMonthlyPAX.LastUpdate = dtLastUpdate;  
                    
                }
                else
                {
                    dtPAXDate = DateTime.Parse(aDeltaMonthlyProjData[2]).Date;
                    if (aDeltaMonthlyProjData.Length > 12)
                    {
                        dtLastUpdate = Convert.ToDateTime(aDeltaMonthlyProjData[15]);
                    }
                    else
                    {
                        dtLastUpdate = Convert.ToDateTime(aDeltaMonthlyProjData[11]);
                        //Since new tables do not provide year, check if last_update is 45 days greater than last update. This will determine which rows need to remove one year
                        if (dtPAXDate > dtLastUpdate.AddDays(45))
                        {
                            dtPAXDate.AddYears(-1);
                        }
                    }

                    deltaMonthlyPAX.PAXTypeID = Convert.ToInt32(aDeltaMonthlyProjData[0]);
                    deltaMonthlyPAX.FlightType = iFlightTypeID;
                    deltaMonthlyPAX.PAXDate = dtPAXDate;
                    //deltaMonthlyPAX.Volume = Convert.ToInt32(aDeltaHourlyProjData[11]);
                    _PAXVolume.Volume = Convert.ToInt32(aDeltaMonthlyProjData[7]);
                    deltaMonthlyPAX.LoadFactorPercentage = float.Parse(aDeltaMonthlyProjData[9].Replace("%", "").Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    _PAXVolume.NumFlights = Convert.ToInt32(aDeltaMonthlyProjData[10]);
                    //deltaMonthlyPAX.FlightCount = Convert.ToInt32(aDeltaHourlyProjData[14]);
                    deltaMonthlyPAX.ExtraInfo = "cap:" + aDeltaMonthlyProjData[8] + "|" + "<=1200:" + aDeltaMonthlyProjData[4] + "1201-1700:" + aDeltaMonthlyProjData[5] + ">1700:" + aDeltaMonthlyProjData[6] + "|";
                    deltaMonthlyPAX.LastUpdate = dtLastUpdate;
                }

                lstPAXVolumes.Add(_PAXVolume);

                deltaMonthlyPAX.Volume.AddRange(lstPAXVolumes);

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row in HandleDeltaMonthlyProjections().");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }


            return deltaMonthlyPAX;
        }

        public static PAX HandleAADailyProjections (string[] aAADailyProjData)
        {
            PAX aADailyPAX = new PAX();
            PAXVolume _PAXVolume = new PAXVolume();
            List<PAXVolume> lstPAXVolumes = new List<PAXVolume>();
            //PAXType,FlightType,Hour,
            // 3,0,01/08/2020,23:00 - 23:59,8,1158,674,58.20%,1/20/2020 7:59:27 PM
            try
            {

                aADailyPAX.PAXTypeID = Convert.ToInt32(aAADailyProjData[0]);
                aADailyPAX.FlightType = Convert.ToInt32(aAADailyProjData[1]);
                aADailyPAX.PAXDate = DateTime.Parse(aAADailyProjData[2]).Date;

                _PAXVolume.PAXHourTimestamp = DateTime.Parse(aAADailyProjData[2] + " " + aAADailyProjData[3].Substring(0, aAADailyProjData[3].IndexOf(" - ")));
                _PAXVolume.NumFlights = Convert.ToInt32(aAADailyProjData[4]);
                _PAXVolume.Volume = Convert.ToInt32(aAADailyProjData[6]);
                aADailyPAX.LoadFactorPercentage = float.Parse(aAADailyProjData[7].Replace("%", "").Trim(), System.Globalization.CultureInfo.InvariantCulture);
                //Local customers column is occasionally included in email. Check if exists by length of column and add it to extra info
                if (aAADailyProjData.Length == 10)
                {
                    aADailyPAX.ExtraInfo = "available seats:" + aAADailyProjData[5] + "|local customers:" + aAADailyProjData[8];
                    aADailyPAX.LastUpdate = Convert.ToDateTime(aAADailyProjData[9]);
                }
                else
                {
                    aADailyPAX.ExtraInfo = "available seats:" + aAADailyProjData[5];
                    aADailyPAX.LastUpdate = Convert.ToDateTime(aAADailyProjData[8]);
                }

                lstPAXVolumes.Add(_PAXVolume);
                aADailyPAX.Volume.AddRange(lstPAXVolumes);

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row in HandleAADailyProjections().");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }


            return aADailyPAX;
        }

        public static PAX HandleTSAEnplanements(string[] aTSAEnplanementsData)
        {
            PAX deltaHourlyPAX = new PAX();
            PAXVolume _PAXVolume = new PAXVolume();
            List<PAXVolume> lstPAXVolumes = new List<PAXVolume>();
            //PAXType,FlightType,Hour,Volume,LastUpdate
            //"4,3,1/7/2020 12:00:00 AM,36871,1/17/2020 5:09:18 PM"
            try
            {
                deltaHourlyPAX.PAXTypeID = Convert.ToInt32(aTSAEnplanementsData[0]);
                deltaHourlyPAX.FlightType = Convert.ToInt32(aTSAEnplanementsData[1]);
                deltaHourlyPAX.PAXDate = Convert.ToDateTime(aTSAEnplanementsData[2]).Date;
                //deltaHourlyPAX.Volume = Convert.ToInt32(aTSAEnplanementsData[3]);
                _PAXVolume.Volume = Convert.ToInt32(aTSAEnplanementsData[3]);
                deltaHourlyPAX.LastUpdate = Convert.ToDateTime(aTSAEnplanementsData[4]);

                lstPAXVolumes.Add(_PAXVolume);
                deltaHourlyPAX.Volume.AddRange(lstPAXVolumes);
            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row in HandleTSAEnplanements().");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return deltaHourlyPAX;
        }

        public static Tuple<PAX, PAX> HandleJetBlueDailyProjections (string[] aJetBlueDailyProjData)
        {
            PAX _ArrivalPAX = new PAX();
            PAX _DeparturePAX = new PAX();
            PAXVolume _PAXArrivalVolume = new PAXVolume();
            PAXVolume _PAXDepartureVolume = new PAXVolume();
            List<PAXVolume> lstPAXArrivalVolumes = new List<PAXVolume>();
            List<PAXVolume> lstPAXDepartureVolumes = new List<PAXVolume>();
            DateTime dtPAXDate = new DateTime();
            //PAXTypeName,PAXDate,FLT#,Org,STA,PAX,Gate,A/C,FLT#,Dest,STD,PAX,LastUpdate
            //8,25FEB,2198,MCO,15:34,134,3,583,399,MCO,16:29,157,2/24/2021 9:44:51 AM
            try
            {

                //if Arrival PAX is empty, skip. This may occur when it is a departure flight row that arrived the night before
                if (aJetBlueDailyProjData[5].Length == 0)
                {

                }
                else
                {
                    _ArrivalPAX.PAXTypeID = Convert.ToInt32(aJetBlueDailyProjData[0]);
                    _ArrivalPAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[_ArrivalPAX.PAXTypeID];
                    _ArrivalPAX.AirportID = PAX_Consumer_Globals.iAirportID;
                    _ArrivalPAX.FlightType = PAX_Consumer_Globals.dictFlightType[PAX_Consumer_Globals.sArrivalsTypeName];
                    dtPAXDate = DateTime.Parse(aJetBlueDailyProjData[1]).Date;
                    _ArrivalPAX.PAXDate = dtPAXDate;
                    _PAXArrivalVolume.PAXHourTimestamp = DateTime.Parse(dtPAXDate.ToString("MM/dd/yy") + " " + aJetBlueDailyProjData[4]);
                    _PAXArrivalVolume.NumFlights = 1;
                    _PAXArrivalVolume.Volume = Convert.ToInt32(aJetBlueDailyProjData[5]);
                    _ArrivalPAX.ExtraInfo = "flight_number:" + aJetBlueDailyProjData[2] + "|origin_airport_code:" + aJetBlueDailyProjData[3] + "|aircraft_number:" + aJetBlueDailyProjData[7] + "|gate:" + aJetBlueDailyProjData[4];
                    _ArrivalPAX.LastUpdate = DateTime.Parse(aJetBlueDailyProjData[12]);
                    lstPAXArrivalVolumes.Add(_PAXArrivalVolume);
                    _ArrivalPAX.Volume.AddRange(lstPAXArrivalVolumes);
                }

                //Same logic for Departure PAX
                if (aJetBlueDailyProjData[11].Length == 0)
                {
                    
                }
                else
                {
                    _DeparturePAX.PAXTypeID = Convert.ToInt32(aJetBlueDailyProjData[0]);
                    _DeparturePAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[_DeparturePAX.PAXTypeID];
                    _DeparturePAX.AirportID = PAX_Consumer_Globals.iAirportID;
                    _DeparturePAX.FlightType = PAX_Consumer_Globals.dictFlightType[PAX_Consumer_Globals.sDeparturesTypeName];
                    dtPAXDate = DateTime.Parse(aJetBlueDailyProjData[1]).Date;
                    _DeparturePAX.PAXDate = dtPAXDate;
                    _PAXDepartureVolume.PAXHourTimestamp = DateTime.Parse(dtPAXDate.ToString("MM/dd/yy") + " " + aJetBlueDailyProjData[10]);
                    _PAXDepartureVolume.NumFlights = 1;
                    _PAXDepartureVolume.Volume = Convert.ToInt32(aJetBlueDailyProjData[11]);
                    _DeparturePAX.ExtraInfo = "flight_number:" + aJetBlueDailyProjData[8] + "|origin_airport_code:" + aJetBlueDailyProjData[9] + "|aircraft_number:" + aJetBlueDailyProjData[7];
                    _DeparturePAX.LastUpdate = DateTime.Parse(aJetBlueDailyProjData[12]);
                    lstPAXDepartureVolumes.Add(_PAXDepartureVolume);
                    _DeparturePAX.Volume.AddRange(lstPAXDepartureVolumes);
                }
                

                

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row in HandleJetBlueDailyProjections().");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            Tuple<PAX, PAX> tPAX = Tuple.Create(_ArrivalPAX, _DeparturePAX);

            return tPAX;
        }

        public static PAX HandleDeltaSixDayDetailedProjections(string[] aDeltaSixDayDetailedProjData)
        {
            PAX _PAX = new PAX();
            PAXVolume _PAXVolume = new PAXVolume();
            List<PAXVolume> lstPAXVolumes = new List<PAXVolume>();
            //PAXType,FlightType,Date,AirlineCode,FlightNumber,PAX,ScheduleTime,AircraftType,Capacity,LF,last_update
            // 7,0,03/02/21,DL,2334,17,11:58,221,109,15.6,2/24/2021 11:34:27 AM
            try
            {

                _PAX.PAXTypeID = Convert.ToInt32(aDeltaSixDayDetailedProjData[0]);
                _PAX.AirlineID = PAX_Consumer_Globals.dictPAXTypeAirline[_PAX.PAXTypeID];
                _PAX.AirportID = PAX_Consumer_Globals.iAirportID;
                _PAX.FlightType = Convert.ToInt32(aDeltaSixDayDetailedProjData[1]);
                _PAX.PAXDate = DateTime.Parse(aDeltaSixDayDetailedProjData[2]).Date;
                _PAXVolume.PAXHourTimestamp = DateTime.Parse(_PAX.PAXDate.ToString("MM/dd/yy") + " " + aDeltaSixDayDetailedProjData[6]);
                //Each row is single flight, so this is one
                _PAXVolume.NumFlights = 1;
                _PAXVolume.Volume = Convert.ToInt32(aDeltaSixDayDetailedProjData[5]);
                //_PAX.LoadFactorPercentage = float.Parse(aDeltaSixDayDetailedProjData[9].Replace("%", "").Trim(), System.Globalization.CultureInfo.InvariantCulture);
                _PAX.LoadFactorPercentage = float.Parse(aDeltaSixDayDetailedProjData[9]);
                _PAX.ExtraInfo = "flight_number:" + aDeltaSixDayDetailedProjData[3] + aDeltaSixDayDetailedProjData[4] + "|capacity:" + aDeltaSixDayDetailedProjData[8] + "|aircraft_type:" + aDeltaSixDayDetailedProjData[7];
                _PAX.LastUpdate = Convert.ToDateTime(aDeltaSixDayDetailedProjData[10]);

                lstPAXVolumes.Add(_PAXVolume);
                _PAX.Volume.AddRange(lstPAXVolumes);

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row in HandleDeltaSixDayDetailedProjections().");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }


            return _PAX;
        }

        public static List<PAX> HandleJSONPax(JToken jCBPToken)
        {
            List<PAX> lstEnplanements = new List<PAX>();
            PAX _PAX = new PAX();
            JArray jPAXArray = null;
            int iAirportID = 0;
            DateTime dtLastUpdate = new DateTime();
            JArray jPAXVolArray = null;
            JArray jPAXWaitTimeArray = null;
            int iPAXClassification = 0;
            PAXVolume _PAXVolume = new PAXVolume();
            List<PAXVolume> lstPAXVolumes = new List<PAXVolume>();
            string sAirlineCode = "";

            try
            {
                iPAXClassification = Convert.ToInt32(jCBPToken.SelectToken("PAXClassification"));
                iAirportID = Convert.ToInt32(jCBPToken.SelectToken("AirportID"));
                dtLastUpdate = Convert.ToDateTime(jCBPToken.SelectToken("ReceiveDate"));
                sAirlineCode = (string)jCBPToken.SelectToken("AirlineCode");

                jPAXArray = (JArray)jCBPToken.SelectToken("PAX");


                foreach (JToken jPAXToken in jPAXArray)
                {
                    _PAX = new PAX();
                    //_PAXWaitTime = new PAXWaitTimes();
                    _PAX.PAXClassification = iPAXClassification;
                    _PAX.AirportID = iAirportID;

                    if (PAX_Consumer_Globals.dictAirlineIDLookup.ContainsKey(sAirlineCode))
                    {
                        _PAX.AirlineID = Convert.ToInt32(PAX_Consumer_Globals.dictAirlineIDLookup[sAirlineCode]);
                    }

                    _PAX.PAXTypeID = iPAXClassification;
                    _PAX.FlightType = Convert.ToInt32(jPAXToken.SelectToken("FlightType"));
                    _PAX.PAXDate = Convert.ToDateTime(jPAXToken.SelectToken("PAXDate"));

                    jPAXVolArray = (JArray)jPAXToken.SelectToken("PAXVolume");

                    foreach (JToken JPAXVolToken in jPAXVolArray)
                    {
                        _PAXVolume = new PAXVolume();

                        _PAXVolume.Terminal = (string)JPAXVolToken.SelectToken("Terminal");
                        _PAXVolume.Volume = Convert.ToInt32(JPAXVolToken.SelectToken("Volume"));
                        _PAXVolume.NumFlights = Convert.ToInt32(JPAXVolToken.SelectToken("NumFlights"));
                        _PAXVolume.NumBooths = Convert.ToInt32(JPAXVolToken.SelectToken("NumBooths"));

                        if (Convert.ToInt16(JPAXVolToken.SelectToken("Hour")) >= 0){
                            _PAXVolume.PAXHourTimestamp = _PAX.PAXDate.AddHours(Convert.ToInt16(JPAXVolToken.SelectToken("Hour")));
                        }

                        _PAXVolume.AvgWaitTime = Convert.ToInt32(JPAXVolToken.SelectToken("AvgWaitTime"));
                        _PAXVolume.MaxWaitTime = Convert.ToInt32(JPAXVolToken.SelectToken("MaxWaitTime"));
                        _PAXVolume.AvgUSWaitTime = Convert.ToInt32(JPAXVolToken.SelectToken("AvgUSWaitTime"));
                        _PAXVolume.MaxUSWaitTime = Convert.ToInt32(JPAXVolToken.SelectToken("MaxUSWaitTime"));
                        _PAXVolume.AvgNonUSWaitTime = Convert.ToInt32(JPAXVolToken.SelectToken("AvgNonUSWaitTime"));
                        _PAXVolume.MaxNonUSWaitTime = Convert.ToInt32(JPAXVolToken.SelectToken("MaxNonUSWaitTime"));

                        lstPAXVolumes.Add(_PAXVolume);

                    }

                    //jPAXWaitTimeArray = (JArray)jPAXToken.SelectToken("PAXWaitTime");
                    //if (jPAXWaitTimeArray.HasValues)
                    //{
                    //    foreach (JToken jPAXWaitTimeToken in jPAXWaitTimeArray)
                    //    {
                    //        lstPAXWaitTimes = new List<PAXWaitTimes>();

                    //        _PAXWaitTime.AvgWaitTime = Convert.ToInt32(jPAXWaitTimeToken.SelectToken("AvgWaitTime"));
                    //        _PAXWaitTime.MaxWaitTime = Convert.ToInt32(jPAXWaitTimeToken.SelectToken("MaxWaitTime"));
                    //        _PAXWaitTime.AvgUSWaitTime = Convert.ToInt32(jPAXWaitTimeToken.SelectToken("AvgUSWaitTime"));
                    //        _PAXWaitTime.MaxUSWaitTime = Convert.ToInt32(jPAXWaitTimeToken.SelectToken("MaxUSWaitTime"));
                    //        _PAXWaitTime.AvgNonUSWaitTime = Convert.ToInt32(jPAXWaitTimeToken.SelectToken("AvgNonUSWaitTime"));
                    //        _PAXWaitTime.MaxNonUSWaitTime = Convert.ToInt32(jPAXWaitTimeToken.SelectToken("MaxNonUSWaitTime"));

                    //        lstPAXWaitTimes.Add(_PAXWaitTime);
                    //    }
                    //}

                    
                    _PAX.LastUpdate = dtLastUpdate;
                    _PAX.Volume.AddRange(lstPAXVolumes);
                    //_PAX.PAXWaitTime.AddRange(lstPAXWaitTimes);
                    lstPAXVolumes.Clear();
                    lstEnplanements.Add(_PAX);
                }

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error processing data row in HandleCBPVolumes().");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return lstEnplanements;
        }
    }


}
