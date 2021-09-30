using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Aviation_DI
{
    class Aviation_DI_Main
    {
        static void Main(string[] args)
        {
            
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Aviation_DI_Globals.UnhandledExceptionHandler);

            Aviation_DI_Globals.GetSQLConnectionStrings();
            Aviation_DI_Globals.ReadConfigParams();

            Aviation_DI_Globals.tmAppCheck.Elapsed += delegate { Aviation_DI_Timers.appCheckTimer(); };
            Aviation_DI_Globals.tmKeepAlive.Elapsed += delegate { Aviation_DI_Timers.doKeepAlive(); };

            Aviation_DI_Globals.tmAppCheck.Enabled = true;
            Aviation_DI_Globals.tmAppCheck.Interval = 60000;
            Aviation_DI_Globals.tmAppCheck.Start();

            Aviation_DI_Globals.tmKeepAlive.Enabled = true;
            Aviation_DI_Globals.tmKeepAlive.Interval = 30000;
            Aviation_DI_Globals.tmKeepAlive.Start();

            Aviation_DI_Globals.LogFile.Log("Application started.");

            //if (Aviation_DI_PublicDataFunctions.GetAgencyInfo())
           // {
                while (!Aviation_DI_PublicDataFunctions.GetStaticData())
                {
                    Aviation_DI_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    System.Threading.Thread.Sleep(10000);
                }

            Aviation_DI_KafkaConsumer.StartKafkaConsumer();
 
           // }

            while (Console.ReadLine() != "")
            {

            }                                                                                        
        }

    }
}
