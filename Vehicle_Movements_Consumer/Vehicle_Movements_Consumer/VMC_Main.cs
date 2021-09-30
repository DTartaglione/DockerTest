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

namespace Vehicle_Movements_Consumer
{
    class Vehicle_Movements_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Vehicle_Movements_Consumer_Globals.UnhandledExceptionHandler);

            Vehicle_Movements_Consumer_Globals.GetSQLConnectionStrings();
            Vehicle_Movements_Consumer_Globals.ReadConfigParams();

            Vehicle_Movements_Consumer_Globals.tmAppCheck.Elapsed += delegate { Vehicle_Movements_Consumer_Timers.appCheckTimer(); };
            Vehicle_Movements_Consumer_Globals.tmKeepAlive.Elapsed += delegate { Vehicle_Movements_Consumer_Timers.doKeepAlive(); };

            Vehicle_Movements_Consumer_Globals.tmAppCheck.Enabled = true;
            Vehicle_Movements_Consumer_Globals.tmAppCheck.Interval = 60000;
            Vehicle_Movements_Consumer_Globals.tmAppCheck.Start();

            Vehicle_Movements_Consumer_Globals.tmKeepAlive.Enabled = true;
            Vehicle_Movements_Consumer_Globals.tmKeepAlive.Interval = 30000;
            Vehicle_Movements_Consumer_Globals.tmKeepAlive.Start();

            Vehicle_Movements_Consumer_Globals.tmGZipDailyLogs.Enabled = true;
            Vehicle_Movements_Consumer_Globals.tmGZipDailyLogs.Interval = 10000;//60000 * 60; //check once per hour
            Vehicle_Movements_Consumer_Globals.tmGZipDailyLogs.Start();

            Vehicle_Movements_Consumer_Globals.LogFile.Log("Application started.");

            if (Vehicle_Movements_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!Vehicle_Movements_Consumer_PublicDataFunctions.GetStaticData())
                {
                    Vehicle_Movements_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                Vehicle_Movements_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
