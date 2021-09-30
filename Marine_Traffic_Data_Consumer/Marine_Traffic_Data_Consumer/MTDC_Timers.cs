using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Marine_Traffic_Data_Consumer
{
    class MTDC_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - MTDC_Globals.dtLastKeepAlive).TotalSeconds > (MTDC_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    MTDC_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        MTDC_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                MTDC_Globals.LogFile.LogErrorText("Error in FA_DI_Timers.AppCheckTimer()");
                MTDC_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                MTDC_Globals.dtLastKeepAlive = DateTime.Now;
                MTDC_Globals.LogFile.Log("Performed keep alive. Timer set to " + MTDC_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                MTDC_Globals.LogFile.LogErrorText("Error in FA_DI_Timers.doKeepAlive()");
                MTDC_Globals.LogFile.LogError(ex);
            }
        }
    }
}
