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

namespace PA_Alerts_Data_Consumer
{
    class PA_Alerts_Data_Consumer_HandleResponse
    {

        public static void HandleResponse(List<dynamic> lstKafkaData)
        {
            AOC_Traffic_Status _AOC_Traffic_Status = new AOC_Traffic_Status();
            List<AOC_Traffic_Status> lstAOCTrafficStatus= new List<AOC_Traffic_Status>();
            Alerts _Alerts = new Alerts();
            List<Alerts> lstAlerts = new List<Alerts>();
            JObject jObj = null;
            string sFrom = "";
            string sSubject = "";
            string sBody = "";
            int iEmailTypeID = 0;
            DateTime dtLastUpdate = new DateTime();
            int iAlertID = 0;
            string sAirlineCode = "";
            DateTime dtReceiveDate = new DateTime();
            string sPAXDate = "";
            string sPAXType = "";
            int iHour = 0;
            List<PAXUpdate> lstPAXUpdate = new List<PAXUpdate>();
            List<PAXUpdate> lstCombinedPAXUpdate = new List<PAXUpdate>();
            JToken jPAXToken = null;
            JToken jPAXVolumeToken = null;
            string sPAAlertsTypeName = "";

            try
                {
                
                foreach (dynamic dLstItem in lstKafkaData)
                {

                    jObj = JObject.Parse(dLstItem.ToString());

                    try
                    {

                        sFrom = jObj["From"].ToString();
                        sSubject = jObj["Subject"].ToString();
                        sBody = jObj["Body"].ToString();
                        iEmailTypeID = Convert.ToInt32(jObj["EmailType"].ToString());
                        dtLastUpdate = Convert.ToDateTime(jObj["LastUpdate"].ToString());

                        foreach (string sTypeName in PA_Alerts_Data_Consumer_Globals.lstPAAlertsTypeName)
                        {
                            if (PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsKey(sTypeName) && PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsValue(iEmailTypeID))
                            {
                                foreach (KeyValuePair<string, int> kvpEmailType in PA_Alerts_Data_Consumer_Globals.dictEmailType) {
                                    if (kvpEmailType.Value == iEmailTypeID && PA_Alerts_Data_Consumer_Globals.lstPAAlertsTypeName.Contains(kvpEmailType.Key))
                                    {
                                        sPAAlertsTypeName = kvpEmailType.Key.ToString();
                                        break;
                                    }
                                }
                            }
                        }

                        //Status code update
                        if (PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsKey(PA_Alerts_Data_Consumer_Globals.sLGAAOCStatusUpdateTypeName) ||
                            (PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsValue(iEmailTypeID) &&  sPAAlertsTypeName != ""))
                        {
                            if (PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsKey(PA_Alerts_Data_Consumer_Globals.sLGAAOCStatusUpdateTypeName))
                            {
                                if (iEmailTypeID == PA_Alerts_Data_Consumer_Globals.dictEmailType[PA_Alerts_Data_Consumer_Globals.sLGAAOCStatusUpdateTypeName])
                                {
                                    _AOC_Traffic_Status = new AOC_Traffic_Status();

                                    _AOC_Traffic_Status.CodeName = GetTrafficStatusCodeName(sSubject.ToUpper());
                                    _AOC_Traffic_Status.CodeID = GetTrafficStatusCodeID(_AOC_Traffic_Status.CodeName.ToUpper());
                                    _AOC_Traffic_Status.LastUpdate = dtLastUpdate;

                                    lstAOCTrafficStatus.Add(_AOC_Traffic_Status);

                                    continue;
                                }

                            }

                            if (PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsValue(iEmailTypeID))
                            {
                                if (PA_Alerts_Data_Consumer_Globals.dictEmailType.ContainsKey(sPAAlertsTypeName) && iEmailTypeID == PA_Alerts_Data_Consumer_Globals.dictEmailType[sPAAlertsTypeName])
                                {
                                    _Alerts = new Alerts();

                                    _Alerts.AlertMessage = sBody;
                                    _Alerts.EmailTypeID = iEmailTypeID;
                                    _Alerts.CreateTimestamp = dtLastUpdate;
                                    _Alerts.LastUpdate = dtLastUpdate;

                                    lstAlerts.Add(_Alerts);

                                    continue;
                                }
                            }
                        }
                        else
                        {
                            PA_Alerts_Data_Consumer_Globals.LogFile.Log("No matching email type for email sent from " +
                                sFrom + " at " + dtLastUpdate.ToString());
                        }

                    }
                    catch (Exception ex2)
                    {
                        PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_HandleResponse.HandleResponse()");
                        PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex2);
                    }
                }

                if (lstAOCTrafficStatus.Count > 0)
                {
                    foreach (AOC_Traffic_Status lstAOCTrafficStatusItem in lstAOCTrafficStatus)
                    {
                        try
                        {
                            PA_Alerts_Data_Consumer_ProcessData.UpdateAOCTrafficStatus(lstAOCTrafficStatusItem);
                        }
                        catch (Exception ex3)
                        {
                            PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error updating AOC status code in PA_Alerts_Data_Consumer_HandleResponse.HandleResponse()");
                            PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex3);
                        }
                    }
                        
                    PA_Alerts_Data_Consumer_Globals.LogFile.Log("Finished updating AOC status code in Postgres.");

                    //MongoDB code
                    if(PA_Alerts_Data_Consumer_Globals.bSendToMongoDB)
                    {
                        PA_Alerts_Data_Consumer_MongoCode.DoBatchUpdateAOCTrafficStatus(lstAOCTrafficStatus);
                    }
                }

