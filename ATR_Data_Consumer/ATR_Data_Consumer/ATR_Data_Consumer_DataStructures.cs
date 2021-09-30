using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ATR_Data_Consumer
{    
    class ATRData
    {
        private int _DeviceID = 0;
        private string _DeviceName = "";
        private int _AgencyID = 0;
        private int _DeviceTypeID = 0;
        private string _DeviceType = "";
        private int _DeviceSubTypeID = 0;
        private string _DeviceSubType = "";
        private int _DeviceParentID = 0;
        private int _ReportingIntervalInMinutes = 0;
        private int _Volume = 0;
        private int _SpeedInMph = 0;
        private double _Occupancy = 0;
        private DateTime _IntervalEnd = new DateTime();
        private DateTime _LastUpdate = new DateTime();

        public int DeviceID
        {
            get { return _DeviceID; }
            set { _DeviceID = value; }
        }

        public string DeviceName
        {
            get { return _DeviceName; }
            set { _DeviceName = value; }
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

        public int DeviceParentID
        {
            get { return _DeviceParentID; }
            set { _DeviceParentID = value; }
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

        public DateTime IntervalEnd
        {
            get { return _IntervalEnd; }
            set { _IntervalEnd = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class SensorZoneSensor
    {
        private SensorZone _SensorZone = new SensorZone();

        public SensorZone SensorZone
        {
            get { return _SensorZone; }
            set { _SensorZone = value; }
        }
    }


    class SensorZone
    {
        private int _DeviceID = 0;
        private int _AgencyID = 0;
        private int _DeviceTypeID = 0;
        private string _DeviceType = "";
        private int _DeviceSubTypeID = 0;
        private string _DeviceSubType = "";
        private string _DeviceName = "";
        private Sensor _SensorDevice = new Sensor();

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

        public Sensor SensorDevice
        {
            get { return _SensorDevice; }
            set { _SensorDevice = value; }
        }
    }

    class Sensor
    {
        private int _DeviceID = 0;
        private int _ParentDeviceID = 0;
        private int _DeviceTypeID = 0;
        private string _DeviceType = "";
        private int _DeviceSubTypeID = 0;
        private string _DeviceSubType = "";
        private string _DeviceName = "";
        private string _DeviceDescription = "";
        private int _ReportingIntervalInMinutes = 0;
        private double _Occupancy = -1;
        private int _Volume = -1;
        private int _SpeedInMph = -1;
        private DateTime _TimePeriod = new DateTime();

        public int DeviceID
        {
            get { return _DeviceID; }
            set { _DeviceID = value; }
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

        public int ParentDeviceID
        {
            get { return _ParentDeviceID; }
            set { _ParentDeviceID = value; }
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

        public DateTime TimePeriod
        {
            get { return _TimePeriod; }
            set { _TimePeriod = value; }
        }
    }
}

