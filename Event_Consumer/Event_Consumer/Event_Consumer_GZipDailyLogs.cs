using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Event_Consumer
{
    class Event_Consumer_GZipDailyLogs
    {

        public static void GZipDailyLogs()
        {
            try
            {
                string sFileDir = Event_Consumer_Globals.sReceivedMessagesDir;
                DirectoryInfo di = new DirectoryInfo(sFileDir);
                FileInfo[] fiInfo = di.GetFiles("*.log");
                DateTime dtCurrDate = new DateTime();
                DateTime dtFileDate = new DateTime();

                dtCurrDate = DateTime.Now;

                if (fiInfo.Length > 0)
                {
                    foreach (FileInfo fi in fiInfo)
                    {
                        dtFileDate = fi.LastWriteTime.Date;
                        if (dtCurrDate.Date > dtFileDate.Date)
                        {
                            //only gzip files from previous days
                            Event_Consumer_Globals.DoGZIP(sFileDir + "\\" + fi.Name);
                        }
                    }

                    Event_Consumer_Globals.LogFile.Log("Finished GZipping any .log files in folder " + sFileDir + ".");
                }
            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in GZipDailyLogs(). " + ex.ToString());
                Event_Consumer_Globals.LogFile.LogError(ex);
            }
        }
    }
}
