using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Event_Consumer
{
    class EventData
    {
        private string _LocalEventID = "";
        private string _SourceEventID = "";
        private int _AgencyID = 0;
        private int _EventCategoryID = 0;
        private string _EventCategory = "";
        private int _EventStatus = 0;
        private string _EventTypes = "";
        private string _CreateTime = "";
        private string _LastUpdate = "";
        private string _CloseTime = "";
        private int _FacilityID = 0;
        private string _FacilityName = "";
        private string _FacilityNameAlt = "";
        private ArrayList _Direction = new ArrayList();
        private string _Article = "";
        private int _FromNodeID = 0;
        private string _FromNodeName = "";
        private int _ToNodeID = 0;
        private string _ToNodeName = "";
        private string _EventDescription = "";
        private int _Severity = 0;
        private double _FromLat = 0;
        private double _FromLon = 0;
        private double _ToLat = 0;
        private double _ToLon = 0;
        private int _EstimatedDuration = 0;
        private string _ExtraInfo = "";
        private string _City = "";
        private string _County = "";
        private string _State = "";
        private int _NumAffectedLanes = 0;
        private int _NumTotalLanes = 0;
        private string _LaneType = "";
        private string _LaneStatus = "";
        private string _StartDateTime = "";
        private string _EndDateTime = "";

        public string sLocalEventID
        {
            get { return _LocalEventID; }
            set { _LocalEventID = value; }
        }

        public string sSourceEventID
        {
            get { return _SourceEventID; }
            set { _SourceEventID = value; }
        }

        public int iAgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string sEventCategory
        {
            get { return _EventCategory; }
            set { _EventCategory = value; }
        }

        public int iEventCategoryID
        {
            get { return _EventCategoryID; }
            set { _EventCategoryID = value; }
        }

        public int iEventStatus
        {
            get { return _EventStatus; }
            set { _EventStatus = value; }
        }

        public string sEventTypes
        {
            get { return _EventTypes; }
            set { _EventTypes = value; }
        }

        public string sCreateTime
        {
            get { return _CreateTime; }
            set { _CreateTime = value; }
        }
        public string sLastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

        public string CloseTime
        {
            get { return _CloseTime; }
            set { _CloseTime = value; }
        }

        public int iFacilityID
        {
            get { return _FacilityID; }
            set { _FacilityID = value; }
        }

        public string sFacilityName
        {
            get { return _FacilityName; }
            set { _FacilityName = value; }
        }

        public string FacilityNameAlt
        {
            get { return _FacilityNameAlt; }
            set { _FacilityNameAlt = value; }
        }

        public ArrayList alDirection
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public string sArticle
        {
            get { return _Article; }
            set { _Article = value; }
        }

        public int iFromNodeID
        {
            get { return _FromNodeID; }
            set { _FromNodeID = value; }
        }

        public string sFromNodeName
        {
            get { return _FromNodeName; }
            set { _FromNodeName = value; }
        }

        public int iToNodeID
        {
            get { return _ToNodeID; }
            set { _ToNodeID = value; }
        }

        public string sToNodeName
        {
            get { return _ToNodeName; }
            set { _ToNodeName = value; }
        }

        public string sEventDescription
        {
            get { return _EventDescription; }
            set { _EventDescription = value; }
        }

        public int iSeverity
        {
            get { return _Severity; }
            set { _Severity = value; }
        }

        public double dFromLat 
        {
            get { return _FromLat; }
            set { _FromLat = value; }
        }

        public double dFromLon
        {
            get { return _FromLon; }
            set { _FromLon = value; }
        }

        public double dToLat
        {
            get { return _ToLat; }
            set { _ToLat = value; }
        }

        public double dToLon
        {
            get { return _ToLon; }
            set { _ToLon = value; }
        }

        public int iEstimatedDuration
        {
            get { return _EstimatedDuration; }
            set { _EstimatedDuration = value; }
        }

        public string sExtraInfo
        {
            get { return _ExtraInfo; }
            set { _ExtraInfo = value; }
        }

        public string sCity
        {
            get { return _City; }
            set { _City = value; }
        }

        public string sCounty
        {
            get { return _County; }
            set { _County = value; }
        }

        public string sState
        {
            get { return _State; }
            set { _State = value; }
        }

        public int iNumAffectedLanes
        {
            get { return _NumAffectedLanes; }
            set { _NumAffectedLanes = value; }
        }

        public int iNumTotalLanes
        {
            get { return _NumTotalLanes; }
            set { _NumTotalLanes = value; }
        }

        public string sLaneType
        {
            get { return _LaneType; }
            set { _LaneType = value; }
        }

        public string sLaneStatus
        {
            get { return _LaneStatus; }
            set { _LaneStatus = value; }
        }

        public string sStartDateTime
        {
            get { return _StartDateTime; }
            set { _StartDateTime = value; }
        }

        public string sEndDateTime
        {
            get { return _EndDateTime; }
            set { _EndDateTime = value; }
        }
    }


    class StoredEventData
    {
        private string _LocalEventID = "";
        private string _SourceEventID = "";
        private int _AgencyID = 0;
        private int _EventStatus = 0;
        private DateTime _LastUpdate;

        public string sLocalEventID
        {
            get { return _LocalEventID; }
            set { _LocalEventID = value; }
        }

        public string sSourceEventID
        {
            get { return _SourceEventID; }
            set { _SourceEventID = value; }
        }
        public int iAgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public int iEventStatus
        {
            get { return _EventStatus; }
            set { _EventStatus = value; }
        }

        public DateTime dtLastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

    }

    class EventClassLookup
    {
        private int _ID = 0;
        private int _AgencyID = 0;
        private string _SourceEventClass = "";
        private string _SourceEventClassName = "";
        private int _LocalEventClass = 0;
        private string _LocalEventClassName = "";
        private bool _Is_Transit = false;
        private bool _Is_Planned = false;

        public int iEventClassID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int iAgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string sSourceEventClass
        {
            get { return _SourceEventClass; }
            set { _SourceEventClass = value; }
        }
        public string sSourceEventClassName
        {
            get { return _SourceEventClassName; }
            set { _SourceEventClassName = value; }
        }

        public int iLocalEventClass
        {
            get { return _LocalEventClass; }
            set { _LocalEventClass = value; }
        }
        public string sLocalEventClassName
        {
            get { return _LocalEventClassName; }
            set { _LocalEventClassName = value; }
        }
        public bool bIsTransit
        {
            get { return _Is_Transit; }
            set { _Is_Transit = value; }
        }

        public bool bIsPlanned
        {
            get { return _Is_Planned; }
            set { _Is_Planned = value; }
        }
    }

    class FacilityLookup
    {
        private int _ID = 0;
        private string _LocalFacilityName = "";
        private int _AgencyID = 0;
        private string _SourceFacilityID = "";
        private string _SourceFacilityName = "";
        private int _Direction = 0;

        public int iLocalFacilityID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string sLocalFacilityName
        {
            get { return _LocalFacilityName; }
            set { _LocalFacilityName = value; }
        }

        public int iAgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string sSourceFacilityID
        {
            get { return _SourceFacilityID; }
            set { _SourceFacilityID = value; }
        }
        public string sSourceFacilityName
        {
            get { return _SourceFacilityName; }
            set { _SourceFacilityName = value; }
        }

        public int iDirection
        {
            get { return _Direction; }
            set { _Direction = value; }
        }
    }

    class NodeLookup
    {
        private int _ID = 0;
        private string _LocalNodeName = "";
        private int _LocalFacilityID = 0;
        private string _SourceNodeID = "";
        private string _SourceNodeName = "";
        private int _Direction = 0;
        private double _Latitude = 0;
        private double _Longitude = 0;
        private int _AgencyID = 0;

        public int iLocalNodeID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string sLocalNodeName
        {
            get { return _LocalNodeName; }
            set { _LocalNodeName = value; }
        }

        public int iLocalFacilityID
        {
            get { return _LocalFacilityID; }
            set { _LocalFacilityID = value; }
        }

        public string sSourceNodeID
        {
            get { return _SourceNodeID; }
            set { _SourceNodeID = value; }
        }
        public string sSourceNodeName
        {
            get { return _SourceNodeName; }
            set { _SourceNodeName = value; }
        }

        public double dLatitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }

        public double dLongitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }

        public int iDirection
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public int iAgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }
    }

    class DirectionLookup
    {
        private int _ID = 0;
        private int _AgencyID = 0;
        private string _SourceDirection = "";
        private string _LocalDirection = "";

        public int iLocalDirectionID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int iAgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string sSourceDirection
        {
            get { return _SourceDirection; }
            set { _SourceDirection = value; }
        }

        public string sLocalDirection
        {
            get { return _LocalDirection; }
            set { _LocalDirection = value; }
        }

    }

    class ArticleLookup
    {
        private int _ID = 0;
        private int _AgencyID = 0;
        private string _SourceArticle = "";
        private string _LocalArticle = "";

        public int iLocalArticleID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int iAgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string sSourceArticle
        {
            get { return _SourceArticle; }
            set { _SourceArticle = value; }
        }

        public string sLocalArticle
        {
            get { return _LocalArticle; }
            set { _LocalArticle = value; }
        }

    }
}
