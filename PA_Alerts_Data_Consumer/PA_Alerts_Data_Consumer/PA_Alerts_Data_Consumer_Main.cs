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

namespace PA_Alerts_Data_Consumer
{
    class PA_Alerts_Data_Consumer_Main
    {

        static void Main(string[] arg)
        {
            //Unhandled exception handler
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.UnhandledException += new UnhandledExceptionEventHandler(PA_Alerts_Data_Consumer_Globals.UnhandledExceptionHandler);

            PA_Alerts_Data_Consumer_Globals.GetSQLConnectionStrings();
            PA_Alerts_Data_Consumer_Globals.ReadConfigParams();

            PA_Alerts_Data_Consumer_Globals.tmAppCheck.Elapsed += delegate { PA_Alerts_Data_Consumer_Timers.appCheckTimer(); };
            PA_Alerts_Data_Consumer_Globals.tmKeepAlive.Elapsed += delegate { PA_Alerts_Data_Consumer_Timers.doKeepAlive(); };

            PA_Alerts_Data_Consumer_Globals.tmAppCheck.Enabled = true;
            PA_Alerts_Data_Consumer_Globals.tmAppCheck.Interval = 60000;
            PA_Alerts_Data_Consumer_Globals.tmAppCheck.Start();

            PA_Alerts_Data_Consumer_Globals.tmKeepAlive.Enabled = true;
            PA_Alerts_Data_Consumer_Globals.tmKeepAlive.Interval = 30000;
            PA_Alerts_Data_Consumer_Globals.tmKeepAlive.Start();

            PA_Alerts_Data_Consumer_Globals.tmAlertDurationCheck.Elapsed += delegate { AlertDurationCheck(); };
            PA_Alerts_Data_Consumer_Globals.tmAlertDurationCheck.Interval = PA_Alerts_Data_Consumer_Globals.iAlertDurationCheckInSeconds;

            PA_Alerts_Data_Consumer_Globals.LogFile.Log("Application started.");


            if (PA_Alerts_Data_Consumer_PublicDataFunctions.GetAgencyInfo()
                && PA_Alerts_Data_Consumer_PublicDataFunctions.GetAOCInfo()
                && PA_Alerts_Data_Consumer_PublicDataFunctions.GetEmailTypeInfo()
                && PA_Alerts_Data_Consumer_PublicDataFunctions.GetActiveAlerts()
                && PA_Alerts_Data_Consumer_PublicDataFunctions.GetLatestPAXUpdateInfo())
            {
                while (!PA_Alerts_Data_Consumer_PublicDataFunctions.GetStaticData())
                {
                    PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error reading application dynamic data from database. Wait 10 seconds and try again.");
                    System.Threading.Thread.Sleep(10000);
                }

                StartDataRetreive();
                DeactivateOldAlerts();
            }

            while (Console.ReadLine() != "")
            {

            }
        }

        private static void StartDataRetreive()
        {
            Thread t = new Thread(MakeRequestForData);
            t.Start();
        }
       
        private static void MakeRequestForData()
        {
            StringBuilder sbEmail = new StringBuilder();
            ArrayList alURLArray = new ArrayList();
            int i = 0;

            try
            {
                if (PA_Alerts_Data_Consumer_Globals.bSimulationEnabled)
                {
                    for (i = 0; i < PA_Alerts_Data_Consumer_Globals.aSimulationFiles.Length; i++)
                    {
                        alURLArray.Add(PA_Alerts_Data_Consumer_Globals.aSimulationFiles[i]);
                    }

                    sbEmail.Append(PA_Alerts_Data_Consumer_Globals.GetJsonFromSimulationFile(alURLArray[0].ToString()));

                }

                else
                {
                    PA_Alerts_Data_Consumer_KafkaConsumer.StartKafkaConsumer();
                }

            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        private static void AlertDurationCheck()
        {
            PA_Alerts_Data_Consumer_Globals.tmAlertDurationCheck.Stop();

            Thread t = new Thread(DeactivateOldAlerts);
            t.Start();

        }

        private static void DeactivateOldAlerts()
        {
            List<int> lstRemoveFromAlertDict = new List<int>();

            try
            {
                //In case we do any manual db edits, clear the active alerts dict and repopulate
                PA_Alerts_Data_Consumer_Globals.dictActiveAlerts.Clear();
                PA_Alerts_Data_Consumer_PublicDataFunctions.GetActiveAlerts();

                DateTime dtRemoveIfBefore = DateTime.Now.AddHours(PA_Alerts_Data_Consumer_Globals.iDisableAlertsOlderThanValueInHours * -1);

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Begin check for alerts older than " +
                    PA_Alerts_Data_Consumer_Globals.iDisableAlertsOlderThanValueInHours.ToString() + " hours.");

                foreach (KeyValuePair<int, DateTime> kvp in PA_Alerts_Data_Consumer_Globals.dictActiveAlerts)
                {
                    if (kvp.Value < dtRemoveIfBefore)
                    {
                        PA_Alerts_Data_Consumer_Globals.LogFile.Log("Alert: " + kvp.Key.ToString() + " is older than disable parameter. Setting it to inactive.");
                        PA_Alerts_Data_Consumer_PublicDataFunctions.DeactivateAlert(kvp.Key,PA_Alerts_Data_Consumer_Globals.iDisableAlertsOlderThanValueInHours);
                        lstRemoveFromAlertDict.Add(kvp.Key);
                    }
                }

                //in case of any manual updates to db, pass -1 which will disable all events greater than our parameter
                PA_Alerts_Data_Consumer_PublicDataFunctions.DeactivateAlert(-1, PA_Alerts_Data_Consumer_Globals.iDisableAlertsOlderThanValueInHours);

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Old alerts disabled. Now remove these alerts from dictionary.");
                foreach (int lstItem in lstRemoveFromAlertDict)
                {
                    PA_Alerts_Data_Consumer_Globals.dictActiveAlerts.Remove(lstItem);
                }
            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
                PA_Alerts_Data_Consumer_Globals.tmAlertDurationCheck.Start();
            }
            finally
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Finished checking for old alerts.");
                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Resetting PA Alerts Consumer Alert Duration Timer.");

                PA_Alerts_Data_Consumer_Globals.tmAlertDurationCheck.Start();
            }
        }
    }
}
