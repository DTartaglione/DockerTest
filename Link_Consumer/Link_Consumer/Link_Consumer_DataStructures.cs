using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Link_Consumer.Link_Consumer_DataStructures
{
    
    class Link_Consumer_LinkData
    {
        private Int64 _LinkID = 0;
        private int _Speed = -1;
        private double _TravelTime = -1;
        private int _Freeflow = -1;
        private int _Volume = -1;
        private int _Occupancy = -1;
        private string _DataType = "";
        private string _AgencyID = "";
        private DateTime _LastUpdate;

        public Int64 LinkID
        {
            get { return _LinkID; }
            set { _LinkID = value; }
        }

        public int Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        public double TravelTime
        {
            get { return _TravelTime; }
            set { _TravelTime = value; }
        }
        public int Freeflow
        {
            get { return _Freeflow; }
            set { _Freeflow = value; }
        }

        public int Volume
        {
            get { return _Volume; }
            set { _Volume = value; }
        }
        public int Occupancy
        {
            get { return _Occupancy; }
            set { _Occupancy = value; }
        }
        public string DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        public string AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

    }

    class Link_Consumer_StoredLinkData
    {
        private string _LinkID = "";
        private string _AgencyID = "";
        private string _LastUpdate = "";
        private string _LinkData = "";
        private bool _DoUpdate = false;

        public string sLinkID
        {
            get { return _LinkID; }
            set { _LinkID = value; }
        }

        public string AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string sLastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

        public string sLinkData
        {
            get { return _LinkData; }
            set { _LinkData = value; }
        }

        public bool bDoUpdate
        {
            get { return _DoUpdate; }
            set { _DoUpdate = value; }
        }


    }
}
