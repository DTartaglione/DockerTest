using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Aviation_DI
{
    
    class Aviation_DI_FlightData
    {
        private string _SourceFlightID = "";
        private int _AgencyID = 0;
        private string _Status = "";
        private string _Type = "";
        private int _FlightTypeID = 0;
        private string _OriginAirportCode = "";
        private string _DestinationAirportCode = "";
        private string _AircraftType = "";
        private string _ScheduledDeparture = "";
        private string _EstimatedDeparture = "";
        private string _ActualDeparture = "";
        private string _ScheduledArrival = "";
        private string _EstimatedArrival = "";
        private string _ActualArrival = "";
        private string _DepartureGate = "";
        private string _ArrivalGate = "";
        private string _AirlineName = "";
        private string _AirlineCode = "UNK";
        private string _CodeShares = "";
        private string _FlightNum = "";
        private double _Latitude = 0.0;
        private double _Longitude = 0.0;
        private double _Altitude = 0.0;
        private double _Speed = 0.0;
        private double _Direction = 0.0;
        private int _DepartureDelay = 0;
        private int _ArrivalDelay = 0;
        private int _NumSeats = 0;
        private DateTime _LastUpdate = new DateTime();

        public string SourceFlightID
        {
            get { return _SourceFlightID; }
            set { _SourceFlightID = value; }
        }

        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public int FlightTypeID
        {
            get { return _FlightTypeID; }
            set { _FlightTypeID = value; }
        }

        public string OriginAirportCode
        {
            get { return _OriginAirportCode; }
            set { _OriginAirportCode = value; }
        }

        public string DestinationAirportCode
        {
            get { return _DestinationAirportCode; }
            set { _DestinationAirportCode = value; }
        }

        public string AircraftType
        {
            get { return _AircraftType; }
            set { _AircraftType = value; }
        }

        public string ScheduledDeparture
        {
            get { return _ScheduledDeparture; }
            set { _ScheduledDeparture = value; }
        }

        public string EstimatedDeparture
        {
            get { return _EstimatedDeparture; }
            set { _EstimatedDeparture = value; }
        }

        public string ActualDeparture
        {
            get { return _ActualDeparture; }
            set { _ActualDeparture = value; }
        }

        public string ScheduledArrival
        {
            get { return _ScheduledArrival; }
            set { _ScheduledArrival = value; }
        }

        public string EstimatedArrival
        {
            get { return _EstimatedArrival; }
            set { _EstimatedArrival = value; }
        }

        public string ActualArrival
        {
            get { return _ActualArrival; }
            set { _ActualArrival = value; }
        }

        public string DepartureGate
        {
            get { return _DepartureGate; }
            set { _DepartureGate = value; }
        }

        public string ArrivalGate
        {
            get { return _ArrivalGate; }
            set { _ArrivalGate = value; }
        }

        public string AirlineName
        {
            get { return _AirlineName; }
            set { _AirlineName = value; }
        }

        public string AirlineCode
        {
            get { return _AirlineCode; }
            set { _AirlineCode = value; }
        }

        public string CodeShares
        {
            get { return _CodeShares; }
            set { _CodeShares = value; }
        }

        public string FlightNum
        {
            get { return _FlightNum; }
            set { _FlightNum = value; }
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
        
        public double Altitude
        {
            get { return _Altitude; }
            set { _Altitude = value; }
        }

        public double Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        public double Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public int DepartureDelay
        {
            get { return _DepartureDelay; }
            set { _DepartureDelay = value; }
        }

        public int ArrivalDelay
        {
            get { return _ArrivalDelay; }
            set { _ArrivalDelay = value; }
        }

        public int NumSeats
        {
            get { return _NumSeats; }
            set { _NumSeats = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class Aviation_DI_StoredFlightData
    {
        Aviation_DI_FlightData _StoredFlight;
        bool _DoUpdate = false;

        public Aviation_DI_FlightData StoredFlight
        {
            get { return _StoredFlight; }
            set { _StoredFlight = value; }
        }

        public bool DoUpdate
        {
            get { return _DoUpdate; }
            set { _DoUpdate = value; }
        }

    }

}
