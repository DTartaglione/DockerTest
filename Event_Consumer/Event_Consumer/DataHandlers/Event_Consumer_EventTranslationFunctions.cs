using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_Consumer
{
    class Event_Consumer_EventTranslationFunctions
    {
        public static string GenerateLocalEventID(int iAgencyID)
        {
            string sLocalEventID = "";
            DateTime dtNow = DateTime.Now;
            string sDateTime = "";
            string sAgencyID = "";

            try
            {
                sDateTime = dtNow.ToString("yyyyMMddHHmmssffff");
                sAgencyID = iAgencyID.ToString("00000.##");

                sLocalEventID = sDateTime + sAgencyID;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_EventTranslationFunctions.GenerateLocalEventID()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return sLocalEventID;
        }

        public static ArrayList GetDirection(string sDirection, int iAgencyID)
        {
            int iDirection = Event_Consumer_Globals.iDefaultDirectionID;
            string sLocalDirection = "";
            ArrayList alDirection = new ArrayList();
            DirectionLookup _Direction = new DirectionLookup();

            try
            {
                if (sDirection != "")
                {
                    foreach (KeyValuePair<int, DirectionLookup> kvpDir in Event_Consumer_Globals.dictDirectionData)
                    {
                        _Direction = kvpDir.Value;

                        if (_Direction.iAgencyID == iAgencyID && _Direction.sLocalDirection == sDirection)
                        {
                            iDirection = _Direction.iLocalDirectionID;
                            sLocalDirection = _Direction.sLocalDirection;
                            break;
                        }
                    }
                }
                else
                {
                    iDirection = _Direction.iLocalDirectionID;
                }

                alDirection.Add(iDirection);
                alDirection.Add(sLocalDirection);
            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in EventTranslationFunctions.GetDirection()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return alDirection;
        }

        
        public static string GetTimestamp(DateTime dt)
        {
            string sTimeStamp = "";

            sTimeStamp = dt.ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss");

            return sTimeStamp;
        }
    }
}
