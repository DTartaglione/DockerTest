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

namespace VMS_Consumer
{
    class VMS_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(VMS_Consumer_Globals.UnhandledExceptionHandler);

            VMS_Consumer_Globals.GetSQLConnectionStrings();
            VMS_Consumer_Globals.ReadConfigParams();

            VMS_Consumer_Globals.tmAppCheck.Elapsed += delegate { VMS_Consumer_Timers.appCheckTimer(); };
            VMS_Consumer_Globals.tmKeepAlive.Elapsed += delegate { VMS_Consumer_Timers.doKeepAlive(); };

            VMS_Consumer_Globals.tmAppCheck.Enabled = true;
            VMS_Consumer_Globals.tmAppCheck.Interval = 60000;
            VMS_Consumer_Globals.tmAppCheck.Start();

            VMS_Consumer_Globals.tmKeepAlive.Enabled = true;
            VMS_Consumer_Globals.tmKeepAlive.Interval = 30000;
            VMS_Consumer_Globals.tmKeepAlive.Start();

            VMS_Consumer_Globals.LogFile.Log("Application started.");

            if (VMS_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!VMS_Consumer_PublicDataFunctions.GetStaticData())
                {
                    VMS_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                while (!VMS_Consumer_PublicDataFunctions.GetCurrentDynamicData())
                {
                    VMS_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                VMS_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
