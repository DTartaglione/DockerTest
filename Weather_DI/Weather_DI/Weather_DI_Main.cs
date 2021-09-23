using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace Weather_DI
{
    class Weather_DI_Main
    {
        static void Main(string[] args)
        {

            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Weather_DI_Globals.UnhandledExceptionHandler);

            Weather_DI_Globals.GetSQLConnectionStrings();
            Weather_DI_Globals.ReadConfigParams();
            
            Weather_DI_Globals.tmAppCheck.Elapsed += delegate { Weather_DI_Timers.appCheckTimer(); };
            Weather_DI_Globals.tmKeepAlive.Elapsed += delegate { Weather_DI_Timers.doKeepAlive(); };

            Weather_DI_Globals.tmAppCheck.Enabled = true;
            Weather_DI_Globals.tmAppCheck.Interval = 60000;
            Weather_DI_Globals.tmAppCheck.Start();

            Weather_DI_Globals.tmKeepAlive.Enabled = true;
            Weather_DI_Globals.tmKeepAlive.Interval = 30000;
            Weather_DI_Globals.tmKeepAlive.Start();

            Weather_DI_Globals.LogFile.Log("Application started.");

            Weather_DI_Globals.tmGetWeatherForecastData.Interval = Weather_DI_Globals.iWeatherForecast_DataRefreshRateInSeconds;

            Weather_DI_KafkaConsumer.StartKafkaConsumer();

            while (Console.ReadLine() != "")
            {

            }                                                                                        
        }            
    }
}
