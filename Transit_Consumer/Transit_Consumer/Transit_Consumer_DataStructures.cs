using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Transit_Consumer
{    
    class Config
    {
        private string _DataType = "Config";
        private Route _RouteConfig = new Route();
        private List<Stop> _StopList = new List<Stop>();

        public string DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        public Route RouteConfig
        {
            get { return _RouteConfig; }
            set { _RouteConfig = value; }
        }
        public List<Stop> StopList
        {
            get { return _StopList; }
            set { _StopList = value; }
        }
    }

    class Coordinate
    {
        private double _Latitude = 0;
        private double _Longitude = 0;

        public double Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }

        public double Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }
    }

    class Route
    {
        private int _Local_Route_Id = 0;
        private string _Route_Id = "";
        private string _Agency_Id = "";
        private string _Name = "";
        private string _Short_Name = "";
        private string _Description = "";
        private string _Direction = "";
        private int _Type = 0;
        private string _URL = "";
        private string _Color = "";
        private string _Text_Color = "";
        private int _Sort_Order = 0;
        private List<Coordinate> _Route_Path = new List<Coordinate>();

        public int Local_Route_Id
        {
            get { return _Local_Route_Id; }
            set { _Local_Route_Id = value; }
        }

        public string Route_Id
        {
            get { return _Route_Id; }
            set { _Route_Id = value; }
        }

        public string Agency_Id
        {
            get { return _Agency_Id; }
            set { _Agency_Id = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Short_Name
        {
            get { return _Short_Name; }
            set { _Short_Name = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public int Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }

        public string Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        public string Text_Color
        {
            get { return _Text_Color; }
            set { _Text_Color = value; }
        }

        public int Sort_Order
        {
            get { return _Sort_Order; }
            set { _Sort_Order = value; }
        }

        public List<Coordinate> Route_Path
        {
            get { return _Route_Path; }
            set { _Route_Path = value; }
        }
    }

    class Stop
    {
        private int _Local_Stop_Id = 0;
        private int _Stop_Id = 0;
        private string _Name = "";
        private string _Code = "";
        private string _Description = "";
        private Coordinate _Coords = new Coordinate();
        private int _Zone_Id = 0;
        private string _URL = "";
        private int _Location_Type = 0;
        private int _Parent_Station_Id = 0;
        private string _Stop_Timezone = "";
        private int _Wheelchair_Boarding = 0;
        private int _Level_Id = 0;
        private string _Platform_Code = "";
        private int _Stop_Sequence = 0;

        public int Local_Stop_Id
        {
            get { return _Local_Stop_Id; }
            set { _Local_Stop_Id = value; }
        }

        public int Stop_Id
        {
            get { return _Stop_Id; }
            set { _Stop_Id = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public Coordinate Coords
        {
            get { return _Coords; }
            set { _Coords = value; }
        }

        public int Zone_Id
        {
            get { return _Zone_Id; }
            set { _Zone_Id = value; }
        }

        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }

        public int Location_Type
        {
            get { return _Location_Type; }
            set { _Location_Type = value; }
        }

        public int Parent_Station_Id
        {
            get { return _Parent_Station_Id; }
            set { _Parent_Station_Id = value; }
        }

        public string Stop_Timezone
        {
            get { return _Stop_Timezone; }
            set { _Stop_Timezone = value; }
        }

        public int Wheelchair_Boarding
        {
            get { return _Wheelchair_Boarding; }
            set { _Wheelchair_Boarding = value; }
        }

        public int Level_Id
        {
            get { return _Level_Id; }
            set { _Level_Id = value; }
        }

        public string Platform_Code
        {
            get { return _Platform_Code; }
            set { _Platform_Code = value; }
        }

        public int Stop_Sequence
        {
            get { return _Stop_Sequence; }
            set { _Stop_Sequence = value; }
        }
    }

    class Predictions
    {
        private string _DataType = "Predictions";
        private string _Agency = "";
        private string _Route = "";
        private DateTime _LastUpdate = new DateTime();
        private string _StopTag = "";
        private bool _IsDeparture = false;
        private int _Minutes = 0;
        private int _Seconds = 0;
        private string _TripTag = "";
        private string _Vehicle = "";
        private string _Block = "";
        private string _DirTag = "";
        private long _EpochTime = 0;

        public string DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        public string Agency
        {
            get { return _Agency; }
            set { _Agency = value; }
        }
        public string Route
        {
            get { return _Route; }
            set { _Route = value; }
        }
        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
        public string StopTag
        {
            get { return _StopTag; }
            set { _StopTag = value; }
        }
        public bool IsDeparture
        {
            get { return _IsDeparture; }
            set { _IsDeparture = value; }
        }
        public int Minutes
        {
            get { return _Minutes; }
            set { _Minutes = value; }
        }
        public int Seconds
        {
            get { return _Seconds; }
            set { _Seconds = value; }
        }
        public string TripTag
        {
            get { return _TripTag; }
            set { _TripTag = value; }
        }
        public string Vehicle
        {
            get { return _Vehicle; }
            set { _Vehicle = value; }
        }
        public string Block
        {
            get { return _Block; }
            set { _Block = value; }
        }
        public string DirTag
        {
            get { return _DirTag; }
            set { _DirTag = value; }
        }
        public long EpochTime
        {
            get { return _EpochTime; }
            set { _EpochTime = value; }
        }

    }

    class VehicleLocations
    {
        private string _DataType = "VehicleLocations";
        private string _TripId = "";
        private string _Id = "";
        private string _Agency = "";
        private string _Route = "";
        private int _StopId = 0;
        private DateTime _LastUpdate = new DateTime();
        private Coordinate _Coords = new Coordinate();
        private bool _Predictable = false;
        private double _SpeedKmHr = 0;
        private string _DirTag = "";
        private int _Heading = 0;
        private int _SecsSinceReport = 0;
        private int _DirectionId = 0;
        private string _StartDate = "";

        public string DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        public string TripId
        {
            get { return _TripId; }
            set { _TripId = value; }
        }


        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Agency
        {
            get { return _Agency; }
            set { _Agency = value; }
        }
        public string Route
        {
            get { return _Route; }
            set { _Route = value; }
        }
        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
        public int StopId
        {
            get { return _StopId; }
            set { _StopId = value; }
        }

        public Coordinate Coords
        {
            get { return _Coords; }
            set { _Coords = value; }
        }

        public bool Predictable
        {
            get { return _Predictable; }
            set { _Predictable = value; }
        }
        public double SpeedKmHr
        {
            get { return _SpeedKmHr; }
            set { _SpeedKmHr = value; }
        }
        public string DirTag
        {
            get { return _DirTag; }
            set { _DirTag = value; }
        }
        public int Heading
        {
            get { return _Heading; }
            set { _Heading = value; }
        }
        
        public int SecsSinceReport
        {
            get { return _SecsSinceReport; }
            set { _SecsSinceReport = value; }
        }

        public int DirectionId
        {
            get { return _DirectionId; }
            set { _DirectionId = value; }
        }

        public string StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
    }

    class TripUpdates
    {
        private string _DataType = "TripUpdate";
        private string _Agency = "";
        private string _TripId = "";
        private string _Route = "";
        private int _DirectionId = 0;
        private string _Vehicle = "";
        private string _StartDate = "";
        private DateTime _LastUpdate = new DateTime();
        private int _Delay = 0;
        private List<StopTimeUpdates> _StopTimeUpdates = new List<StopTimeUpdates>();

        public string DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }
        public string Agency
        {
            get { return _Agency; }
            set { _Agency = value; }
        }

        public string TripId
        {
            get { return _TripId; }
            set { _TripId = value; }
        }

        public string Route
        {
            get { return _Route; }
            set { _Route = value; }
        }

        public int DirectionId
        {
            get { return _DirectionId; }
            set { _DirectionId = value; }
        }
        public string Vehicle
        {
            get { return _Vehicle; }
            set { _Vehicle = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

        public string StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        public int Delay
        {
            get { return _Delay; }
            set { _Delay = value; }
        }

        public List<StopTimeUpdates> StopTimeUpdates
        {
            get { return _StopTimeUpdates; }
            set { _StopTimeUpdates = value; }
        }
    }

    class StopTimeUpdates
    {
        private int _Stop_Id = 0;
        private int _Stop_Sequence = 0;
        private long _Arrival = 0;
        private int _ArrivalDelay = 0;
        private int _ArrivalUncertainty = 0;
        private long _Departure = 0;
        private int _DepartureDelay = 0;
        private int _DepartureUncertainty = 0;

        public int Stop_Id
        {
            get { return _Stop_Id; }
            set { _Stop_Id = value; }
        }

        public int Stop_Sequence
        {
            get { return _Stop_Sequence; }
            set { _Stop_Sequence = value; }
        }

        public long Arrival
        {
            get { return _Arrival; }
            set { _Arrival = value; }
        }

        public int ArrivalDelay
        {
            get { return _ArrivalDelay; }
            set { _ArrivalDelay = value; }
        }
        public int ArrivalUncertainty
        {
            get { return _ArrivalUncertainty; }
            set { _ArrivalUncertainty = value; }
        }

        public long Departure
        {
            get { return _Departure; }
            set { _Departure = value; }
        }

        public int DepartureDelay
        {
            get { return _DepartureDelay; }
            set { _DepartureDelay = value; }
        }

        public int DepartureUncertainty
        {
            get { return _DepartureUncertainty; }
            set { _DepartureUncertainty = value; }
        }
    }


    class Messages
    {
        private string _DataType = "Messages";
        private string _Agency = "";
        private string _Route = "";
        private DateTime _LastUpdate = new DateTime();
        private string _Id = "";
        private bool _SendToBuses = false;
        private string _Text = "";
        private string _Priority = "";
        private long _EndTime = 0;
        private long _StartTime = 0;
        private string _StopTag = "";
        private List<Interval> _Intervals = new List<Interval>();

        public string DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        public string Agency
        {
            get { return _Agency; }
            set { _Agency = value; }
        }
        public string Route
        {
            get { return _Route; }
            set { _Route = value; }
        }
        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public bool SendToBuses
        {
            get { return _SendToBuses; }
            set { _SendToBuses = value; }
        }
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }
        public string Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }
        public long EndBoundary
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
        public long StartBoundary
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        public string StopTag
        {
            get { return _StopTag; }
            set { _StopTag = value; }
        }
        public List<Interval> Intervals
        {
            get { return _Intervals; }
            set { _Intervals = value; }
        }
    }

    class Interval
    {
        private int _StartTime = 0;
        private int _EndDay = 0;
        private int _StartDay = 0;
        private int _EndTime = 0;
        public int StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        public int EndDay
        {
            get { return _EndDay; }
            set { _EndDay = value; }
        }
        public int StartDay
        {
            get { return _StartDay; }
            set { _StartDay = value; }
        }
        public int EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
    }

    
}
