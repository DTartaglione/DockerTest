using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Transit_Consumer
{
    class Transit_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - Transit_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (Transit_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Transit_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " +
                        Transit_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_Timers.AppCheckTimer()");
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Transit_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                Transit_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + Transit_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_Timers.doKeepAlive()");
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }
        }
        
    }
}
