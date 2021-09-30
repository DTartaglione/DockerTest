using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PA_Alerts_Data_Consumer
{
    class PA_Alerts_Data_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                // PA_Alerts_Data_Consumer_Globals.LogFile.Log("compare: " + (DateTime.Now - PA_Alerts_Data_Consumer_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - PA_Alerts_Data_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (PA_Alerts_Data_Consumer_Globals.iAppCheckTimeoutInSeconds / 1000))
                {
                    PA_Alerts_Data_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " +
                        PA_Alerts_Data_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_Timers.AppCheckTimer()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                PA_Alerts_Data_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + PA_Alerts_Data_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error in PA_Alerts_Data_Consumer_Timers.doKeepAlive()");
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
