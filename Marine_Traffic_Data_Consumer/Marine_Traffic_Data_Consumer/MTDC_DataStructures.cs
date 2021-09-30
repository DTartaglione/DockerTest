using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Marine_Traffic_Data_Consumer
{
    class BerthItem
    {
        PortInfo _PortInfo = new PortInfo();
        TerminalInfo _TerminalInfo = new TerminalInfo();
        BerthInfo _BerthInfo = new BerthInfo();
        ShipInfo _ShipInfo = new ShipInfo();

        public PortInfo PortInfo
        {
            get { return _PortInfo; }
            set { _PortInfo = value; }
        }

        public TerminalInfo TerminalInfo
        {
            get { return _TerminalInfo; }
            set { _TerminalInfo = value; }
        }

        public BerthInfo BerthInfo
        {
            get { return _BerthInfo; }
            set { _BerthInfo = value; }
        }

        public ShipInfo ShipInfo
        {
            get { return _ShipInfo; }
            set { _ShipInfo = value; }
        }

    }

    class PortInfo
    {
        private int _ID = 0;
        private int _AgencyID = 0;
        private string _UNLOCODE = "";
        private string _Name = "";
        private string _State = "";

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string UNLOCODE
        {
            get { return _UNLOCODE; }
            set { _UNLOCODE = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string State
        {
            get { return _State; }
            set { _State = value; }
        }
    }

    class TerminalInfo
    {
        private int _ID = 0;
        private int _AgencyID = 0;
        private int _PortID = 0;
        private string _Name = "";

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public int PortID
        {
            get { return _PortID; }
            set { _PortID = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }

    class BerthInfo
    {
        private int _ID = 0;
       // private int _AgencyID = 0;
        private string _Name = "";
        private int _PortID = 0;
        private int _TerminalID = 0;
        private double _Latitude = 0;
        private double _Longitude = 0;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        //public int AgencyID
        //{
        //    get { return _AgencyID; }
        //    set { _AgencyID = value; }
        //}
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int PortID
        {
            get { return _PortID; }
            set { _PortID = value; }
        }

        public int TerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }
        
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

    class ShipInfo
    {
        private int _ID = 0;
        private int _AgencyID = 0;
        private int _SourceShipID = 0;
        private int _MMSI_ID = 0;
        private int _IMO_ID = 0;
        private string _Name = "";
        private string _TypeName = "";
        private int _DeadWeight = 0;
        private int _GrossTonnage = 0;
        private int _YearBuilt = 0;
        private DateTime? _ArrivalTimestamp = null;
        private DateTime? _DepartureTimestamp = null;
        private DateTime? _DockTimestamp = null;
        private DateTime? _UndockTimestamp = null;
        private int _TimeAtBerth = 0;
        private int _TimeAtPort = 0;
        private int _ArrivalLoadStatus = 0;
        private int _DepartureLoadStatus = 0;
        private DateTime _LastUpdate = new DateTime();

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public int SourceShipID
        {
            get { return _SourceShipID; }
            set { _SourceShipID = value; }
        }

        public int MMSI_ID
        {
            get { return _MMSI_ID; }
            set { _MMSI_ID = value; }
        }

        public int IMO_ID
        {
            get { return _IMO_ID; }
            set { _IMO_ID = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string TypeName
        {
            get { return _TypeName; }
            set { _TypeName = value; }
        }

        public int DWT
        {
            get { return _DeadWeight; }
            set { _DeadWeight = value; }
        }

        public int GRT
        {
            get { return _GrossTonnage; }
            set { _GrossTonnage = value; }
        }

        public int YearBuilt
        {
            get { return _YearBuilt; }
            set { _YearBuilt = value; }
        }
                
        public DateTime? ArrivalTimestamp
        {
            get { return _ArrivalTimestamp; }
            set { _ArrivalTimestamp = value; }
        }
        public DateTime? DepartureTimestamp
        {
            get { return _DepartureTimestamp; }
            set { _DepartureTimestamp = value; }
        }
        public DateTime? DockTimestamp
        {
            get { return _DockTimestamp; }
            set { _DockTimestamp = value; }
        }
        public DateTime? UndockTimestamp
        {
            get { return _UndockTimestamp; }
            set { _UndockTimestamp = value; }
        }

        public int TimeAtBerth
        {
            get { return _TimeAtBerth; }
            set { _TimeAtBerth = value; }
        }

        public int TimeAtPort
        {
            get { return _TimeAtPort; }
            set { _TimeAtPort = value; }
        }

        public int ArrivalLoadStatus
        {
            get { return _ArrivalLoadStatus; }
            set { _ArrivalLoadStatus = value; }
        }
        public int DepartureLoadStatus
        {
            get { return _DepartureLoadStatus; }
            set { _DepartureLoadStatus = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class LoadStatusInfo
    {
        private int _SourceStatusID = 0;
        private string _LoadStatus = "";

     
        public int SourceStatusID
        {
            get { return _SourceStatusID; }
            set { _SourceStatusID = value; }
        }
        public string LoadStatus
        {
            get { return _LoadStatus; }
            set { _LoadStatus = value; }
        }
    }


    //class StoredFlightData
    //{
    //    private string _FlightAwareID = "";
    //    private string _FlightNumber = "";
    //    private int _FlightProgress = 0;
    //    private string _Status = "";
    //    private DateTime _LastUpdate = new DateTime();

    //    public string sFlightAwareID
    //    {
    //        get { return _FlightAwareID; }
    //        set { _FlightAwareID = value; }
    //    }

    //    public string sFlightNumber
    //    {
    //        get { return _FlightNumber; }
    //        set { _FlightNumber = value; }
    //    }

    //    public string sStatus
    //    {
    //        get { return _Status; }
    //        set { _Status = value; }
    //    }

    //    public int iFlightProgress
    //    {
    //        get { return _FlightProgress; }
    //        set { _FlightProgress = value; }
    //    }

    //    public DateTime LastUpdate
    //    {
    //        get { return _LastUpdate; }
    //        set { _LastUpdate = value; }
    //    }

    //}

}
