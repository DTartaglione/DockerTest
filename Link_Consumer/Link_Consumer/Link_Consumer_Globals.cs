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

namespace Link_Consumer
{
    class Link_Consumer_Globals
    {

        //generic logging
        public static LoggingClass LogFile = new LoggingClass();

        //generic globals
        public static string sAppName = "";
        public static bool bSimulationEnabled = false;
        public static string sSimulationType = "";
        public static string[] aSimulationFiles;
        public static int iAppCheckTimeoutInSeconds = 120;
        public static int iCassandraBatchInsertSize = 50;
        public const int iSecondsPerHour = 3600;
        public static int iAgencyID = 0;
        public static bool bSendToCassandra = false;
        public static string sSubscribeToAgencyIDs = "";
        public static bool bSendToMongoDB = false;

        //DB Parameters
        public static int iDBCommandTimeoutInSeconds = 30;
        public static string sMODDBConn = "";
        public static string sPG_MOD_DBConn = "";
        public static string sCassandra_DBConn = "";
        public static string sKafkaBrokerList = "";
        public static string sKafkaLinkTopic = "";
        public static string sKafkaConsumerGroup = "";
        public static string sMongo_DBConn = "";

        //Kafka/Event Hubs parameters
        public static int iSecurityProtocol = 0;
        public static int iSaslMechanism = 0;
        public static string sSaslUsername = "";
        public static string sSaslPassword = "";
        public static int iKafkaConsumerConsumeTimeoutInMillieconds = 0;

        //Settings
        //public static string sLinkOutFile = "";
        //public static string sLinkDeleteListFile = "";
        public static string sCopyLinkBatFile = "";
        public static int iDefaultDirectionID = 0;
        public static string sLinkOutDirectory = "";
        public static int iLinkInsertStatusIndicator = 0;

        //app hash tables and dictionaries
        public static Dictionary<int, string> dictAgencyInfo = new Dictionary<int, string>();
        public static HashSet<string> hshLinksToArchive = new HashSet<string>();
        public static Dictionary<string, Link_Consumer_DataStructures.Link_Consumer_StoredLinkData> dictLinkIDAndListData = new Dictionary<string, Link_Consumer_DataStructures.Link_Consumer_StoredLinkData>();

        //SNMP parameters
        public static string SNMPHost = "127.0.0.1";
        public static string SNMPPort = "162";
        public static string SNMPCommunity = "public";
        public static string SNMPOID = "1.3.6.1.4.1.1629.1.1.4";

        //app timers
        public static Dictionary<string, Timer> dictTimers = new Dictionary<string, Timer>();
        public static Timer tmGetLinks = new Timer();
        public static Timer tmAppCheck = new Timer();
        public static Timer tmKeepAlive = new Timer();
        public static DateTime dtLastKeepAlive = DateTime.Now;

        public static void GetSQLConnectionStrings()
        {
            sMODDBConn = ConfigurationManager.ConnectionStrings["MOD_DB"].ConnectionString;
            sPG_MOD_DBConn = ConfigurationManager.ConnectionStrings["PG_MOD_DB"].ConnectionString;
            sCassandra_DBConn = ConfigurationManager.ConnectionStrings["Cassandra_Host"].ConnectionString;
            sKafkaBrokerList = ConfigurationManager.ConnectionStrings["KafkaBrokerList"].ConnectionString;
            sMongo_DBConn = ConfigurationManager.ConnectionStrings["MongoDB_MOD_DB"].ConnectionString;
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
            iCassandraBatchInsertSize = Convert.ToInt32(ConfigurationManager.AppSettings["CassandraBatchInsertSize"]);
            sKafkaLinkTopic = ConfigurationManager.AppSettings["KafkaLinkTopic"];
            bSendToCassandra = Convert.ToBoolean(ConfigurationManager.AppSettings["SendToCassandra"].ToString());
            sSubscribeToAgencyIDs = ConfigurationManager.AppSettings["SubscribeToAgencyIDs"];
            sKafkaConsumerGroup = ConfigurationManager.AppSettings["KafkaConsumerGroup"];
            bSendToMongoDB = Convert.ToBoolean(ConfigurationManager.AppSettings["SendToMongoDB"].ToString());

            //Kafka/Event Hubs
            iSecurityProtocol = Convert.ToInt32(ConfigurationManager.AppSettings["SecurityProtocol"]);
            iSaslMechanism = Convert.ToInt32(ConfigurationManager.AppSettings["SaslMechanism"]);
            sSaslUsername = ConfigurationManager.AppSettings["SaslUsername"];
            sSaslPassword = ConfigurationManager.AppSettings["SaslPassword"];
            iKafkaConsumerConsumeTimeoutInMillieconds = Convert.ToInt32(ConfigurationManager.AppSettings["KafkaConsumerConsumeTimeoutInMillieconds"]);

            //sLinkOutFile = ConfigurationManager.AppSettings["LinkOutFile"];
            //sLinkDeleteListFile = ConfigurationManager.AppSettings["LinkDeleteListFile"];
            sCopyLinkBatFile = ConfigurationManager.AppSettings["CopyLinkBatFile"];
            iDefaultDirectionID = Convert.ToInt16(ConfigurationManager.AppSettings["DefaultDirectionID"]);
            sLinkOutDirectory = ConfigurationManager.AppSettings["LinkOutDirectory"];
            iLinkInsertStatusIndicator = Convert.ToInt16(ConfigurationManager.AppSettings["LinkInsertStatusIndicator"]);

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
                LogFile.LogErrorText("Error in Link_Consumer_Globals.GetXMLFromSimulationFile()");
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
                LogFile.LogErrorText("Error in Link_Consumer_Globals.GetJsonFromSimulationFile()");
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
            // string sDateTime = "";

            try
            {
                // sDateTime = dtNow.ToString("MMddyyyy_HHmmss.fff");


                var bytes = File.ReadAllBytes(sOutFile);
                using (FileStream fs = new FileStream(sOutFile + "." + dtNow.Millisecond.ToString() + ".gz", FileMode.CreateNew))
                using (GZipStream zipStream = new GZipStream(fs, CompressionMode.Compress, false))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                }

                File.Delete(sOutFile);
                bSuccess = true;
            }
            catch (Exception ex)
            {
                LogFile.LogErrorText("Error in Link_Consumer_Globals.DoGZIP()");
                LogFile.LogErrorText(ex.ToString());
                LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static int SetAgencyID(string sAgencyName)
        {
            int iLclAgencyID = 0;

            if (Link_Consumer_Globals.dictAgencyInfo.ContainsValue(sAgencyName))
            {
                foreach (KeyValuePair<int, string> kvpAgency in Link_Consumer_Globals.dictAgencyInfo)
                {
                    if (kvpAgency.Value.ToString() == sAgencyName)
                    {
                        iLclAgencyID = Convert.ToInt32(kvpAgency.Key.ToString());
                        break;
                    }
                }
            }

            return iLclAgencyID;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

    }
}
