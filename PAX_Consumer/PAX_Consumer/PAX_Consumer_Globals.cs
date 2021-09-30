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

namespace PAX_Consumer
{
    class PAX_Consumer_Globals
    {

        //generic logging
        public static LoggingClass LogFile = new LoggingClass();

        //generic globals
        public static string sAppName = "";
        public static bool bSimulationEnabled = false;
        public static string sSimulationType = "";
        public static string[] aSimulationFiles;
        public static string sSimulationPath = "";
        public static int iAppCheckTimeoutInSeconds = 120;
        public const int iSecondsPerHour = 3600;
        public static int iAgencyID = 0;
        public static bool bSendToMongoDB = false;
        public static int iAirportID = 0;


        //DB Parameters
        public static int iDBCommandTimeoutInSeconds = 30;
        public static string sMongo_DBConn = "";
        public static string sPG_MOD_DBConn = "";

        //Kafka Parameters
        public static string sKafkaBrokerList = "";
        public static string sKafkaTopic = "";
        public static string sKafkaConsumerGroup = "";
        public static int iSecurityProtocol = 0;
        public static int iSaslMechanism = 0;
        public static string sSaslUsername = "";
        public static string sSaslPassword = "";
        public static int iKafkaConsumerConsumeTimeoutInMillieconds = 0;

        //PAX Consumer globals
        public static string sAgencyName = "";
        public static string sAirportCode = "";
        public static int iAADailyProjectionsTypeID = 0;
        public static int iDeltaDailyProjectionsTypeID = 0;
        public static int iDeltaHourlyProjectionsTypeID = 0;
        public static int iDeltaMonthlyProjectionsTypeID = 0;
        public static int iTSAEnplanementsTypeID = 0;
        public static int iCBPPAXTypeID = 0;
        public static int iTerminalBProjectionsID = 0;
        public static int iDeltaSixDayDetailedProjectionsID = 0;
        public static int iJetBlueDailyProjectionsID = 0;
        public static string sDeltaHourlyProjectionTypeName = "";
        public static string sDeltaMonthlyProjectionTypeName = "";
        public static string sAAHourlyProjectionTypeName = "";
        public static string sTSAEnplanementsTypeName = "";
        public static string sCBPAXTypeName = "";
        public static string sArrivalsTypeName = "";
        public static string sDeparturesTypeName = "";
        public static int iTSALevelOneMax = 0;
        public static int iTSALevelTwoMax = 0;
        public static int iTSALevelThreeMax = 0;
        public static string sTerminalBProjectionsTypeName = "";
        public static string sDeltaSixDayDetailedProjectionsTypeName = "";
        public static string sJetBlueDailyProjectionsTypeName = "";

        //app hash tables and dictionaries
        public static Dictionary<int, string> dictAgencyInfo = new Dictionary<int, string>();
        public static Dictionary<int, string> dictAirportInfo = new Dictionary<int, string>();
        public static Dictionary<string, int> dictPAXType = new Dictionary<string, int>();
        public static Dictionary<int, int> dictPAXTypeAirline = new Dictionary<int, int>();
        public static Dictionary<string, int> dictFlightType = new Dictionary<string, int>();
        public static Dictionary<string, int> dictAirlineIDLookup = new Dictionary<string, int>();

        //SNMP parameters
        public static string SNMPHost = "127.0.0.1";
        public static string SNMPPort = "162";
        public static string SNMPCommunity = "public";
        public static string SNMPOID = "1.3.6.1.4.1.1629.1.1.4";

        //app timers
        public static Timer tmGetData = new Timer();
        public static Timer tmAppCheck = new Timer();
        public static Timer tmKeepAlive = new Timer();
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
            sSimulationPath = ConfigurationManager.AppSettings["SimulationFile"];
            bSendToMongoDB = Convert.ToBoolean(ConfigurationManager.AppSettings["SendToMongoDB"]);

