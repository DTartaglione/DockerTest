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

namespace EventHubs_SampleProducer
{
    class EventHubs_SampleProducer_Globals
    {

        //generic logging
        public static LoggingClass LogFile = new LoggingClass();

        //generic globals
        public static string sAppName = "";
        public static string sAgencyName = "";
        public static bool bSimulationEnabled = false;
         public static string sSimulationFile;
        public static int iAppCheckTimeoutInSeconds = 120;
        public const int iSecondsPerHour = 3600;
        public static int iAgencyID = 0;
        public static bool bSendToKafka = false;
        public static int iDataRefreshRateInSeconds = 180;

        //DB Parameters
        public static int iDBCommandTimeoutInSeconds = 30;
        public static string sMODDBConn = "";
        public static string sPG_MOD_DBConn = "";

        //Kafka Parameters
        public static string sKafkaBrokerList = "";
        public static string sKafkaTopic = "";
        public static int iSecurityProtocol = 0;
        public static int iSaslMechanism = 0;
        public static string sSaslUsername = "";
        public static string sSaslPassword = "";

        //app hash tables and dictionaries
        public static Dictionary<int, string> dictAgencyInfo = new Dictionary<int, string>();
        
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
            sMODDBConn = ConfigurationManager.ConnectionStrings["MOD_DB"].ConnectionString;
            sPG_MOD_DBConn = ConfigurationManager.ConnectionStrings["PG_MOD_DB"].ConnectionString;
            sKafkaBrokerList = ConfigurationManager.ConnectionStrings["KafkaBrokerList"].ConnectionString;
        }

        public static void ReadConfigParams()
        {

            LogFile.Log("Begin reading configuration parameters.");

            //general settings
            sAppName = ConfigurationManager.AppSettings["AppName"];
            sAgencyName = ConfigurationManager.AppSettings["AgencyName"];
            iDBCommandTimeoutInSeconds = Convert.ToInt16(ConfigurationManager.AppSettings["DBCommandTimeoutInSeconds"]) * 1000;
            iAppCheckTimeoutInSeconds = Convert.ToInt16(ConfigurationManager.AppSettings["AppCheckTimeoutInSeconds"]) * 1000;
            bSimulationEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["SimulationEnabled"]);
            sSimulationFile = ConfigurationManager.AppSettings["SimulationFile"];
            iDataRefreshRateInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["DataRefreshRateInSeconds"]) * 1000;

            //Kafka Parameters
            bSendToKafka = Convert.ToBoolean(ConfigurationManager.AppSettings["SendToKafka"].ToString());
            sKafkaTopic = ConfigurationManager.AppSettings["KafkaTopic"];
            iSecurityProtocol = Convert.ToInt32(ConfigurationManager.AppSettings["SecurityProtocol"]);
            iSaslMechanism = Convert.ToInt32(ConfigurationManager.AppSettings["SaslMechanism"]);
            sSaslUsername = ConfigurationManager.AppSettings["SaslUsername"];
            sSaslPassword = ConfigurationManager.AppSettings["SaslPassword"];

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
                LogFile.LogErrorText("Error in EventHubs_SampleProducer_Globals.GetXMLFromSimulationFile()");
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
                LogFile.LogErrorText("Error in EventHubs_SampleProducer_Globals.GetJsonFromSimulationFile()");
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
                LogFile.LogErrorText("Error in EventHubs_SampleProducer_Globals.DoGZIP()");
                LogFile.LogErrorText(ex.ToString());
                LogFile.LogError(ex);
                bSuccess = false;
            }

            return bSuccess;
        }

        public static int SetAgencyID(string sAgencyName)
        {
            int iAgencyID = 0;

            if (EventHubs_SampleProducer_Globals.dictAgencyInfo.ContainsValue(sAgencyName))
            {
                foreach (KeyValuePair<int, string> kvpAgency in EventHubs_SampleProducer_Globals.dictAgencyInfo)
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

        public static DateTime FromUnixTime(long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

    }
}
