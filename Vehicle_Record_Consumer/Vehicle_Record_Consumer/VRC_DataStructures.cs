using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Vehicle_Record_Consumer
{
    class VehicleRecord
    {
        private int _DeviceID = 0;
        private int _AgencyID = 0;
        private string _NativeDeviceID = "";
        private int _DeviceTypeID = 0;
        private int _DeviceSubTypeID = 0;
        private int _Speed = 0;
        private double _VehicleLength = 0.0;
        private double _Gap = 0.0;
        private DateTime _RecordTimestamp;
        private DateTime _LastUpdate;

        public int DeviceID
        {
            get { return _DeviceID; }
            set { _DeviceID = value; }
        }
        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }
        public string NativeDeviceID
        {
            get { return _NativeDeviceID; }
            set { _NativeDeviceID = value; }
        }

        public int DeviceTypeID
        {
            get { return _DeviceTypeID; }
            set { _DeviceTypeID = value; }
        }

        public int DeviceSubTypeID
        {
            get { return _DeviceSubTypeID; }
            set { _DeviceSubTypeID = value; }
        }

        public int Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        public double VehicleLength
        {
            get { return _VehicleLength; }
            set { _VehicleLength = value; }
        }

        public double Gap
        {
            get { return _Gap; }
            set { _Gap = value; }
        }
        public DateTime RecordTimestamp
        {
            get { return _RecordTimestamp; }
            set { _RecordTimestamp = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }
}