                if (lstAlerts.Count > 0)
                {

                    foreach(Alerts lstAlertsItem in lstAlerts)
                    { 
                        try
                        {
                            iAlertID = PA_Alerts_Data_Consumer_ProcessData.InsertAlert(lstAlertsItem);
                            lstAlertsItem.AlertID = iAlertID;
                        }
                        catch (Exception ex4)
                        {
                            PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error updating PA alert in PA_Alerts_Data_Consumer_HandleResponse.HandleResponse()");
                            PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex4);
                        }
                    }

                    PA_Alerts_Data_Consumer_Globals.LogFile.Log("Finished updating PA alerts in Postgres.");

                    //MongoDB code
                    if (PA_Alerts_Data_Consumer_Globals.bSendToMongoDB)
                    {
                        PA_Alerts_Data_Consumer_MongoCode.DoBatchUpdateAlerts(lstAlerts);
                    }
                }
            }

            
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_HandleResponse()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }
            finally
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Finished processing new PA alert data");
            }
        }
        
        public static string GetTrafficStatusCodeName(string sSubject)
        {
            //"18 SEP 2019 CODE GREEN LGA - AOC Status Update (Trial Version)"
            string sCodeName = "";

            try
            {
                string sSubString = sSubject.Substring(sSubject.IndexOf(" CODE ")+6, 10);
                sCodeName = sSubString.Substring(0, sSubString.IndexOf(" "));
            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error getting Traffic Status Code with "+
                    "PA_Alerts_Data_Consumer_HandleResponse.GetTrafficStatusCodeName()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return sCodeName;
        }

        public static int GetTrafficStatusCodeID(string sCodeName)
        {
            int iCodeID = 0;
            if (sCodeName == "GREEN")
            {
                iCodeID = 1;
            }
            if (sCodeName == "YELLOW")
            {
                iCodeID = 2;
            }
            if (sCodeName == "AMBER")
            {
                iCodeID = 3;
            }
            if (sCodeName == "RED")
            {
                iCodeID = 4;
            }
            return iCodeID;
        }

        //public static DateTime GetHourTimestamp(int iHour, string sPAXDate)
        //{
        //    DateTime dtHourTimestamp = DateTime.Now;
        //    try
        //    {
        //        if (iHour.ToString().Length > 1)
        //        {
        //            //MM/dd/yyyy HH:mm:ss
        //            dtHourTimestamp = DateTime.ParseExact(sPAXDate + " " + iHour.ToString() + ":00:00", "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        //        }
        //        else
        //        {
        //            dtHourTimestamp = DateTime.ParseExact(sPAXDate + " 0" + iHour.ToString() + ":00:00", "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error attempting to convert hour to valid timestamp. Using current time in " +
        //            "PA_Alerts_Data_Consumer_HandleResponse.GetHourTimestamp()");
        //        PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
        //    }
        //    return dtHourTimestamp;
        //}

        //public static List<PAXUpdate> CombinePAXUpdates(List<PAXUpdate> lstPAXUpdate)
        //{
        //    List<PAXUpdate> lstCombinedPAXUpdate = new List<PAXUpdate>();

        //    foreach (PAXUpdate lstPAXUpdateItem in lstPAXUpdate)
        //    {
        //        //Skip over any depature objects since we will only use arrv to generate objects
        //        if (lstPAXUpdateItem.PAXType == "dept")
        //        {
        //            continue;
        //        }

        //        PAXUpdate _CombinedPAXUpdate = new PAXUpdate();
        //        _CombinedPAXUpdate.AirlineCode = lstPAXUpdateItem.AirlineCode;
        //        _CombinedPAXUpdate.HourTimestamp = lstPAXUpdateItem.HourTimestamp;
        //        _CombinedPAXUpdate.ArrvVolume = lstPAXUpdateItem.ArrvVolume;
        //        _CombinedPAXUpdate.LastUpdate = lstPAXUpdateItem.LastUpdate;

        //        try
        //        {
        //            var vRelatedPAX = lstPAXUpdate.Find(x => x.LastUpdate == lstPAXUpdateItem.LastUpdate && x.HourTimestamp == lstPAXUpdateItem.HourTimestamp && x.AirlineCode == lstPAXUpdateItem.AirlineCode);
        //            _CombinedPAXUpdate.DeptVolume = vRelatedPAX.DeptVolume;
        //        }
        //        catch (Exception ex)
        //        {
        //            PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error finding related dept volume for: " + lstPAXUpdateItem.AirlineCode + " at hour timestamp: " +
        //                lstPAXUpdateItem.HourTimestamp.ToString() + ". Please verify the source data. The tables should match.");
        //            PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
        //        }

        //        lstCombinedPAXUpdate.Add(_CombinedPAXUpdate);
        //    }

        //        return lstCombinedPAXUpdate;
        //}
    }
}