            //Kafka Parameters
            sKafkaTopic = ConfigurationManager.AppSettings["KafkaTopic"];
            sKafkaConsumerGroup = ConfigurationManager.AppSettings["KafkaConsumerGroup"];
            iSecurityProtocol = Convert.ToInt32(ConfigurationManager.AppSettings["SecurityProtocol"]);
            iSaslMechanism = Convert.ToInt32(ConfigurationManager.AppSettings["SaslMechanism"]);
            sSaslUsername = ConfigurationManager.AppSettings["SaslUsername"];
            sSaslPassword = ConfigurationManager.AppSettings["SaslPassword"];
            iKafkaConsumerConsumeTimeoutInMillieconds = Convert.ToInt32(ConfigurationManager.AppSettings["KafkaConsumerConsumeTimeoutInMillieconds"]);

            //PAX Consumer Settings
            sAgencyName = ConfigurationManager.AppSettings["AgencyName"];
            sAirportCode = ConfigurationManager.AppSettings["AirportCode"];
            sDeltaHourlyProjectionTypeName = ConfigurationManager.AppSettings["Delta_Hourly_Projections_TypeName"];
            sDeltaMonthlyProjectionTypeName = ConfigurationManager.AppSettings["Delta_Monthly_Projections_TypeName"];
            sAAHourlyProjectionTypeName = ConfigurationManager.AppSettings["American_Airlines_Hourly_Projections_TypeName"];
            sTSAEnplanementsTypeName = ConfigurationManager.AppSettings["TSA_Enplanements_TypeName"];
            sArrivalsTypeName = ConfigurationManager.AppSettings["Arrivals_TypeName"];
            sDeparturesTypeName = ConfigurationManager.AppSettings["Departures_TypeName"];
            sCBPAXTypeName = ConfigurationManager.AppSettings["CBP_PAX_TypeName"];
            iTSALevelOneMax = Convert.ToInt32(ConfigurationManager.AppSettings["TSALevelOneMax"]);
            iTSALevelTwoMax = Convert.ToInt32(ConfigurationManager.AppSettings["TSALevelTwoMax"]);
            iTSALevelThreeMax = Convert.ToInt32(ConfigurationManager.AppSettings["TSALevelThreeMax"]);
            sTerminalBProjectionsTypeName = ConfigurationManager.AppSettings["Terminal_B_Projections_TypeName"];
            sDeltaSixDayDetailedProjectionsTypeName = ConfigurationManager.AppSettings["Delta_Six_Day_Detailed_Projections_TypeName"];
            sJetBlueDailyProjectionsTypeName = ConfigurationManager.AppSettings["JetBlue_Daily_Projections_TypeName"];

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
                LogFile.LogErrorText("Error in PAX_Consumer_Globals.GetXMLFromSimulationFile()");
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
                LogFile.LogErrorText("Error in PAX_Consumer_Globals.GetJsonFromSimulationFile()");
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
            }
            catch (Exception ex)
            {
                LogFile.LogErrorText("Error in PAX_Consumer_Globals.DoGZIP()");
                LogFile.LogErrorText(ex.ToString());
                LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static int SetAgencyID(string sAgencyName)
        {
            int iAgencyID = 0;

            if (PAX_Consumer_Globals.dictAgencyInfo.ContainsValue(sAgencyName))
            {
                foreach (KeyValuePair<int, string> kvpAgency in PAX_Consumer_Globals.dictAgencyInfo)
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
        public static int SetAirportID(string sAirportCode)
        {
            int iAirportID = 0;

            if (PAX_Consumer_Globals.dictAirportInfo.ContainsValue(sAirportCode))
            {
                foreach (KeyValuePair<int, string> kvpAirport in PAX_Consumer_Globals.dictAirportInfo)
                {
                    if (kvpAirport.Value.ToString() == sAirportCode)
                    {
                        iAirportID = Convert.ToInt32(kvpAirport.Key.ToString());
                        break;
                    }
                }
            }

            return iAirportID;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

    }
}
