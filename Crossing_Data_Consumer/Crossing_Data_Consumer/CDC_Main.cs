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

namespace Crossing_Data_Consumer
{
    class Crossing_Data_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Crossing_Data_Consumer_Globals.UnhandledExceptionHandler);

            Crossing_Data_Consumer_Globals.GetSQLConnectionStrings();
            Crossing_Data_Consumer_Globals.ReadConfigParams();

            Crossing_Data_Consumer_Globals.tmAppCheck.Elapsed += delegate { Crossing_Data_Consumer_Timers.appCheckTimer(); };
            Crossing_Data_Consumer_Globals.tmKeepAlive.Elapsed += delegate { Crossing_Data_Consumer_Timers.doKeepAlive(); };

            Crossing_Data_Consumer_Globals.tmAppCheck.Enabled = true;
            Crossing_Data_Consumer_Globals.tmAppCheck.Interval = 60000;
            Crossing_Data_Consumer_Globals.tmAppCheck.Start();

            Crossing_Data_Consumer_Globals.tmKeepAlive.Enabled = true;
            Crossing_Data_Consumer_Globals.tmKeepAlive.Interval = 30000;
            Crossing_Data_Consumer_Globals.tmKeepAlive.Start();

            Crossing_Data_Consumer_Globals.LogFile.Log("Application started.");

            if (Crossing_Data_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!Crossing_Data_Consumer_PublicDataFunctions.GetStaticData())
                {
                    Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                Crossing_Data_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
