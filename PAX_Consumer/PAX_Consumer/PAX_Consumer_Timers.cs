using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PAX_Consumer
{
    class PAX_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                // PAX_Consumer_Globals.LogFile.Log("compare: " + (DateTime.Now - PAX_Consumer_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - PAX_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (PAX_Consumer_Globals.iAppCheckTimeoutInSeconds / 1000))
                {
                    PAX_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " +
                        PAX_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_Timers.AppCheckTimer()");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                PAX_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                PAX_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + PAX_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_Timers.doKeepAlive()");
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
