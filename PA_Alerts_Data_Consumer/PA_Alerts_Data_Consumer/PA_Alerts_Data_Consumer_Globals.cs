using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Timers;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Collections;

namespace PA_Alerts_Data_Consumer
{
    class PA_Alerts_Data_Consumer_Globals
    {

        //generic logging
        public static LoggingClass LogFile = new LoggingClass();

        //generic globals
        public static string sAppName = "";
        public static bool bSimulationEnabled = false;
        public static string sSimulationType = "";
        public static string[] aSimulationFiles;
        public static int iAppCheckTimeoutInSeconds = 120;
        public const int iSecondsPerHour = 3600;
        public static int iAgencyID = 0;
        public static int iAOCID = 0;

        //DB Parameters
        public static int iDBCommandTimeoutInSeconds = 30;
        public static string sMongo_DBConn = "";
        public static string sPG_MOD_DBConn = "";
        public static bool bSendToMongoDB = false;

        //Kafka globals
        public static string sKafkaBrokerList = "";
        public static string sKafkaTopic = "";
        public static string sKafkaConsumerGroup = "";
        public static int iSecurityProtocol = 0;
        public static int iSaslMechanism = 0;
        public static string sSaslUsername = "";
        public static string sSaslPassword = "";
        public static int iKafkaConsumerConsumeTimeoutInMillieconds = 0;

        //PA Alerts Data Producer globals
        public static string sAgencyName = "";
        public static string sAOCName = "";
        public static int iDisableAlertsOlderThanValueInHours = 24;
        public static int iAlertDurationCheckInSeconds = 3600;

        //PA Alerts Parsing Settings
        //AOC Status Code
        //public static bool bAOCStatusCodeCheckBySenderEmail = false;
        //public static string sAOCStatusCodeSenderEmail = "";
        //public static bool bAOCStatusCodeCheckBySubject = false;
        //public static string sAOCStatusCodeSubjectSearchString = "";
        //public static bool bAOCStatusCodeCheckByBody = false;
        //public static string sAOCStatusCodeBodySearchString = "";
        public static string sLGAAOCStatusUpdateTypeName = "";
        public static bool bLGAAOCStatusUpdateAddMitigations = false;
        public static string sLGAAOCStatusUpdateMitigationsTimeFormat = "";
        public static string sLGAAOCStatusUpdateMitigationsSectionStartText = "";
        public static string sLGAAOCStatusUpdateMitigationsEndText = "";
        public static string sLGAAOCStatusUpdateMitigationsOngoingText = "";
        //PA Alerts
        //public static bool bAlertsCheckBySenderEmail = false;
        //public static string sAlertsSenderEmail = "";
        //public static bool bAlertsCheckBySubject = false;
        //public static string sAlertsSubjectSearchString = "";
        //public static bool bAlertsCheckByBody = false;
        //public static string sAlertsBodySearchString = "";
        public static List<string> lstPAAlertsTypeName = new List<string>();

        //app hash tables and dictionaries
        public static Dictionary<int, string> dictAgencyInfo = new Dictionary<int, string>();
        public static Dictionary<int, string> dictAOCInfo = new Dictionary<int, string>();
        public static Dictionary<int, DateTime> dictActiveAlerts = new Dictionary<int, DateTime>();
        public static Dictionary<string, DateTime> dictLatestPAXUpdate = new Dictionary<string, DateTime>();
        public static Dictionary<string, int> dictEmailType = new Dictionary<string, int>();

        //SNMP parameters
        public static string SNMPHost = "127.0.0.1";
        public static string SNMPPort = "162";
        public static string SNMPCommunity = "public";
        public static string SNMPOID = "1.3.6.1.4.1.1629.1.1.4";

        //app timers
        public static Timer tmGetData = new Timer();
        public static Timer tmAppCheck = new Timer();
        public static Timer tmKeepAlive = new Timer();
        public static Timer tmAlertDurationCheck = new Timer();
        public static DateTime dtLastKeepAlive = DateTime.Now;

        public static void GetSQLConnectionStrings()
        {
            sMongo_DBConn = ConfigurationManager.ConnectionStrings["MongoDB_MOD_DB"].ConnectionString;
            sPG_MOD_DBConn = ConfigurationManager.ConnectionStrings["PG_MOD_DB"].ConnectionString;
            sKafkaBrokerList = ConfigurationManager.ConnectionStrings["KafkaBrokerList"].ConnectionString;
        }

