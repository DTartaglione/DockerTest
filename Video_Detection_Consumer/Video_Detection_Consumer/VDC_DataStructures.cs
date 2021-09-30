using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Video_Detection_Consumer
{
    class SensorZone
    {
        private int _DeviceID = 0;
        private string _NativeDeviceID = "";
        private int _AgencyID = 0;
        private int _DeviceTypeID = 0;
        private string _DeviceType = "";
        private int _DeviceSubTypeID = 0;
        private string _DeviceSubType = "";
        private string _DeviceName = "";
        private int _ReportingIntervalInMinutes = 0;
        private double _Occupancy = -1;
        private int _Volume = -1;
        private int _SpeedInMph = -1;
        private int _DirectionId = 0;
        private DateTime _IntervalTimestamp = new DateTime();
        //private Sensor _SensorDevice = new Sensor();

        public int DeviceID
        {
            get { return _DeviceID; }
            set { _DeviceID = value; }
        }

        public string NativeDeviceID
        {
            get { return _NativeDeviceID; }
            set { _NativeDeviceID = value; }
        }

        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public int DeviceTypeID
        {
            get { return _DeviceTypeID; }
            set { _DeviceTypeID = value; }
        }

        public string DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; }
        }

        public int DeviceSubTypeID
        {
            get { return _DeviceSubTypeID; }
            set { _DeviceSubTypeID = value; }
        }

        public string DeviceSubType
        {
            get { return _DeviceSubType; }
            set { _DeviceSubType = value; }
        }

        public string DeviceName
        {
            get { return _DeviceName; }
            set { _DeviceName = value; }
        }

        public int ReportingIntervalInMinutes
        {
            get { return _ReportingIntervalInMinutes; }
            set { _ReportingIntervalInMinutes = value; }
        }

        public double Occupancy
        {
            get { return _Occupancy; }
            set { _Occupancy = value; }
        }

        public int Volume
        {
            get { return _Volume; }
            set { _Volume = value; }
        }

        public int SpeedInMph
        {
            get { return _SpeedInMph; }
            set { _SpeedInMph = value; }
        }

        public int DirectionId
        {
            get { return _DirectionId; }
            set { _DirectionId = value; }
        }

        public DateTime IntervalTimestamp
        {
            get { return _IntervalTimestamp; }
            set { _IntervalTimestamp = value; }
        }

        //public Sensor SensorDevice
        //{
        //    get { return _SensorDevice; }
        //    set { _SensorDevice = value; }
        //}
    }

}

