using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Waze_Route_Consumer
{
    class Waze_Route_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
               // Waze_Route_Consumer_Globals.LogFile.Log("compare: " + (DateTime.Now - Waze_Route_Consumer_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - Waze_Route_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (Waze_Route_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Waze_Route_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        Waze_Route_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_Timers.AppCheckTimer()");
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Waze_Route_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                Waze_Route_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + Waze_Route_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error in Waze_Route_Consumer_Timers.doKeepAlive()");
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
