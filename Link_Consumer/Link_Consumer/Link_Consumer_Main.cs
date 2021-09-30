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

namespace Link_Consumer
{
    class Link_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Link_Consumer_Globals.UnhandledExceptionHandler);

            Link_Consumer_Globals.GetSQLConnectionStrings();
            Link_Consumer_Globals.ReadConfigParams();

            Link_Consumer_Globals.tmAppCheck.Elapsed += delegate { Link_Consumer_Timers.appCheckTimer(); };
            Link_Consumer_Globals.tmKeepAlive.Elapsed += delegate { Link_Consumer_Timers.doKeepAlive(); };

            Link_Consumer_Globals.tmAppCheck.Enabled = true;
            Link_Consumer_Globals.tmAppCheck.Interval = 60000;
            Link_Consumer_Globals.tmAppCheck.Start();

            Link_Consumer_Globals.tmKeepAlive.Enabled = true;
            Link_Consumer_Globals.tmKeepAlive.Interval = 30000;
            Link_Consumer_Globals.tmKeepAlive.Start();

            Link_Consumer_Globals.LogFile.Log("Application started.");

            if (Link_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!Link_Consumer_PublicDataFunctions.GetStaticData())
                {
                    Link_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    System.Threading.Thread.Sleep(10000);
                }

                Link_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
