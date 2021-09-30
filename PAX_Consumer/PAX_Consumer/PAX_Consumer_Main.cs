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

namespace PAX_Consumer
{
    class PAX_Consumer_Main
    {

        static void Main(string[] arg)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(PAX_Consumer_Globals.UnhandledExceptionHandler);

            PAX_Consumer_Globals.GetSQLConnectionStrings();
            PAX_Consumer_Globals.ReadConfigParams();

            PAX_Consumer_Globals.tmAppCheck.Elapsed += delegate { PAX_Consumer_Timers.appCheckTimer(); };
            PAX_Consumer_Globals.tmKeepAlive.Elapsed += delegate { PAX_Consumer_Timers.doKeepAlive(); };

            PAX_Consumer_Globals.tmAppCheck.Enabled = true;
            PAX_Consumer_Globals.tmAppCheck.Interval = 60000;
            PAX_Consumer_Globals.tmAppCheck.Start();

            PAX_Consumer_Globals.tmKeepAlive.Enabled = true;
            PAX_Consumer_Globals.tmKeepAlive.Interval = 30000;
            PAX_Consumer_Globals.tmKeepAlive.Start();

            PAX_Consumer_Globals.LogFile.Log("Application started.");

            if (PAX_Consumer_PublicDataFunctions.GetAgencyInfo() &&
                PAX_Consumer_PublicDataFunctions.GetAirportInfo() &&
                PAX_Consumer_PublicDataFunctions.GetFlightTypeInfo() &&
                PAX_Consumer_PublicDataFunctions.GetPAXTypeAirlineInfo() &&
                PAX_Consumer_PublicDataFunctions.GetPAXTypeInfo())
            {
                while (!PAX_Consumer_PublicDataFunctions.GetStaticData())
                {
                    PAX_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    Thread.Sleep(10000);
                }

                if (PAX_Consumer_Globals.bSimulationEnabled)
                {
                    dynamic dData = "";
                    List<dynamic> lstData = new List<dynamic>();
                    dData = File.ReadAllText(PAX_Consumer_Globals.sSimulationPath);
                    lstData.Add(dData);

                    if (dData.Length > 0)
                    {
                        PAX_Consumer_Globals.LogFile.Log("Successfully retrieved simulation JSON. Now handle response");
                        PAX_Consumer_HandleResponse.HandleResponse(lstData);
                    }
                    else
                    {
                        PAX_Consumer_Globals.LogFile.Log("Failed to retrieve simulation JSON file. Please verify simulation path and contents.");
                    }
                }

                else
                {
                    PAX_Consumer_KafkaConsumer.StartKafkaConsumer();
                }
            }


            while (Console.ReadLine() != "")
            {

            }
        }
    }
}
