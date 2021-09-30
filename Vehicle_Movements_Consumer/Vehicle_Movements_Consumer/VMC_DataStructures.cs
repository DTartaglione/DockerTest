using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Vehicle_Movements_Consumer
{
    class VehicleMovement
    {
        private int _AgencyID = 0;
        private string _DataPointId = "";
        private string _JourneyId = "";
        private string _VehicleIdNum = "";
        private double _VehicleLatitude = 0;
        private double _VehicleLongitude = 0;
        private string _GeoHash = "";
        private double _Speed = 0;
        private double _Heading = 0;
        private string _VehicleMake = "";
        private string _VehicleModel = "";
        private int _VehicleYear = 0;
        private string _IgntitionStatus = "";
        private DateTime _CapturedTimestamp;
        private DateTime _LastUpdate;

        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string DataPointId
        {
            get { return _DataPointId; }
            set { _DataPointId = value; }
        }

        public string JourneyId
        {
            get { return _JourneyId; }
            set { _JourneyId = value; }
        }

        public string VehicleIdNum
        {
            get { return _VehicleIdNum; }
            set { _VehicleIdNum = value; }
        }

        public double VehicleLatitude
        {
            get { return _VehicleLatitude; }
            set { _VehicleLatitude = value; }
        }

        public double VehicleLongitude
        {
            get { return _VehicleLongitude; }
            set { _VehicleLongitude = value; }
        }

        public string GeoHash
        {
            get { return _GeoHash; }
            set { _GeoHash = value; }
        }

        public double Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        public double Heading
        {
            get { return _Heading; }
            set { _Heading = value; }
        }

        public string VehicleMake
        {
            get { return _VehicleMake; }
            set { _VehicleMake = value; }
        }

        public string VehicleModel
        {
            get { return _VehicleModel; }
            set { _VehicleModel = value; }
        }

        public int VehicleYear
        {
            get { return _VehicleYear; }
            set { _VehicleYear = value; }
        }
        public string IgntitionStatus
        {
            get { return _IgntitionStatus; }
            set { _IgntitionStatus = value; }
        }

        public DateTime CapturedTimestamp
        {
            get { return _CapturedTimestamp; }
            set { _CapturedTimestamp = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }
}

