using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ATR_Data_Consumer
{
    class ATR_Data_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - ATR_Data_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (ATR_Data_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    ATR_Data_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        ATR_Data_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error in ATR_Data_Consumer_Timers.AppCheckTimer()");
                ATR_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                ATR_Data_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                ATR_Data_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + ATR_Data_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error in ATR_Data_Consumer_Timers.doKeepAlive()");
                ATR_Data_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
