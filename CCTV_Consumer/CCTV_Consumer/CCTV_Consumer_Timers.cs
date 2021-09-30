using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CCTV_Consumer
{
    class CCTV_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
               // CCTV_Consumer_Globals.LogFile.Log("compare: " + (DateTime.Now - CCTV_Consumer_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - CCTV_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (CCTV_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    CCTV_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        CCTV_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                CCTV_Consumer_Globals.LogFile.LogErrorText("Error in CCTV_Consumer_Timers.AppCheckTimer()");
                CCTV_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                CCTV_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                CCTV_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + CCTV_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                CCTV_Consumer_Globals.LogFile.LogErrorText("Error in CCTV_Consumer_Timers.doKeepAlive()");
                CCTV_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
