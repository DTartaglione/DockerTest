using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Link_Consumer
{
    class Link_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
               // Link_Consumer_Globals.LogFile.Log("compare: " + (DateTime.Now - Link_Consumer_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - Link_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (Link_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Link_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        Link_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_Timers.AppCheckTimer()");
                Link_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Link_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                Link_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + Link_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_Timers.doKeepAlive()");
                Link_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
