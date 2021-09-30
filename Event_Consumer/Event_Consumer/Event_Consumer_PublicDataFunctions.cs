using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Event_Consumer
{
    class Event_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            if (GetAgencyInfo())
            {
                if (GetEventClassLookupData())
                {
                    if (GetFacilityData())
                    {
                        if (GetNodeData())
                        {
                            if (GetDirectionData())
                            {
                                if (GetArticleData())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                
            }
            return false;
        }

        public static bool GetCurrentDynamicData()
        {
            if (GetCurrentEventData())
            {
                return true;
            }
            return false;
        }

        public static bool GetCurrentEventData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select _event_id, _source_event_id, _agency_id, _last_update _last_update, _event_status FROM db_objects.sp_MOD_DI_getopenevents(); ";
            StoredEventData _Event = new StoredEventData();

            try
            {
                Event_Consumer_Globals.dictEventIDMap.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                if (!Event_Consumer_Globals.dictEventIDMap.ContainsKey(rsRead[2].ToString() + "|" + rsRead[1].ToString()))
                                {
                                    _Event = new StoredEventData();

                                    _Event.sLocalEventID = rsRead[0].ToString();
                                    _Event.sSourceEventID = rsRead[1].ToString();
                                    _Event.iAgencyID = Convert.ToInt32(rsRead[2].ToString());
                                    _Event.dtLastUpdate = Convert.ToDateTime(rsRead[3].ToString());
                                    _Event.iEventStatus = Convert.ToInt32(rsRead[4].ToString());

                                    Event_Consumer_Globals.dictEventIDMap.Add(rsRead[2].ToString() + "|" + rsRead[1].ToString(), _Event);
                                    _Event = null;
                                }
                            }
                        }
                    }

                }

                Event_Consumer_Globals.LogFile.Log("Successfully read event data in GetCurrentEventData().");
                bSuccess = true;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_PublicDataFunctions.GetCurrentEventData()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                Event_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!Event_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    Event_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            //if ((Event_Consumer_Globals.iAgencyID = Event_Consumer_Globals.SetAgencyID(Event_Consumer_Globals.sAgencyName)) == 0)
                            //{
                            //    Event_Consumer_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + Event_Consumer_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                            //    return false;
                            //}
                        }
                    }

                }

                Event_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in GetAgencyInfo()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetEventClassLookupData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select * from db_objects.sp_MOD_DI_geteventclasslookup();";
            EventClassLookup _EventClass = new EventClassLookup();

            try
            {
                Event_Consumer_Globals.dictEventClassLookup.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                
                                if (!Event_Consumer_Globals.dictEventClassLookup.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    _EventClass = new EventClassLookup();
                                    _EventClass.iEventClassID = Convert.ToInt32(rsRead[0].ToString());
                                    _EventClass.iAgencyID = Convert.ToInt32(rsRead[1].ToString());
                                    _EventClass.sSourceEventClass = rsRead[2].ToString();
                                    _EventClass.sSourceEventClassName = rsRead[3].ToString();
                                    _EventClass.iLocalEventClass = Convert.ToInt32(rsRead[4].ToString());
                                    _EventClass.sLocalEventClassName = rsRead[5].ToString();
                                    _EventClass.bIsTransit = Convert.ToBoolean(rsRead[6]);
                                    _EventClass.bIsPlanned = Convert.ToBoolean(rsRead[7]);

                                    Event_Consumer_Globals.dictEventClassLookup.Add(Convert.ToInt32(rsRead[0].ToString()), _EventClass);
                                }
                            }
                        }
                    }

                }

                Event_Consumer_Globals.LogFile.Log("Successfully read event data in GetEventClassLookupData().");
                bSuccess = true;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_PublicDataFunctions.GetEventClassLookupData()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetFacilityData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT DISTINCT _local_facility_id, _local_facility_name, _source_facility_id, _source_facility_name, _direction, _agency_id " +
                "from db_objects.sp_MOD_DI_getfacilitynodedata(); ";
            FacilityLookup _Facility = new FacilityLookup();

            try
            {
                Event_Consumer_Globals.dictFacilityData.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                _Facility = new FacilityLookup();

                                if (!Event_Consumer_Globals.dictFacilityData.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    _Facility.iLocalFacilityID = Convert.ToInt32(rsRead[0].ToString());
                                    _Facility.sLocalFacilityName = rsRead[1].ToString();
                                    _Facility.sSourceFacilityID = rsRead[2].ToString();
                                    _Facility.sSourceFacilityName = rsRead[3].ToString();
                                    _Facility.iDirection = Convert.ToInt32(rsRead[4].ToString());
                                    _Facility.iAgencyID = Convert.ToInt32(string.IsNullOrEmpty(rsRead[5].ToString()) ? "0" : rsRead[5].ToString());

                                    Event_Consumer_Globals.dictFacilityData.Add(Convert.ToInt32(rsRead[0].ToString()), _Facility);
                                }
                            }
                        }
                    }

                }

                Event_Consumer_Globals.LogFile.Log("Successfully read facility data in GetFacilityData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_PublicDataFunctions.GetFacilityData()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }
        public static bool GetNodeData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT DISTINCT _local_node_id, _local_facility_id, _local_node_name, _source_node_id, _source_node_name, _direction, _agency_id " +
                "from db_objects.sp_MOD_DI_getfacilitynodedata(); ";
            NodeLookup _Node = new NodeLookup();

            try
            {
                Event_Consumer_Globals.dictFacilityData.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                _Node = new NodeLookup();

                                if (string.IsNullOrEmpty(rsRead[0].ToString()))
                                {
                                    continue;
                                }


                                if (!Event_Consumer_Globals.dictNodeData.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    _Node.iLocalNodeID = Convert.ToInt32(rsRead[0].ToString());
                                    _Node.iLocalFacilityID = Convert.ToInt32(rsRead[1].ToString());
                                    _Node.sLocalNodeName = rsRead[2].ToString();
                                    _Node.sSourceNodeID = rsRead[3].ToString();
                                    _Node.sSourceNodeName = rsRead[4].ToString();
                                    _Node.iDirection = Convert.ToInt32(rsRead[5].ToString());
                                    _Node.iAgencyID = Convert.ToInt32(string.IsNullOrEmpty(rsRead[6].ToString()) ? "0" : rsRead[6].ToString());

                                    Event_Consumer_Globals.dictNodeData.Add(Convert.ToInt32(rsRead[0].ToString()), _Node);
                                }
                            }
                        }
                    }

                }

                Event_Consumer_Globals.LogFile.Log("Successfully read node data in GetNodeData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_PublicDataFunctions.GetNodeData()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetDirectionData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select * from db_objects.sp_MOD_DI_getdirectionlookup();";
            DirectionLookup _Direction = new DirectionLookup();

            try
            {
                Event_Consumer_Globals.dictDirectionData.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                _Direction = new DirectionLookup();

                                if (!Event_Consumer_Globals.dictDirectionData.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    _Direction.iLocalDirectionID = Convert.ToInt32(rsRead[1].ToString());
                                    _Direction.sSourceDirection = rsRead[3].ToString();
                                    _Direction.iAgencyID = Convert.ToInt32(rsRead[2].ToString());
                                    _Direction.sLocalDirection = rsRead[5].ToString();

                                    Event_Consumer_Globals.dictDirectionData.Add(Convert.ToInt32(rsRead[0].ToString()), _Direction);
                                }
                            }
                        }
                    }

                }

                Event_Consumer_Globals.LogFile.Log("Successfully read direction data in GetDirectionData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_PublicDataFunctions.GetDirectionData()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetArticleData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select * from db_objects.sp_MOD_DI_getarticlelookup();";
            ArticleLookup _Article = new ArticleLookup();

            try
            {
                Event_Consumer_Globals.dictArticleData.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                _Article = new ArticleLookup();

                                if (!Event_Consumer_Globals.dictArticleData.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    _Article.iLocalArticleID = Convert.ToInt32(rsRead[1].ToString());
                                    _Article.iAgencyID = Convert.ToInt32(rsRead[2].ToString());
                                    _Article.sSourceArticle = rsRead[3].ToString();
                                    _Article.sLocalArticle = rsRead[4].ToString();

                                    Event_Consumer_Globals.dictArticleData.Add(Convert.ToInt32(rsRead[0].ToString()), _Article);
                                }
                            }
                        }
                    }

                }

                Event_Consumer_Globals.LogFile.Log("Successfully read article data in GetArticleData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in Event_Consumer_PublicDataFunctions.GetArticleData()");
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }
    }
}
