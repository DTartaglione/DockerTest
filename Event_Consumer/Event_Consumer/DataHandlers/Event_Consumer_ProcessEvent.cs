using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Event_Consumer
{
    class Event_Consumer_ProcessEvent
    {
        public bool InsertEvent(EventData _Event)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_insertevent";
            StoredEventData _StoredEvent = new StoredEventData();

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_event_id", NpgsqlDbType.Varchar, _Event.sLocalEventID);
                        cmdSQL.Parameters.AddWithValue("_source_event_id", NpgsqlDbType.Varchar, _Event.sSourceEventID);
                        cmdSQL.Parameters.AddWithValue("_owner_org_id", NpgsqlDbType.Bigint, _Event.iAgencyID);
                        cmdSQL.Parameters.AddWithValue("_event_status", NpgsqlDbType.Bigint, _Event.iEventStatus);
                        cmdSQL.Parameters.AddWithValue("_event_category_id", NpgsqlDbType.Integer, _Event.iEventCategoryID);
                        cmdSQL.Parameters.AddWithValue("_event_category", NpgsqlDbType.Varchar, _Event.sEventCategory);
                        cmdSQL.Parameters.AddWithValue("_event_type", NpgsqlDbType.Varchar, _Event.sEventTypes);
                        cmdSQL.Parameters.AddWithValue("_create_time", NpgsqlDbType.Timestamp, _Event.sCreateTime);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _Event.sLastUpdate);
                        cmdSQL.Parameters.AddWithValue("_facility_id", NpgsqlDbType.Bigint, _Event.iFacilityID);
                        cmdSQL.Parameters.AddWithValue("_facility_name", NpgsqlDbType.Varchar, _Event.sFacilityName);
                        cmdSQL.Parameters.AddWithValue("_event_direction", NpgsqlDbType.Bigint, Convert.ToInt16(_Event.alDirection[0]));
                        cmdSQL.Parameters.AddWithValue("_article", NpgsqlDbType.Varchar, _Event.sArticle);
                        cmdSQL.Parameters.AddWithValue("_from_node_id", NpgsqlDbType.Bigint, _Event.iFromNodeID);
                        cmdSQL.Parameters.AddWithValue("_from_node_name", NpgsqlDbType.Varchar, _Event.sFromNodeName);
                        cmdSQL.Parameters.AddWithValue("_to_node_id", NpgsqlDbType.Bigint, _Event.iToNodeID);
                        cmdSQL.Parameters.AddWithValue("_to_node_name", NpgsqlDbType.Varchar, _Event.sToNodeName);
                        cmdSQL.Parameters.AddWithValue("_event_description", NpgsqlDbType.Varchar, _Event.sEventDescription);
                        cmdSQL.Parameters.AddWithValue("_event_severity", NpgsqlDbType.Bigint, _Event.iSeverity);
                        cmdSQL.Parameters.AddWithValue("_from_latitude", NpgsqlDbType.Double, _Event.dFromLat);
                        cmdSQL.Parameters.AddWithValue("_from_longitude", NpgsqlDbType.Double, _Event.dFromLon);
                        cmdSQL.Parameters.AddWithValue("_to_latitude", NpgsqlDbType.Double, _Event.dToLat);
                        cmdSQL.Parameters.AddWithValue("_to_longitude", NpgsqlDbType.Double, _Event.dToLon);
                        cmdSQL.Parameters.AddWithValue("_estimated_duration", NpgsqlDbType.Bigint, _Event.iEstimatedDuration);
                        cmdSQL.Parameters.AddWithValue("_extra_info", NpgsqlDbType.Varchar, _Event.sExtraInfo);
                        cmdSQL.Parameters.AddWithValue("_city", NpgsqlDbType.Varchar, _Event.sCity);
                        cmdSQL.Parameters.AddWithValue("_county", NpgsqlDbType.Varchar, _Event.sCounty);
                        cmdSQL.Parameters.AddWithValue("_state", NpgsqlDbType.Varchar, _Event.sState);
                        cmdSQL.Parameters.AddWithValue("_num_affected_lanes", NpgsqlDbType.Bigint, _Event.iNumAffectedLanes);
                        cmdSQL.Parameters.AddWithValue("_num_total_lanes", NpgsqlDbType.Bigint, _Event.iNumTotalLanes);
                        cmdSQL.Parameters.AddWithValue("_lane_type", NpgsqlDbType.Varchar, _Event.sLaneType);
                        cmdSQL.Parameters.AddWithValue("_lane_status", NpgsqlDbType.Varchar, _Event.sLaneStatus);
                        cmdSQL.Parameters.AddWithValue("_start_date_time", NpgsqlDbType.Timestamp, _Event.sStartDateTime);
                        //cmdSQL.Parameters.AddWithValue("_end_date_time", NpgsqlDbType.Timestamp, (object)_Event.sEndDateTime ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_facility_name_alt", NpgsqlDbType.Varchar, (object)_Event.FacilityNameAlt ?? DBNull.Value);
                        if (_Event.sEndDateTime != "")
                        {
                            cmdSQL.Parameters.AddWithValue("_end_date_time", NpgsqlDbType.Timestamp, _Event.sEndDateTime);
                        }
                        else
                        {
                            cmdSQL.Parameters.Add("_end_date_time", NpgsqlDbType.Timestamp).Value = DBNull.Value;
                        }


                        cmdSQL.ExecuteNonQuery();

                        _StoredEvent.sLocalEventID = _Event.sLocalEventID;
                        _StoredEvent.sSourceEventID = _Event.sSourceEventID;
                        _StoredEvent.iAgencyID = _Event.iAgencyID;
                        _StoredEvent.iEventStatus = _Event.iEventStatus;
                        _StoredEvent.dtLastUpdate = Convert.ToDateTime(_Event.sLastUpdate);

                        //Add to event ID map
                        Event_Consumer_Globals.dictEventIDMap.Add(_Event.iAgencyID + "|" + _Event.sSourceEventID, _StoredEvent);

                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in InsertEvent().");
                Event_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Event_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }

        public bool UpdateEvent(EventData _Event)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_updateevent";
            StoredEventData _StoredEvent = new StoredEventData();

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_event_id", NpgsqlDbType.Varchar, _Event.sLocalEventID);
                        cmdSQL.Parameters.AddWithValue("_source_event_id", NpgsqlDbType.Varchar, _Event.sSourceEventID);
                        cmdSQL.Parameters.AddWithValue("_owner_org_id", NpgsqlDbType.Bigint, _Event.iAgencyID);
                        cmdSQL.Parameters.AddWithValue("_event_status", NpgsqlDbType.Bigint, _Event.iEventStatus);
                        cmdSQL.Parameters.AddWithValue("_event_category_id", NpgsqlDbType.Integer, _Event.iEventCategoryID);
                        cmdSQL.Parameters.AddWithValue("_event_category", NpgsqlDbType.Varchar, _Event.sEventCategory);
                        cmdSQL.Parameters.AddWithValue("_event_type", NpgsqlDbType.Varchar, _Event.sEventTypes);
                        //cmdSQL.Parameters.AddWithValue("_create_time", NpgsqlDbType.Timestamp, _Event.sCreateTime);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _Event.sLastUpdate);
                        cmdSQL.Parameters.AddWithValue("_facility_id", NpgsqlDbType.Bigint, _Event.iFacilityID);
                        cmdSQL.Parameters.AddWithValue("_facility_name", NpgsqlDbType.Varchar, _Event.sFacilityName);
                        cmdSQL.Parameters.AddWithValue("_event_direction", NpgsqlDbType.Bigint, Convert.ToInt16(_Event.alDirection[0]));
                        cmdSQL.Parameters.AddWithValue("_article", NpgsqlDbType.Varchar, _Event.sArticle);
                        cmdSQL.Parameters.AddWithValue("_from_node_id", NpgsqlDbType.Bigint, _Event.iFromNodeID);
                        cmdSQL.Parameters.AddWithValue("_from_node_name", NpgsqlDbType.Varchar, _Event.sFromNodeName);
                        cmdSQL.Parameters.AddWithValue("_to_node_id", NpgsqlDbType.Bigint, _Event.iToNodeID);
                        cmdSQL.Parameters.AddWithValue("_to_node_name", NpgsqlDbType.Varchar, _Event.sToNodeName);
                        cmdSQL.Parameters.AddWithValue("_event_description", NpgsqlDbType.Varchar, _Event.sEventDescription);
                        cmdSQL.Parameters.AddWithValue("_event_severity", NpgsqlDbType.Bigint, _Event.iSeverity);
                        cmdSQL.Parameters.AddWithValue("_from_latitude", NpgsqlDbType.Double, _Event.dFromLat);
                        cmdSQL.Parameters.AddWithValue("_from_longitude", NpgsqlDbType.Double, _Event.dFromLon);
                        cmdSQL.Parameters.AddWithValue("_to_latitude", NpgsqlDbType.Double, _Event.dToLat);
                        cmdSQL.Parameters.AddWithValue("_to_longitude", NpgsqlDbType.Double, _Event.dToLon);
                        cmdSQL.Parameters.AddWithValue("_estimated_duration", NpgsqlDbType.Bigint, _Event.iEstimatedDuration);
                        cmdSQL.Parameters.AddWithValue("_extra_info", NpgsqlDbType.Varchar, _Event.sExtraInfo);
                        cmdSQL.Parameters.AddWithValue("_city", NpgsqlDbType.Varchar, _Event.sCity);
                        cmdSQL.Parameters.AddWithValue("_county", NpgsqlDbType.Varchar, _Event.sCounty);
                        cmdSQL.Parameters.AddWithValue("_state", NpgsqlDbType.Varchar, _Event.sState);
                        cmdSQL.Parameters.AddWithValue("_num_affected_lanes", NpgsqlDbType.Bigint, _Event.iNumAffectedLanes);
                        cmdSQL.Parameters.AddWithValue("_num_total_lanes", NpgsqlDbType.Bigint, _Event.iNumTotalLanes);
                        cmdSQL.Parameters.AddWithValue("_lane_type", NpgsqlDbType.Varchar, _Event.sLaneType);
                        cmdSQL.Parameters.AddWithValue("_lane_status", NpgsqlDbType.Varchar, _Event.sLaneStatus);
                        cmdSQL.Parameters.AddWithValue("_start_date_time", NpgsqlDbType.Timestamp, _Event.sStartDateTime);
                        cmdSQL.Parameters.AddWithValue("_facility_name_alt", NpgsqlDbType.Varchar, (object)_Event.FacilityNameAlt ?? DBNull.Value);

                        if (_Event.sEndDateTime != "")
                        {
                            cmdSQL.Parameters.AddWithValue("_end_date_time", NpgsqlDbType.Timestamp, _Event.sEndDateTime);
                        }
                        else
                        {
                            cmdSQL.Parameters.Add("_end_date_time", NpgsqlDbType.Timestamp).Value = DBNull.Value;
                        }
                      

                        cmdSQL.ExecuteNonQuery();

                        if (Event_Consumer_Globals.dictEventIDMap.ContainsKey(_Event.iAgencyID + "|" + _Event.sSourceEventID.ToString()))
                        {
                            Event_Consumer_Globals.dictEventIDMap.Remove(_Event.iAgencyID + "|" + _Event.sSourceEventID.ToString());
                        }

                        _StoredEvent.sLocalEventID = _Event.sLocalEventID;
                        _StoredEvent.sSourceEventID = _Event.sSourceEventID;
                        _StoredEvent.iAgencyID = _Event.iAgencyID;
                        _StoredEvent.iEventStatus = _Event.iEventStatus;
                        _StoredEvent.dtLastUpdate = Convert.ToDateTime(_Event.sLastUpdate);

                        //Add to event ID map
                        Event_Consumer_Globals.dictEventIDMap.Add(_Event.iAgencyID + "|" + _Event.sSourceEventID.ToString(), _StoredEvent);

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in sp_MOD_DI_updateevent()");
                Event_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        public bool CloseEvent(string sSourceEventID, string sLocalEventID, int iAgencyID, string sCloseTime)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_closeevent";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Event_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Event_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_event_id", NpgsqlDbType.Varchar, sLocalEventID);

                        if (sCloseTime != "")
                        {
                            cmdSQL.Parameters.AddWithValue("_close_time", NpgsqlDbType.Timestamp, Convert.ToDateTime(sCloseTime));
                        }
                        else
                        {
                            cmdSQL.Parameters.Add("_close_time", NpgsqlDbType.Timestamp).Value = DBNull.Value;
                        }

                        cmdSQL.ExecuteNonQuery();

                        Event_Consumer_Globals.dictEventIDMap.Remove(iAgencyID.ToString() + "|" + sSourceEventID.ToString());

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error in CloseEvent()");
                Event_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Event_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

    }
}
