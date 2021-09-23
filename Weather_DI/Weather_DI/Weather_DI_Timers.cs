using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Weather_DI
{
    class Weather_DI_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - Weather_DI_Globals.dtLastKeepAlive).TotalSeconds > (Weather_DI_Globals.iAppCheckTimeoutInSeconds / 1000))
                {
                    Weather_DI_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " +
                        Weather_DI_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Weather_DI_Globals.LogFile.LogErrorText("Error in Weather_DI_Timers.AppCheckTimer()");
                Weather_DI_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Weather_DI_Globals.dtLastKeepAlive = DateTime.Now;
                Weather_DI_Globals.LogFile.Log("Performed keep alive. Timer set to " + Weather_DI_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Weather_DI_Globals.LogFile.LogErrorText("Error in Weather_DI_Timers.doKeepAlive()");
                Weather_DI_Globals.LogFile.LogError(ex);
            }
        }
    }
}
