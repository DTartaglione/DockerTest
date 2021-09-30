using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace PA_Alerts_Data_Consumer
{


    class Email
    {
        private string _From = "";
        private string _Subject = "";
        private string _Body = "";
        private DateTime _LastUpdate = new DateTime();

        public string From
        {
            get { return _From; }
            set { _From = value; }
        }
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }
        public string Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class AOC_Traffic_Status
    {
        private int _CodeID = 0;
        private string _CodeName = "";
        private List<AOC_Mitgation> _Mitigations = new List<AOC_Mitgation>();
        private DateTime _LastUpdate = new DateTime();

        public int CodeID
        {
            get { return _CodeID; }
            set { _CodeID = value; }
        }
        public string CodeName
        {
            get { return _CodeName; }
            set { _CodeName = value; }
        }
        public List<AOC_Mitgation> Mitgations
        {
            get { return _Mitigations; }
            set { _Mitigations = value; }
        }
        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class AOC_Mitgation
    {
        private string _Mitigation = "";
        private DateTime _StartTime = new DateTime();
        private DateTime _EndTime = new DateTime();

        public string Mitigaion
        {
            get { return _Mitigation; }
            set { _Mitigation = value; }
        }
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
    }

    class Alerts
    {
        private int _AlertID = 0;
        private string _AlertMessage = "";
        private DateTime _CreateTimestamp = new DateTime();        
        private int _Status = 0;
        private string _Geom = "";
        private int _EmailTypeID = 0;
        private DateTime _LastUpdate = new DateTime();

        public int AlertID
        {
            get { return _AlertID; }
            set { _AlertID = value; }
        }
        public string AlertMessage
        {
            get { return _AlertMessage; }
            set { _AlertMessage = value; }
        }
        public DateTime CreateTimestamp
        {
            get { return _CreateTimestamp; }
            set { _CreateTimestamp = value; }
        }
        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public string Geom
        {
            get { return _Geom; }
            set { _Geom = value; }
        }

        public int EmailTypeID
        {
            get { return _EmailTypeID; }
            set { _EmailTypeID = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }
    class PAXUpdate
    {
        private string _AirlineCode = "";
        private DateTime _HourTimestamp = new DateTime();
        private int _ArrvVolume = 0;
        private int _DeptVolume = 0;
        private DateTime _LastUpdate = new DateTime();
        private string _PAXType = "";

        public string AirlineCode
        {
            get { return _AirlineCode; }
            set { _AirlineCode = value; }
        }
        public DateTime HourTimestamp
        {
            get { return _HourTimestamp; }
            set { _HourTimestamp = value; }
        }
        public int ArrvVolume
        {
            get { return _ArrvVolume; }
            set { _ArrvVolume = value; }
        }
        public int DeptVolume
        {
            get { return _DeptVolume; }
            set { _DeptVolume = value; }
        }
        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
        public string PAXType
        {
            get { return _PAXType; }
            set { _PAXType = value; }
        }
    }
}
