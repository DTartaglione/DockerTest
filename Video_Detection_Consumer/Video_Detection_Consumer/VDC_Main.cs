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

namespace Video_Detection_Consumer
{
    class Video_Detection_Consumer_Main
    {
       // public static int iAgencyID = 0;

        static void Main(string[] args)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Video_Detection_Consumer_Globals.UnhandledExceptionHandler);

            Video_Detection_Consumer_Globals.GetSQLConnectionStrings();
            Video_Detection_Consumer_Globals.ReadConfigParams();

            Video_Detection_Consumer_Globals.tmAppCheck.Elapsed += delegate { Video_Detection_Consumer_Timers.appCheckTimer(); };
            Video_Detection_Consumer_Globals.tmKeepAlive.Elapsed += delegate { Video_Detection_Consumer_Timers.doKeepAlive(); };

            Video_Detection_Consumer_Globals.tmAppCheck.Enabled = true;
            Video_Detection_Consumer_Globals.tmAppCheck.Interval = 60000;
            Video_Detection_Consumer_Globals.tmAppCheck.Start();

            Video_Detection_Consumer_Globals.tmKeepAlive.Enabled = true;
            Video_Detection_Consumer_Globals.tmKeepAlive.Interval = 30000;
            Video_Detection_Consumer_Globals.tmKeepAlive.Start();

            Video_Detection_Consumer_Globals.tmGZipDailyLogs.Enabled = true;
            Video_Detection_Consumer_Globals.tmGZipDailyLogs.Interval = 10000;//60000 * 60; //check once per hour
            Video_Detection_Consumer_Globals.tmGZipDailyLogs.Start();

            Video_Detection_Consumer_Globals.LogFile.Log("Application started.");

            if (Video_Detection_Consumer_PublicDataFunctions.GetAgencyInfo())
            {
                while (!Video_Detection_Consumer_PublicDataFunctions.GetStaticData())
                {
                    Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error reading application static data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                Video_Detection_Consumer_KafkaConsumer.StartKafkaConsumer();
            }

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
