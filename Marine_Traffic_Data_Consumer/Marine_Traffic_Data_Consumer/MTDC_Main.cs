using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using System.Configuration;
using System.Timers;
using System.Xml;

namespace Marine_Traffic_Data_Consumer
{
    class MTDC_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(MTDC_Globals.UnhandledExceptionHandler);

            MTDC_Globals.GetSQLConnectionStrings();
            MTDC_Globals.ReadConfigParams();

            MTDC_Globals.tmAppCheck.Elapsed += delegate { MTDC_Timers.appCheckTimer(); };
            MTDC_Globals.tmKeepAlive.Elapsed += delegate { MTDC_Timers.doKeepAlive(); };

            MTDC_Globals.tmAppCheck.Enabled = true;
            MTDC_Globals.tmAppCheck.Interval = 60000;
            MTDC_Globals.tmAppCheck.Start();

            MTDC_Globals.tmKeepAlive.Enabled = true;
            MTDC_Globals.tmKeepAlive.Interval = 30000;
            MTDC_Globals.tmKeepAlive.Start();

            MTDC_Globals.LogFile.Log("Application started.");

            if (MTDC_PublicDataFunctions.GetAgencyInfo())
            {
                while (!MTDC_PublicDataFunctions.GetStaticData())
                {
                    MTDC_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                MTDC_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
