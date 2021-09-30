using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Vehicle_Record_Consumer
{
    class Vehicle_Record_Consumer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - Vehicle_Record_Consumer_Globals.dtLastKeepAlive).TotalSeconds > (Vehicle_Record_Consumer_Globals.iAppCheckTimeoutInSeconds/1000))
                {
                    Vehicle_Record_Consumer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " + 
                        Vehicle_Record_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error in Vehicle_Record_Consumer_Timers.AppCheckTimer()");
                Vehicle_Record_Consumer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                Vehicle_Record_Consumer_Globals.dtLastKeepAlive = DateTime.Now;
                Vehicle_Record_Consumer_Globals.LogFile.Log("Performed keep alive. Timer set to " + Vehicle_Record_Consumer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error in Vehicle_Record_Consumer_Timers.doKeepAlive()");
                Vehicle_Record_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
