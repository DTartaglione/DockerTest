using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAX_Consumer
{
    enum eFlightType
    {
        dept = 0,
        arrv = 1,
        both = 2
    }

    class PAX
    {
        private int _PAXClassification = 0;
        private int _AirlineID = 0;
        private int _AirportID = 0;
        private int _PAXTypeID = 0;
        private DateTime _PAXDate = new DateTime();
        private List<PAXVolume> _Volume = new List<PAXVolume>();
        private int _FlightType = 0;
        private float _LoadFactorPercentage = 0;
        private string _ExtraInfo = "";
        //private List<PAXWaitTimes> _PAXWaitTime = new List<PAXWaitTimes>();
        private DateTime _LastUpdate = new DateTime();

        public int PAXClassification
        {
            get { return _PAXClassification; }
            set { _PAXClassification = value; }
        }

        public int AirlineID
        {
            get { return _AirlineID; }
            set { _AirlineID = value; }
        }

        public int AirportID
        {
            get { return _AirportID; }
            set { _AirportID = value; }
        }

        public int PAXTypeID
        {
            get { return _PAXTypeID; }
            set { _PAXTypeID = value; }
        }

        public DateTime PAXDate
        {
            get { return _PAXDate; }
            set { _PAXDate = value; }
        }

        public List<PAXVolume> Volume
        {
            get { return _Volume; }
            set { _Volume = value; }
        }

        public int FlightType
        {
            get { return _FlightType; }
            set { _FlightType = value; }
        }

        public float LoadFactorPercentage
        {
            get { return _LoadFactorPercentage; }
            set { _LoadFactorPercentage = value; }
        }

        public string ExtraInfo
        {
            get { return _ExtraInfo; }
            set { _ExtraInfo = value; }
        }

        //public List<PAXWaitTimes> PAXWaitTime
        //{
        //    get { return _PAXWaitTime; }
        //    set { _PAXWaitTime = value; }
        //}

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class PAXVolume
    {
        private string _Terminal = "";
        private int _Hour = 0;
        private int _Volume = -1;
        private int _NumFlights = 0;
        private int _NumBooths = 0;
        private DateTime _PAXHourTimestamp = new DateTime();
        private int _AvgWaitTime = 0;
        private int _MaxWaitTime = 0;
        private int _AvgUSWaitTime = 0;
        private int _MaxUSWaitTime = 0;
        private int _AvgNonUSWaitTime = 0;
        private int _MaxNonUSMaxWaitTime = 0;

        public string Terminal
        {
            get { return _Terminal; }
            set { _Terminal = value; }
        }

        public int Hour
        {
            get { return _Hour; }
            set { _Hour = value; }
        }

        public int Volume
        {
            get { return _Volume; }
            set { _Volume = value; }
        }

        public int NumFlights
        {
            get { return _NumFlights; }
            set { _NumFlights = value; }
        }

        public int NumBooths
        {
            get { return _NumBooths; }
            set { _NumBooths = value; }
        }

        public DateTime PAXHourTimestamp
        {
            get { return _PAXHourTimestamp; }
            set { _PAXHourTimestamp = value; }
        }

        public int AvgWaitTime
        {
            get { return _AvgWaitTime; }
            set { _AvgWaitTime = value; }
        }

        public int MaxWaitTime
        {
            get { return _MaxWaitTime; }
            set { _MaxWaitTime = value; }
        }

        public int AvgUSWaitTime
        {
            get { return _AvgUSWaitTime; }
            set { _AvgUSWaitTime = value; }
        }

        public int MaxUSWaitTime
        {
            get { return _MaxUSWaitTime; }
            set { _MaxUSWaitTime = value; }
        }

        public int AvgNonUSWaitTime
        {
            get { return _AvgNonUSWaitTime; }
            set { _AvgNonUSWaitTime = value; }
        }

        public int MaxNonUSWaitTime
        {
            get { return _MaxNonUSMaxWaitTime; }
            set { _MaxNonUSMaxWaitTime = value; }
        }
    }

    //class PAXWaitTimes
    //{
    //    private int _AvgWaitTime = 0;
    //    private int _MaxWaitTime = 0;
    //    private int _AvgUSWaitTime = 0;
    //    private int _MaxUSWaitTime = 0;
    //    private int _AvgNonUSWaitTime = 0;
    //    private int _MaxNonUSMaxWaitTime = 0;

    //    public int AvgWaitTime
    //    {
    //        get { return _AvgWaitTime; }
    //        set { _AvgWaitTime = value; }
    //    }

    //    public int MaxWaitTime
    //    {
    //        get { return _MaxWaitTime; }
    //        set { _MaxWaitTime = value; }
    //    }

    //    public int AvgUSWaitTime
    //    {
    //        get { return _AvgUSWaitTime; }
    //        set { _AvgUSWaitTime = value; }
    //    }

    //    public int MaxUSWaitTime
    //    {
    //        get { return _MaxUSWaitTime; }
    //        set { _MaxUSWaitTime = value; }
    //    }

    //    public int AvgNonUSWaitTime
    //    {
    //        get { return _AvgNonUSWaitTime; }
    //        set { _AvgNonUSWaitTime = value; }
    //    }

    //    public int MaxNonUSWaitTime
    //    {
    //        get { return _MaxNonUSMaxWaitTime; }
    //        set { _MaxNonUSMaxWaitTime = value; }
    //    }
    //}

}
