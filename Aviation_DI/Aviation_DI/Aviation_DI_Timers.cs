using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Aviation_DI
{
    class Aviation_DI_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
               // Aviation_DI_Globals.LogFile.Log("compare: " + (DateTime.Now - Aviation_DI_Globals.dtLastAppCheck).TotalSeconds);
                if ((DateTime.Now - Aviation_DI_Globals.dtLastKeepAlive).TotalSeconds > (Aviation_DI_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Aviation_DI_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        Aviation_DI_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Aviation_DI_Globals.LogFile.LogErrorText("Error in Aviation_DI_Timers.AppCheckTimer()");
                Aviation_DI_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Aviation_DI_Globals.dtLastKeepAlive = DateTime.Now;
                Aviation_DI_Globals.LogFile.Log("Performed keep alive. Timer set to " + Aviation_DI_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Aviation_DI_Globals.LogFile.LogErrorText("Error in Aviation_DI_Timers.doKeepAlive()");
                Aviation_DI_Globals.LogFile.LogError(ex);
            }
        }
    }
}
