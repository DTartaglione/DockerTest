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

namespace ATR_Data_Consumer
{
    class ATR_Data_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(ATR_Data_Consumer_Globals.UnhandledExceptionHandler);

            ATR_Data_Consumer_Globals.GetSQLConnectionStrings();
            ATR_Data_Consumer_Globals.ReadConfigParams();

            ATR_Data_Consumer_Globals.tmAppCheck.Elapsed += delegate { ATR_Data_Consumer_Timers.appCheckTimer(); };
            ATR_Data_Consumer_Globals.tmKeepAlive.Elapsed += delegate { ATR_Data_Consumer_Timers.doKeepAlive(); };

            ATR_Data_Consumer_Globals.tmAppCheck.Enabled = true;
            ATR_Data_Consumer_Globals.tmAppCheck.Interval = 60000;
            ATR_Data_Consumer_Globals.tmAppCheck.Start();

            ATR_Data_Consumer_Globals.tmKeepAlive.Enabled = true;
            ATR_Data_Consumer_Globals.tmKeepAlive.Interval = 30000;
            ATR_Data_Consumer_Globals.tmKeepAlive.Start();

            ATR_Data_Consumer_Globals.tmGZipDailyLogs.Enabled = true;
            ATR_Data_Consumer_Globals.tmGZipDailyLogs.Interval = 10000;//60000 * 60; //check once per hour
            ATR_Data_Consumer_Globals.tmGZipDailyLogs.Start();

            ATR_Data_Consumer_Globals.LogFile.Log("Application started.");

            if (ATR_Data_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!ATR_Data_Consumer_PublicDataFunctions.GetStaticData())
                {
                    ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                //while (!ATR_Data_Consumer_PublicDataFunctions.GetCurrentDynamicData())
                //{
                //    ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                //    Thread.Sleep(10000);
                //}

                ATR_Data_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
