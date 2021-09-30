using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Crossing_Data_Consumer
{
    class Crossing_Data_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - Crossing_Data_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (Crossing_Data_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Crossing_Data_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        Crossing_Data_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error in Crossing_Data_Consumer_Timers.AppCheckTimer()");
                Crossing_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Crossing_Data_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                Crossing_Data_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + Crossing_Data_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error in Crossing_Data_Consumer_Timers.doKeepAlive()");
                Crossing_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
