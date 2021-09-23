using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Configuration;
using System.Xml;
using System.Collections;
using System.Net;


namespace EventHubs_SampleProducer
{
    class EventHubs_SampleProducer_Main
    {

        static void Main(string[] arg)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(EventHubs_SampleProducer_Globals.UnhandledExceptionHandler);

            EventHubs_SampleProducer_Globals.GetSQLConnectionStrings();
            EventHubs_SampleProducer_Globals.ReadConfigParams();

            EventHubs_SampleProducer_Globals.tmAppCheck.Elapsed += delegate { EventHubs_SampleProducer_Timers.appCheckTimer(); };
            EventHubs_SampleProducer_Globals.tmKeepAlive.Elapsed += delegate { EventHubs_SampleProducer_Timers.doKeepAlive(); };

            EventHubs_SampleProducer_Globals.tmAppCheck.Enabled = true;
            EventHubs_SampleProducer_Globals.tmAppCheck.Interval = 60000;
            EventHubs_SampleProducer_Globals.tmAppCheck.Start();

            EventHubs_SampleProducer_Globals.tmKeepAlive.Enabled = true;
            EventHubs_SampleProducer_Globals.tmKeepAlive.Interval = 30000;
            EventHubs_SampleProducer_Globals.tmKeepAlive.Start();

            EventHubs_SampleProducer_Globals.tmGetData.Elapsed += delegate { StartDataProduce(); };
            EventHubs_SampleProducer_Globals.tmGetData.Interval = EventHubs_SampleProducer_Globals.iDataRefreshRateInSeconds;

            EventHubs_SampleProducer_Globals.LogFile.Log("Application started.");


            //if (EventHubs_SampleProducer_PublicDataFunctions.GetAgencyInfo())
            //{
            //    while (!EventHubs_SampleProducer_PublicDataFunctions.GetStaticData())
            //    {
            //        EventHubs_SampleProducer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
            //        System.Threading.Thread.Sleep(10000);
            //    }

            //}

            StartDataProduce();

            while (Console.ReadLine() != "")
            {

            }
        }
        private static void StartDataProduce()
        {
            EventHubs_SampleProducer_Globals.tmGetData.Stop();
            Thread t = new Thread(MakeRequestForData);
            t.Start();
        }

        private static void MakeRequestForData()
        {
            List<StringBuilder> lstData = new List<StringBuilder>();
            StringBuilder sbOut = new StringBuilder();

            try {
                if (EventHubs_SampleProducer_Globals.bSendToKafka)
                {
                    sbOut = new StringBuilder();

                    sbOut.Append("New message date/time is : " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                    lstData.Add(sbOut);

                    EventHubs_SampleProducer_KafkaProducer.DoBatchUpdateAsync(lstData);
                }
                else
                {
                    EventHubs_SampleProducer_Globals.LogFile.Log("SendToKafka false in app.config. No messages will be pushed. Restart.");
                }
            }
            catch (Exception ex)
            {
                EventHubs_SampleProducer_Globals.LogFile.LogErrorText("Error in MakeRequestForData().");
                EventHubs_SampleProducer_Globals.LogFile.LogErrorText(ex.ToString());
                EventHubs_SampleProducer_Globals.LogFile.LogError(ex);
            }
            finally
            {
                EventHubs_SampleProducer_Globals.LogFile.Log("Restarting data retrieve timer.");
                EventHubs_SampleProducer_Globals.tmGetData.Start();
            }
        }
    }
}
