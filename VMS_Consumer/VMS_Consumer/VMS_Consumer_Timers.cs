using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VMS_Consumer
{
    class VMS_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
               // VMS_Consumer_Globals.LogFile.Log("compare: " + (DateTime.Now - VMS_Consumer_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - VMS_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (VMS_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    VMS_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        VMS_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                VMS_Consumer_Globals.LogFile.LogErrorText("Error in VMS_Consumer_Timers.AppCheckTimer()");
                VMS_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                VMS_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                VMS_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + VMS_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                VMS_Consumer_Globals.LogFile.LogErrorText("Error in VMS_Consumer_Timers.doKeepAlive()");
                VMS_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