        public static void ReadConfigParams()
        {

            LogFile.Log("Begin reading configuration parameters.");

            //general settings
            iDBCommandTimeoutInSeconds = Convert.ToInt16(ConfigurationManager.AppSettings["DBCommandTimeoutInSeconds"]) * 1000;
            iAppCheckTimeoutInSeconds = Convert.ToInt16(ConfigurationManager.AppSettings["AppCheckTimeoutInSeconds"]) * 1000;
            bSimulationEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["SimulationEnabled"]);
            sSimulationType = ConfigurationManager.AppSettings["SimulationType"];
            aSimulationFiles = ConfigurationManager.AppSettings["SimulationFile"].Split(new char[] { ' ' }, StringSplitOptions.None);
            bSendToMongoDB = Convert.ToBoolean(ConfigurationManager.AppSettings["SendToMongoDB"]);

            //Kafka settings
            sKafkaTopic = ConfigurationManager.AppSettings["KafkaTopic"];
            sKafkaConsumerGroup = ConfigurationManager.AppSettings["KafkaConsumerGroup"];
            iSecurityProtocol = Convert.ToInt32(ConfigurationManager.AppSettings["SecurityProtocol"]);
            iSaslMechanism = Convert.ToInt32(ConfigurationManager.AppSettings["SaslMechanism"]);
            sSaslUsername = ConfigurationManager.AppSettings["SaslUsername"];
            sSaslPassword = ConfigurationManager.AppSettings["SaslPassword"];
            iKafkaConsumerConsumeTimeoutInMillieconds = Convert.ToInt32(ConfigurationManager.AppSettings["KafkaConsumerConsumeTimeoutInMillieconds"]);

            //PA Alerts Data Producer Settings
            sAgencyName = ConfigurationManager.AppSettings["AgencyName"];
            sAOCName = ConfigurationManager.AppSettings["AOCName"];
            iDisableAlertsOlderThanValueInHours = Convert.ToInt32(ConfigurationManager.AppSettings["DisableAlertsOlderThanValueInHours"]);
            iAlertDurationCheckInSeconds = Convert.ToInt16(ConfigurationManager.AppSettings["AlertDurationCheckInSeconds"]) * 1000;

            //PA Alerts Parsing Settings
            //AOC Status Code
            //bAOCStatusCodeCheckBySenderEmail = Convert.ToBoolean(ConfigurationManager.AppSettings["AOCStatusCodeCheckBySenderEmail"]);
            //sAOCStatusCodeSenderEmail = ConfigurationManager.AppSettings["AOCStatusCodeSenderEmail"];
            //bAOCStatusCodeCheckBySubject = Convert.ToBoolean(ConfigurationManager.AppSettings["AOCStatusCodeCheckBySubject"]);
            //sAOCStatusCodeSubjectSearchString = ConfigurationManager.AppSettings["AOCStatusCodeSubjectSearchString"];
            //bAOCStatusCodeCheckByBody = Convert.ToBoolean(ConfigurationManager.AppSettings["AOCStatusCodeCheckByBody"]);
            //sAOCStatusCodeBodySearchString = ConfigurationManager.AppSettings["AOCStatusCodeBodySearchString"];
            sLGAAOCStatusUpdateTypeName = ConfigurationManager.AppSettings["LGAAOC_Status_Update_TypeName"];
            bLGAAOCStatusUpdateAddMitigations = Convert.ToBoolean(ConfigurationManager.AppSettings["LGAAOC_Status_Update_AddMitigations"]);
            sLGAAOCStatusUpdateMitigationsTimeFormat = ConfigurationManager.AppSettings["LGAAOC_Status_Update_MitigationsTimeFormat"];
            sLGAAOCStatusUpdateMitigationsSectionStartText = ConfigurationManager.AppSettings["LGAAOC_Status_Update_MitigationsSectionStartText"];
            sLGAAOCStatusUpdateMitigationsEndText = ConfigurationManager.AppSettings["LGAAOC_Status_Update_MitigationsEndText"];
            sLGAAOCStatusUpdateMitigationsOngoingText = ConfigurationManager.AppSettings["LGAAOC_Status_Update_MitigationsOngoingText"];
            //PA Alerts
            //bAlertsCheckBySenderEmail = Convert.ToBoolean(ConfigurationManager.AppSettings["AlertsCheckBySenderEmail"]);
            //sAlertsSenderEmail = ConfigurationManager.AppSettings["AlertsSenderEmail"];
            //bAlertsCheckBySubject = Convert.ToBoolean(ConfigurationManager.AppSettings["AlertsCheckBySubject"]);
            //sAlertsSubjectSearchString = ConfigurationManager.AppSettings["AlertsSubjectSearchString"];
            //bAlertsCheckByBody = Convert.ToBoolean(ConfigurationManager.AppSettings["AlertsCheckByBody"]);
            //sAlertsBodySearchString = ConfigurationManager.AppSettings["AlertsBodySearchString"];
            lstPAAlertsTypeName = ConfigurationManager.AppSettings["PA_Alerts_TypeName"].Split(new char[] { ',' }, StringSplitOptions.None).ToList();

