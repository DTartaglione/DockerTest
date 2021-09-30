using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transit_Consumer
{
    class Transit_Consumer_ProcessTripUpdates
    {
        private static int iNextStopTimeEventId = 0;

        public static bool ProcessTripUpdates(List<TripUpdates> lstTripUpdates)
        {
            string sFileName = "";
            bool bResult = true;
            StringBuilder[] sbCurrentLine = new StringBuilder[2];
            StringBuilder sbStopTimeUpdates = new StringBuilder();
            StringBuilder sbStopTimeEvents = new StringBuilder();

            iNextStopTimeEventId = GetNextStopTimeEventId();

            foreach (TripUpdates _TripUpdate in lstTripUpdates)
            {
                try
                {
                    //process the update
                    sbCurrentLine = InsertTripUpdates(_TripUpdate);

                    if (sbCurrentLine[0].Length == 0)
                    {
                        Transit_Consumer_Globals.LogFile.LogErrorText("Trip Update failed to insert stop time updates. See logs for details.");
                    }
                    else
                    {
                        sbStopTimeUpdates.Append(sbCurrentLine[0]);
                        sbCurrentLine[0].Remove(0, sbCurrentLine[0].Length);
                    }

                    if (sbCurrentLine[1].Length == 0)
                    {
                        Transit_Consumer_Globals.LogFile.LogErrorText("Trip Update failed to insert stop time events. See logs for details.");
                    }
                    else
                    {
                        sbStopTimeEvents.Append(sbCurrentLine[1]);
                        sbCurrentLine[1].Remove(0, sbCurrentLine[1].Length);
                    }

                }
                catch (Exception ex)
                {
                    Transit_Consumer_Globals.LogFile.LogErrorText("Error in ProcessTripUpdates().");
                    Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                    Transit_Consumer_Globals.LogFile.LogError(ex);
                    bResult = false;
                }
            }

            if (sbStopTimeUpdates.Length > 0)
            {
                sFileName = Transit_Consumer_Globals.GenerateFileName("stoptimeupdates_") + ".csv";
                using (StreamWriter writer = new StreamWriter(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName, false))
                {
                    writer.Write(sbStopTimeUpdates.ToString());
                    sbStopTimeUpdates.Remove(0, sbStopTimeUpdates.Length);
                    Transit_Consumer_Globals.LogFile.Log("Successfully dumped stop time updates to text file.");

                }

                if (File.Exists(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName))
                {
                    if (ProcessStopTimeUpdates(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName, "stoptimeupdates", "tblstoptimeupdate"))
                    {
                        if (Transit_Consumer_Globals.DoGZIP(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName))
                        {
                            Transit_Consumer_Globals.LogFile.Log("Successfully GZipped stop time updates output text file.");
                        }
                    }
                }
            }

            if (sbStopTimeEvents.Length > 0)
            {
                sFileName = Transit_Consumer_Globals.GenerateFileName("stoptimeevents_") + ".csv";
                using (StreamWriter writer = new StreamWriter(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName, false))
                {
                    writer.Write(sbStopTimeEvents.ToString());
                    sbStopTimeEvents.Remove(0, sbStopTimeEvents.Length);
                    Transit_Consumer_Globals.LogFile.Log("Successfully dumped stop time events to text file.");

                }

                if (File.Exists(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName))
                {
                    if (ProcessStopTimeUpdates(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName, "stoptimeevents", "tblstoptimeevent"))
                    {
                        if (Transit_Consumer_Globals.DoGZIP(Transit_Consumer_Globals.sStoredOutputFilePath + "\\" + sFileName))
                        {
                            Transit_Consumer_Globals.LogFile.Log("Successfully GZipped stop time events output text file.");
                        }
                    }
                }
            }

            if (Transit_Consumer_PublicDataFunctions.GetCurrentTripUpdateData())
            {
                Transit_Consumer_Globals.LogFile.Log("Refreshed trip update data storage in memory.");
            }

            return bResult;

        }

        private static int GetNextStopTimeEventId()
        {

            int iNextStopTimeEventId = 0;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select last_value from transitds.tblstoptimeevent_id_seq as curval;";

            try
            {

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                iNextStopTimeEventId = Convert.ToInt32(rsRead[0]);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in GetNextStopTimeEventId()");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
            }

            return iNextStopTimeEventId;
        }
        
        public static StringBuilder[] InsertTripUpdates(TripUpdates _TripUpdate)
        {
           // bool bResult = false;
            string sSQL = "transitds.sp_updatetrips";
            DateTime dtStoredDateTime = new DateTime();
            StringBuilder sbStopTimeUpdates = new StringBuilder();
            StringBuilder sbStopTimeEvents = new StringBuilder();
            StringBuilder[] aSBArray = new StringBuilder[2];
            int iStopTimeUpdateId = 0;
            int iArrivalEventId = 0;
            int iDepartureEventId = 0;
            string sModVehId = "";
            NpgsqlDataReader rsRead = null;

            try
            {
                

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        if (_TripUpdate.Vehicle == null)
                        {
                            sModVehId = "Misc_Veh_Id_" + _TripUpdate.Route;

                            if (_TripUpdate.TripId != "")
                            {
                                sModVehId += "_" + _TripUpdate.TripId;
                            }
                        }

                        cmdSQL.Parameters.AddWithValue("_trip_id", NpgsqlDbType.Varchar, (object)_TripUpdate.TripId ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_route_id", NpgsqlDbType.Varchar, _TripUpdate.Route);
                        cmdSQL.Parameters.AddWithValue("_direction_id", NpgsqlDbType.Integer, (object)_TripUpdate.DirectionId ?? 0);
                        cmdSQL.Parameters.AddWithValue("_vehicle_id", NpgsqlDbType.Varchar, (object)_TripUpdate.Vehicle ?? sModVehId);
                        cmdSQL.Parameters.AddWithValue("_start_date", NpgsqlDbType.Varchar, (object)_TripUpdate.StartDate ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _TripUpdate.LastUpdate);
                        cmdSQL.Parameters.AddWithValue("_delay", NpgsqlDbType.Integer, (object)_TripUpdate.Delay ?? 0);

                        cmdSQL.Parameters.AddWithValue("_stop_id", NpgsqlDbType.Varchar, DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_stop_sequence", NpgsqlDbType.Integer, 0);
                        cmdSQL.Parameters.AddWithValue("_arrival", NpgsqlDbType.Integer, 0);
                        cmdSQL.Parameters.AddWithValue("_departure", NpgsqlDbType.Integer, 0);

                        //cmdSQL.Parameters.AddWithValue("_stop_id", NpgsqlDbType.Varchar, (object)_StopTimeUpdate.Stop_Id);
                        //cmdSQL.Parameters.AddWithValue("_stop_sequence", NpgsqlDbType.Integer, (object)_StopTimeUpdate.Stop_Sequence);
                        //cmdSQL.Parameters.AddWithValue("_arrival", NpgsqlDbType.Integer, (object)_StopTimeUpdate.Arrival);
                        //cmdSQL.Parameters.AddWithValue("_departure", NpgsqlDbType.Integer, (object)_StopTimeUpdate.Departure);

                        //iStopTimeUpdateId = (int)cmdSQL.ExecuteScalar();
                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                iStopTimeUpdateId = Convert.ToInt32(rsRead[0]);
                                iArrivalEventId = Convert.ToInt32(rsRead[1]);
                                iDepartureEventId = Convert.ToInt32(rsRead[2]);
                            }
                        }
                    }

                    foreach (StopTimeUpdates _StopTimeUpdate in _TripUpdate.StopTimeUpdates)
                    {
                        iNextStopTimeEventId++;
                        iArrivalEventId = iNextStopTimeEventId;
                        iNextStopTimeEventId++;
                        iDepartureEventId = iNextStopTimeEventId;

                        
                        sbStopTimeUpdates.Append(iStopTimeUpdateId.ToString() + "," + _StopTimeUpdate.Stop_Sequence.ToString() + "," +
                            _StopTimeUpdate.Stop_Id.ToString() + "," + iArrivalEventId.ToString() + "," + iDepartureEventId.ToString() + 
                            Environment.NewLine);

                        sbStopTimeEvents.Append(iArrivalEventId.ToString() + "," + _StopTimeUpdate.ArrivalDelay.ToString() + "," +
                            _StopTimeUpdate.Arrival.ToString() + "," + _StopTimeUpdate.ArrivalUncertainty.ToString() + Environment.NewLine);

                        sbStopTimeEvents.Append(iDepartureEventId.ToString() + "," + _StopTimeUpdate.DepartureDelay.ToString() + "," +
                            _StopTimeUpdate.Departure.ToString() + "," + _StopTimeUpdate.DepartureUncertainty.ToString() + Environment.NewLine);
                    }
                }

                aSBArray[0] = sbStopTimeUpdates;
                aSBArray[1] = sbStopTimeEvents;
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in InsertTripUpdate().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);;
            }

            return aSBArray;
        }

        public static bool ProcessStopTimeUpdates(string sFullFileName, string sType, string sTableName)
        {
            bool bSuccess = true;
            string sFileName = "";
            string sErr = "";
            FileInfo fInfo = null;

            try
            {

                if (!File.Exists(sFullFileName))
                {
                    Transit_Consumer_Globals.LogFile.Log("File " + sFullFileName + " does not exist. No " + sType + " data to copy to prod. Exit returning false.");
                    return false;
                }

                fInfo = new FileInfo(sFullFileName);
                sFileName = fInfo.Name;

                Transit_Consumer_Globals.LogFile.Log("Copying " + sFileName + " data to " + sTableName + " using psql from batch file " + Transit_Consumer_Globals.sCopyRecordsBatFile);

                Process process = new Process();
                process.StartInfo.FileName = Transit_Consumer_Globals.sCopyRecordsBatFile;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.Arguments = String.Format(sType.ToLower() + "," + sFullFileName);
                process.Start();

                //* Read the output (or the error)
                string sOutput = process.StandardOutput.ReadToEnd();
                //GTFS_Data_Producer_Globals.LogFile.Log(sOutput);
                if ((sErr = process.StandardError.ReadToEnd()) != "")
                {
                    Transit_Consumer_Globals.LogFile.LogErrorText("Error executing batch file to insert data into table.");
                    Transit_Consumer_Globals.LogFile.LogErrorText(sErr);
                    bSuccess = false;
                }

                process.WaitForExit();

                Transit_Consumer_Globals.LogFile.Log("Finished copying " + sFileName + " into database.");
            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Failed in ProcessStopTimeUpdates().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
