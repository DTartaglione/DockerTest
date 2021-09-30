using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Event_Consumer
{
    class Event_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
               // Event_Consumer_Globals.LogFile.Log("compare: " + (DateTime.Now - Event_Consumer_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - Event_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (Event_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Event_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        Event_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_Timers.AppCheckTimer()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Event_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                Event_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + Event_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_Timers.doKeepAlive()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doGZipDailyLogs()
        {
            try
            {
                Event_Consumer_GZipDailyLogs.GZipDailyLogs();
            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_Timers.doGZipDailyLogs()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