            //SNMP Params
            SNMPHost = ConfigurationManager.AppSettings["SNMPHost"];
            SNMPPort = ConfigurationManager.AppSettings["SNMPPort"];
            SNMPCommunity = ConfigurationManager.AppSettings["SNMPCommunity"];
            SNMPOID = ConfigurationManager.AppSettings["SNMPOID"];

            foreach (string sKey in ConfigurationManager.AppSettings.AllKeys)
            {
                LogFile.Log(sKey + "=" + ConfigurationManager.AppSettings[sKey]);
            }

            Console.WriteLine();
        }
        public static XmlDocument GetXMLFromSimulationFile(string sXmlDoc)
        {
            XmlDocument XmlDoc = new XmlDocument();
            try
            {
                XmlDoc.Load(sXmlDoc);
            }
            catch (Exception ex)
            {
                LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_Globals.GetXMLFromSimulationFile()");
                LogFile.LogErrorText(ex.ToString());
                LogFile.LogError(ex);
                XmlDoc = null;
            }

            return XmlDoc;
        }

        public static string GetJsonFromSimulationFile(string sJsonDoc)
        {
            string sJsonResp = "";

            try
            {
                using (StreamReader r = new StreamReader(sJsonDoc))
                {
                    sJsonResp = r.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_Globals.GetJsonFromSimulationFile()");
                LogFile.LogErrorText(ex.ToString());
                LogFile.LogError(ex);
                sJsonResp = "";
            }

            return sJsonResp;
        }


        public static void UnhandledExceptionHandler(Object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            LogFile.LogErrorText("Encountered unhandled exception. Error message is: " + ex.Message + "." +
                Environment.NewLine + "Exception type is: " + ex.GetType().ToString() + ex.StackTrace);
            LogFile.LogError(ex);
        }

        public static Boolean DoGZIP(string sOutFile)
        {
            bool bSuccess = false;
            DateTime dtNow = DateTime.Now;
            string sDateTime = "";

            try
            {
                sDateTime = dtNow.ToString("MMddyyyy_HHmmss");

                var bytes = File.ReadAllBytes(sOutFile);
                using (FileStream fs = new FileStream(sOutFile + "." + sDateTime + ".gz", FileMode.CreateNew))
                using (GZipStream zipStream = new GZipStream(fs, CompressionMode.Compress, false))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                }

                File.Delete(sOutFile);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_Globals.DoGZIP()");
                LogFile.LogErrorText(ex.ToString());
                LogFile.LogError(ex);
                bSuccess = false;
            }

            return bSuccess;
        }

        public static int SetAgencyID(string sAgencyName)
        {
            int iAgencyID = 0;

            if (PA_Alerts_Data_Consumer_Globals.dictAgencyInfo.ContainsValue(sAgencyName))
            {
                foreach (KeyValuePair<int, string> kvpAgency in PA_Alerts_Data_Consumer_Globals.dictAgencyInfo)
                {
                    if (kvpAgency.Value.ToString() == sAgencyName)
                    {
                        iAgencyID = Convert.ToInt32(kvpAgency.Key.ToString());
                        break;
                    }
                }
            }

            return iAgencyID;
        }

        public static int SetAOCID(string sAOCName)
        {
            int iAOCID = 0;

            if (PA_Alerts_Data_Consumer_Globals.dictAOCInfo.ContainsValue(sAOCName))
            {
                foreach (KeyValuePair<int, string> kvpAOC in PA_Alerts_Data_Consumer_Globals.dictAOCInfo)
                {
                    if (kvpAOC.Value.ToString() == sAOCName)
                    {
                        iAOCID = Convert.ToInt32(kvpAOC.Key.ToString());
                        break;
                    }
                }
            }

            return iAOCID;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

    }
}
