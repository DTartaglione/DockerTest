using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EventHubs_SampleProducer
{
    class EventHubs_SampleProducer_Timers
    {

        public static void appCheckTimer()
        {
            try
            {
                if ((DateTime.Now - EventHubs_SampleProducer_Globals.dtLastKeepAlive).TotalSeconds > (EventHubs_SampleProducer_Globals.iAppCheckTimeoutInSeconds / 1000))
                {
                    EventHubs_SampleProducer_Globals.LogFile.Log("App check monitor returned failure. Last keep alive was " +
                        EventHubs_SampleProducer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ". App hung. Restart.");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                EventHubs_SampleProducer_Globals.LogFile.LogErrorText("Error in EventHubs_SampleProducer_Timers.AppCheckTimer().");
                EventHubs_SampleProducer_Globals.LogFile.LogErrorText(ex.ToString());
                EventHubs_SampleProducer_Globals.LogFile.LogError(ex);
            }
        }

        public static void doKeepAlive()
        {
            try
            {
                EventHubs_SampleProducer_Globals.dtLastKeepAlive = DateTime.Now;
                EventHubs_SampleProducer_Globals.LogFile.Log("Performed keep alive. Timer set to " + EventHubs_SampleProducer_Globals.dtLastKeepAlive.ToString("MM/dd/yyyy HH:mm:ss") + ".");
            }
            catch (Exception ex)
            {
                EventHubs_SampleProducer_Globals.LogFile.LogErrorText("Error in EventHubs_SampleProducer_Timers.doKeepAlive().");
                EventHubs_SampleProducer_Globals.LogFile.LogErrorText(ex.ToString());
                EventHubs_SampleProducer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
