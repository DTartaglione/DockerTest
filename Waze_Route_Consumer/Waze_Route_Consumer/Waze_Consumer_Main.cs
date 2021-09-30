using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Configuration;
using System.Xml;
using System.Collections;

namespace Waze_Route_Consumer
{
    class Waze_Route_Consumer_Main
    {

        static void Main(string[] arg)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(Waze_Route_Consumer_Globals.UnhandledExceptionHandler);

            Waze_Route_Consumer_Globals.GetSQLConnectionStrings();
            Waze_Route_Consumer_Globals.ReadConfigParams();

            Waze_Route_Consumer_Globals.tmAppCheck.Elapsed += delegate { Waze_Route_Consumer_Timers.appCheckTimer(); };
            Waze_Route_Consumer_Globals.tmKeepAlive.Elapsed += delegate { Waze_Route_Consumer_Timers.doKeepAlive(); };

            Waze_Route_Consumer_Globals.tmAppCheck.Enabled = true;
            Waze_Route_Consumer_Globals.tmAppCheck.Interval = 60000;
            Waze_Route_Consumer_Globals.tmAppCheck.Start();

            Waze_Route_Consumer_Globals.tmKeepAlive.Enabled = true;
            Waze_Route_Consumer_Globals.tmKeepAlive.Interval = 30000;
            Waze_Route_Consumer_Globals.tmKeepAlive.Start();

            Waze_Route_Consumer_Globals.LogFile.Log("Application started.");

            if (Waze_Route_Consumer_PublicDataFunctions.GetAgencyInfo() && Waze_Route_Consumer_PublicDataFunctions.GetRouteLastUpdateData() && 
                Waze_Route_Consumer_PublicDataFunctions.GetRouteLookupData() && Waze_Route_Consumer_PublicDataFunctions.GetSubrouteLookupData())
            {
                while (!Waze_Route_Consumer_PublicDataFunctions.GetStaticData())
                {
                    Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                if (Waze_Route_Consumer_Globals.bSimulationEnabled)
                {
                    dynamic dWazeRoutes = "";
                    List<dynamic> lstWazeRoutes = new List<dynamic>();
                    dWazeRoutes = File.ReadAllText(Waze_Route_Consumer_Globals.sSimulationPath);
                    lstWazeRoutes.Add(dWazeRoutes);

                    if (dWazeRoutes.Length > 0)
                    {
                        Waze_Route_Consumer_Globals.LogFile.Log("Successfully retrieved simulation JSON. Now handle response");
                        Waze_Route_Consumer_HandleResponse.HandleResponse(lstWazeRoutes);
                    }
                    else
                    {
                        Waze_Route_Consumer_Globals.LogFile.Log("Failed to retrieve simulation JSON file. Please verify simulation path and contents.");
                    }
                }

                else
                {
                    Waze_Route_Consumer_KafkaConsumer.StartKafkaConsumer();
                }
            }
            

            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
