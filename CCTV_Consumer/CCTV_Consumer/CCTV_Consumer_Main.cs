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

namespace CCTV_Consumer
{
    class CCTV_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(CCTV_Consumer_Globals.UnhandledExceptionHandler);

            CCTV_Consumer_Globals.GetSQLConnectionStrings();
            CCTV_Consumer_Globals.ReadConfigParams();

            CCTV_Consumer_Globals.tmAppCheck.Elapsed += delegate { CCTV_Consumer_Timers.appCheckTimer(); };
            CCTV_Consumer_Globals.tmKeepAlive.Elapsed += delegate { CCTV_Consumer_Timers.doKeepAlive(); };

            CCTV_Consumer_Globals.tmAppCheck.Enabled = true;
            CCTV_Consumer_Globals.tmAppCheck.Interval = 60000;
            CCTV_Consumer_Globals.tmAppCheck.Start();

            CCTV_Consumer_Globals.tmKeepAlive.Enabled = true;
            CCTV_Consumer_Globals.tmKeepAlive.Interval = 30000;
            CCTV_Consumer_Globals.tmKeepAlive.Start();

            CCTV_Consumer_Globals.LogFile.Log("Application started.");

            if (CCTV_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!CCTV_Consumer_PublicDataFunctions.GetStaticData())
                {
                    CCTV_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                while (!CCTV_Consumer_PublicDataFunctions.GetCurrentDynamicData())
                {
                    CCTV_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                CCTV_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
