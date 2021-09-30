using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Npgsql;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Link_Consumer
{
    class Link_Consumer_HandleLinkStatusResponse
    {
        private static string sPreparedStatement = "INSERT INTO db_PA_MOD.tbllinktrafficdata (id, agency_id, " +
            "speed_in_kph, travel_time_seconds, freeflow_in_kph, volume, occupancy, data_type, last_update) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);";

        public static void HandleLinkStatusResponse(List<string> lstLinkData)
        {

            int iUpdateCount = 0;
            List<Link_Consumer_DataStructures.Link_Consumer_LinkData> lstNoSQLData = new List<Link_Consumer_DataStructures.Link_Consumer_LinkData>();
            List<string> lstItem = new List<string>();

            Dictionary<string, string> dictSQLInsertLines = new Dictionary<string, string>();

            StringBuilder sbLinks = new StringBuilder();
            StringBuilder sbLinkData = new StringBuilder();
            Dictionary<string, StringBuilder> dictLinkStrings = new Dictionary<string, StringBuilder>();
            Dictionary<string, StringBuilder> dictLinkStringsHistoric = new Dictionary<string, StringBuilder>();

            Link_Consumer_DataStructures.Link_Consumer_LinkData _Link;
            Link_Consumer_DataStructures.Link_Consumer_StoredLinkData _StoredLinkData = new Link_Consumer_DataStructures.Link_Consumer_StoredLinkData();
            string sLinkOutFile = "LinksOut_" + DateTime.Now.Ticks.ToString() + ".txt";
            string sLinkOutFileFullName = "";
            string sLinkOutFileHistoric = "LinksOutHistoric_" + DateTime.Now.Ticks.ToString() + ".txt";
            string sLinkOutFileHistoricFullName = "";

            try
            {
              

                Link_Consumer_Globals.LogFile.Log("Received data in Link_Consumer_HandleLinkStatusResponse.HandleLinkStatusResponse(). " +
                    "Begin analysis of " + lstLinkData.Count + " link records.");

                dictLinkStrings.Clear();

                foreach (string sItem in lstLinkData)
                {
                    _StoredLinkData = new Link_Consumer_DataStructures.Link_Consumer_StoredLinkData();
                    _Link = new Link_Consumer_DataStructures.Link_Consumer_LinkData();
                    lstItem = sItem.Split(',').ToList();

                    try {

                        _Link.LinkID = Convert.ToInt64(lstItem[0].ToString());
                        _Link.AgencyID = lstItem[1].ToString().Trim();
                        _Link.Speed = Convert.ToInt32(lstItem[2].ToString());
                        _Link.TravelTime = Convert.ToDouble(lstItem[3].ToString());
                        _Link.Freeflow = Convert.ToInt32(lstItem[4].ToString());
                        _Link.Volume = Convert.ToInt32(lstItem[5].ToString());
                        _Link.Occupancy = Convert.ToInt32(lstItem[6].ToString());
                        _Link.LastUpdate = Convert.ToDateTime(lstItem[8].ToString());
                        _Link.DataType = lstItem[7].ToString();

                        if (Link_Consumer_Globals.dictLinkIDAndListData.ContainsKey(_Link.LinkID.ToString()))
                        {
                            _StoredLinkData = Link_Consumer_Globals.dictLinkIDAndListData[_Link.LinkID.ToString()];

                            if (Convert.ToDateTime(_Link.LastUpdate) > Convert.ToDateTime(_StoredLinkData.sLastUpdate))
                            {
                                //if link already in list, only keep newest for insert into RDBMS
                                sbLinkData.Append(_Link.LinkID.ToString() + "," + _Link.AgencyID + "," + _Link.Speed.ToString() + "," + _Link.TravelTime.ToString() +
                                    "," + _Link.Freeflow + "," + _Link.Volume + "," + _Link.Occupancy + ",,'" + _Link.LastUpdate.ToString() + "'");

                                _StoredLinkData.AgencyID = _Link.AgencyID;
                                _StoredLinkData.sLastUpdate = Convert.ToDateTime(_Link.LastUpdate).ToString();
                                _StoredLinkData.sLinkData = sbLinkData.ToString();
                                _StoredLinkData.bDoUpdate = true;

                                //add to storage for processing
                                Link_Consumer_Globals.dictLinkIDAndListData[_Link.LinkID.ToString()] = _StoredLinkData;
                            }
                            else
                            {
                                //not newer, make sure to set DoUpdate to false
                                _StoredLinkData.bDoUpdate = false;
                                Link_Consumer_Globals.dictLinkIDAndListData[_Link.LinkID.ToString()] = _StoredLinkData;
                            }
                        }
                        else
                        {
                            //link data is new. add to stored data for insert
                            sbLinkData.Append(_Link.LinkID.ToString() + "," + _Link.AgencyID + "," + _Link.Speed.ToString() + "," + _Link.TravelTime.ToString() +
                                "," + _Link.Freeflow + "," + _Link.Volume + "," + _Link.Occupancy + ",,'" + _Link.LastUpdate.ToString() + "'");

                            _StoredLinkData.sLinkID = _Link.LinkID.ToString();
                            _StoredLinkData.AgencyID = _Link.AgencyID;
                            _StoredLinkData.sLastUpdate = _Link.LastUpdate.ToString();
                            _StoredLinkData.sLinkData = sbLinkData.ToString();
                            _StoredLinkData.bDoUpdate = true;

                            //add to storage for processing
                            Link_Consumer_Globals.dictLinkIDAndListData.Add(_StoredLinkData.sLinkID.ToString(), _StoredLinkData);
                        }

                        if (Link_Consumer_Globals.hshLinksToArchive.Contains(_Link.LinkID.ToString()) && _StoredLinkData.bDoUpdate == true)
                        {
                            //no need to send the data to NoSQL if it is not newer than what we already have.
                            lstNoSQLData.Add(_Link);
                        }

                        iUpdateCount++;
                        _StoredLinkData = null;
                        _Link = null;

                        sbLinkData.Remove(0, sbLinkData.Length);
                    }
                    catch (Exception ex1)
                    {
                        Link_Consumer_Globals.LogFile.LogErrorText("Error processing individual link in Link_Consumer_HandleLinkStatusResponse.HandleLinkStatusResponse()");
                        Link_Consumer_Globals.LogFile.LogError(ex1);
                    }

                }

                foreach (KeyValuePair<string, Link_Consumer_DataStructures.Link_Consumer_StoredLinkData> dictItem in Link_Consumer_Globals.dictLinkIDAndListData)
                {
                    //now append the links to the list for insert into RDMBS
                    if (dictItem.Value.sLinkData != "" && dictItem.Value.bDoUpdate)
                    {

                        if (!dictLinkStrings.ContainsKey(dictItem.Value.AgencyID))
                        {
                            dictLinkStrings.Add(dictItem.Value.AgencyID, new StringBuilder());
                        }

                        if (!dictLinkStringsHistoric.ContainsKey(dictItem.Value.AgencyID))
                        {
                            dictLinkStringsHistoric.Add(dictItem.Value.AgencyID, new StringBuilder());
                        }


                        sLinkOutFileFullName = dictItem.Value.AgencyID + "_" + sLinkOutFile;
                        sLinkOutFileHistoricFullName = dictItem.Value.AgencyID + "_" + sLinkOutFileHistoric;

                        dictLinkStrings[dictItem.Value.AgencyID].Append(dictItem.Value.sLinkData + "," + dictItem.Value.AgencyID + "_" + sLinkOutFile + "," + Link_Consumer_Globals.iLinkInsertStatusIndicator.ToString() + Environment.NewLine);

                        if (Link_Consumer_Globals.hshLinksToArchive.Contains(dictItem.Value.sLinkID))
                        {
                            //only archive links that are in the designated polygon
                            dictLinkStringsHistoric[dictItem.Value.AgencyID].Append(dictItem.Value.sLinkData + Environment.NewLine);
                        }
                    }

                    Link_Consumer_Globals.dictLinkIDAndListData[dictItem.Key].bDoUpdate = false;
                }

                if (dictLinkStrings.Count > 0)
                {

                    Link_Consumer_Globals.LogFile.Log("Successfully dumped " + iUpdateCount.ToString() + " links to text file(s). Begin to store the data.");

                    foreach (KeyValuePair<string, StringBuilder> kvpLinkSBs in dictLinkStrings)
                    {
                        using (StreamWriter writer = new StreamWriter(Link_Consumer_Globals.sLinkOutDirectory + "\\" + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileFullName, false))
                        {
                            writer.Write(kvpLinkSBs.Value.ToString());
                            kvpLinkSBs.Value.Remove(0, sbLinks.Length);
                        }

                        if (DoBulkCopyToTemp(Link_Consumer_Globals.sLinkOutDirectory + "\\" + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileFullName))
                        {
                            Link_Consumer_Globals.LogFile.Log("Successfully inserted links into temp table in DoBulkCopyToTemp().");
                            if (DoBulkCopyToProd(kvpLinkSBs.Key.ToString() + "_" + sLinkOutFile))
                            {
                                Link_Consumer_Globals.LogFile.Log("Successfully copied or inserted links into prod table from " + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileFullName + ". Done.");
                            }
                            if (Link_Consumer_Globals.DoGZIP(Link_Consumer_Globals.sLinkOutDirectory + "\\" + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileFullName))
                            {
                                Link_Consumer_Globals.LogFile.Log("Successfully GZipped link text file.");
                            }
                        }

                    }

                    foreach (KeyValuePair<string, StringBuilder> kvpLinkSBs in dictLinkStringsHistoric)
                    {
                        using (StreamWriter writer = new StreamWriter(Link_Consumer_Globals.sLinkOutDirectory + "\\" + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileHistoricFullName, false))
                        {
                            writer.Write(kvpLinkSBs.Value.ToString());
                            kvpLinkSBs.Value.Remove(0, sbLinks.Length);
                        }

                        if (DoBulkCopyToProdHistoric(kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileHistoricFullName))
                        {
                            Link_Consumer_Globals.LogFile.Log("Successfully copied or inserted links into prod historic table from " + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileHistoricFullName + ". Done.");

                            if (File.Exists(Link_Consumer_Globals.sLinkOutDirectory + "\\" + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileHistoricFullName))
                            {
                                File.Delete(Link_Consumer_Globals.sLinkOutDirectory + "\\" + kvpLinkSBs.Key.ToString() + "_" + sLinkOutFileHistoricFullName);
                                Link_Consumer_Globals.LogFile.Log("Successfully deleted temporary historic link output file.");
                            }
                        }

                        
                    }

                }
                else
                {
                    Link_Consumer_Globals.LogFile.Log("No new link data to process into tbllinktrafficdata.");
                }

                if (lstNoSQLData.Count > 0 && Link_Consumer_Globals.bSendToMongoDB)
                {
                    Link_Consumer_MongoCode.DoBatchUpdate(lstNoSQLData);
                }
            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_HandleLinkStatusResponse.HandleLinkStatusResponse()");
                Link_Consumer_Globals.LogFile.LogError(ex);
            }
            finally
            {
                //Link_Consumer_Globals.LogFile.Log("Resetting link status data timer.");
                //Link_Consumer_Globals.tmGetLinks.Start();
                lstLinkData.Clear();
            }

            
        }

        private static bool DoBulkCopyToTemp(string sFileName)
        {
            bool bSuccess = true;
            string sErr = "";

            try
            {

                if (!File.Exists(sFileName))
                {
                    Link_Consumer_Globals.LogFile.Log("File " + sFileName + " does not exist. No links to copy to temp or prod. Exit returning false.");
                    return false;
                }

          
                Link_Consumer_Globals.LogFile.Log("Copying links into temp table using psql from batch file " + Link_Consumer_Globals.sCopyLinkBatFile);

                Process process = new Process();
                process.StartInfo.FileName = Link_Consumer_Globals.sCopyLinkBatFile;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.Arguments = String.Format("insert," + sFileName);
                process.Start();

                //* Read the output (or the error)
                string sOutput = process.StandardOutput.ReadToEnd();
                //Link_Consumer_Globals.LogFile.Log(sOutput);
                if ((sErr = process.StandardError.ReadToEnd()) != "")
                {
                    Link_Consumer_Globals.LogFile.LogErrorText("Error executing batch file to insert links into temp table.");
                    Link_Consumer_Globals.LogFile.LogErrorText(sErr);
                    bSuccess = false;
                }

                process.WaitForExit();

            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Failed in Link_Consumer_HandleLinkStatusResponse.DoBulkCopyToTemp(). See error logs for details.");
                Link_Consumer_Globals.LogFile.LogError(ex);
                bSuccess = false;
            }

            return bSuccess;
        }

        private static bool DoBulkCopyToProd(string sLinkOutFile)
        {
            bool bSuccess = false;

            string sSQL = "SELECT db_objects.bulkupdatelinks('" + sLinkOutFile + "');";

            try
            {
                Link_Consumer_Globals.LogFile.Log("Updating or copying links into prod table from file " + sLinkOutFile + ".");

                using (NpgsqlConnection sConn = new NpgsqlConnection(Link_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Link_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.ExecuteNonQuery();
                    }
                }
                bSuccess = true;
            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Failed in Link_Consumer_HandleLinkStatusResponse.DoBulkCopyToProd(). See error logs for details.");
                Link_Consumer_Globals.LogFile.LogError(ex);
                bSuccess = false;
            }

            return bSuccess;
        }

        private static bool DoBulkCopyToProdHistoric(string sFileName)
        {
            bool bSuccess = true;
            string sErr = "";
            string sAgencyID = "";
            string sLinkOutFileFullName = Link_Consumer_Globals.sLinkOutDirectory + "\\" + sFileName;

            try
            {
                //get agency ID from first part of filename string 
                sAgencyID = sFileName.Split(new char[] { '_' }, StringSplitOptions.None)[0];

                if (!File.Exists(sLinkOutFileFullName))
                {
                    Link_Consumer_Globals.LogFile.Log("File " + sLinkOutFileFullName + " does not exist. No links to copy to temp or prod. Exit returning false.");
                    return false;
                }


                Link_Consumer_Globals.LogFile.Log("Copying links into temp table using psql from batch file " + Link_Consumer_Globals.sCopyLinkBatFile);

                Process process = new Process();
                process.StartInfo.FileName = Link_Consumer_Globals.sCopyLinkBatFile;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.Arguments = String.Format("insert_historic," + sLinkOutFileFullName + "," + sAgencyID);
                process.Start();

                //* Read the output (or the error)
                string sOutput = process.StandardOutput.ReadToEnd();
                //Link_Consumer_Globals.LogFile.Log(sOutput);
                if ((sErr = process.StandardError.ReadToEnd()) != "")
                {
                    Link_Consumer_Globals.LogFile.LogErrorText("Error executing batch file to insert links into historic table.");
                    Link_Consumer_Globals.LogFile.LogErrorText(sErr);
                    bSuccess = false;
                }

                process.WaitForExit();

            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Failed in Link_Consumer_HandleLinkStatusResponse.DoBulkCopyToTemp(). See error logs for details.");
                Link_Consumer_Globals.LogFile.LogError(ex);
                bSuccess = false;
            }

            return bSuccess;
        }

    }
}
