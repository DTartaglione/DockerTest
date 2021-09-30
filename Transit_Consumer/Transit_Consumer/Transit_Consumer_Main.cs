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

namespace Transit_Consumer
{
    class Transit_Consumer_Main
    {
       
        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Transit_Consumer_Globals.UnhandledExceptionHandler);

            Transit_Consumer_Globals.GetSQLConnectionStrings();
            Transit_Consumer_Globals.ReadConfigParams();

            Transit_Consumer_Globals.tmAppCheck.Elapsed += delegate { Transit_Consumer_Timers.appCheckTimer(); };
            Transit_Consumer_Globals.tmKeepAlive.Elapsed += delegate { Transit_Consumer_Timers.doKeepAlive(); };
          
            Transit_Consumer_Globals.tmAppCheck.Enabled = true;
            Transit_Consumer_Globals.tmAppCheck.Interval = 60000;
            Transit_Consumer_Globals.tmAppCheck.Start();

            Transit_Consumer_Globals.tmKeepAlive.Enabled = true;
            Transit_Consumer_Globals.tmKeepAlive.Interval = 30000;
            Transit_Consumer_Globals.tmKeepAlive.Start();
            
            Transit_Consumer_Globals.LogFile.Log("Application started.");

            if (Transit_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!Transit_Consumer_PublicDataFunctions.GetStaticData())
                {
                    Transit_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                while (!Transit_Consumer_PublicDataFunctions.GetCurrentDynamicData())
                {
                    Transit_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                Transit_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
