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

namespace Event_Consumer
{
    class Event_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Event_Consumer_Globals.UnhandledExceptionHandler);

            Event_Consumer_Globals.GetSQLConnectionStrings();
            Event_Consumer_Globals.ReadConfigParams();

            Event_Consumer_Globals.tmAppCheck.Elapsed += delegate { Event_Consumer_Timers.appCheckTimer(); };
            Event_Consumer_Globals.tmKeepAlive.Elapsed += delegate { Event_Consumer_Timers.doKeepAlive(); };
            Event_Consumer_Globals.tmGZipDailyLogs.Elapsed += delegate { Event_Consumer_Timers.doGZipDailyLogs(); };

            Event_Consumer_Globals.tmAppCheck.Enabled = true;
            Event_Consumer_Globals.tmAppCheck.Interval = 60000;
            Event_Consumer_Globals.tmAppCheck.Start();

            Event_Consumer_Globals.tmKeepAlive.Enabled = true;
            Event_Consumer_Globals.tmKeepAlive.Interval = 30000;
            Event_Consumer_Globals.tmKeepAlive.Start();

            Event_Consumer_Globals.tmGZipDailyLogs.Enabled = false;
            //Event_Consumer_Globals.tmGZipDailyLogs.Interval = 10000;//60000 * 60; //check once per hour
            //Event_Consumer_Globals.tmGZipDailyLogs.Start();

            Event_Consumer_Globals.LogFile.Log("Application started.");

            if (Event_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!Event_Consumer_PublicDataFunctions.GetStaticData())
                {
                    Event_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                while (!Event_Consumer_PublicDataFunctions.GetCurrentDynamicData())
                {
                    Event_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                Event_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
