using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Video_Detection_Consumer
{
    class Video_Detection_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - Video_Detection_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (Video_Detection_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Video_Detection_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        Video_Detection_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error in Video_Detection_Consumer_Timers.AppCheckTimer()");
                Video_Detection_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Video_Detection_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                Video_Detection_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + Video_Detection_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error in Video_Detection_Consumer_Timers.doKeepAlive()");
                Video_Detection_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
