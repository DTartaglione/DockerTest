using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CCTV_Consumer
{
    
    class CCTVData
    {
        private int _LocalCCTVID = 0;
        private string _SourceCCTVID = "";
        private int _AgencyID = 0;
        private string _CCTVName = "";
        private string _Roadway = "";
        private string _Direction = "";
        private Point _Coords = null;
        private string _SnapshotURL = "";
        private string _VideoURL = "";
        private int _Status = 0;
        private bool _Blocked = false;
        private DateTime _LastUpdate;

        public int LocalCCTVID
        {
            get { return _LocalCCTVID; }
            set { _LocalCCTVID = value; }
        }

        public string SourceCCTVID
        {
            get { return _SourceCCTVID; }
            set { _SourceCCTVID = value; }
        }
        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string CCTVName
        {
            get { return _CCTVName; }
            set { _CCTVName = value; }
        }

        public string Roadway
        {
            get { return _Roadway; }
            set { _Roadway = value; }
        }

        public string Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public Point Coords
        {
            get { return _Coords; }
            set { _Coords = value; }
        }

        public string SnapshotURL
        {
            get { return _SnapshotURL; }
            set { _SnapshotURL = value; }
        }

        public string VideoURL
        {
            get { return _VideoURL; }
            set { _VideoURL = value; }
        }

        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public bool Blocked
        {
            get { return _Blocked; }
            set { _Blocked = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class StoredCCTVData
    {
        private string _SourceCCTVID = "";
        private StringBuilder _StoredJSON = new StringBuilder();
        
        public string SourceCCTVID
        {
            get { return _SourceCCTVID; }
            set { _SourceCCTVID = value; }
        }

        public StringBuilder StoredJSON
        {
            get { return _StoredJSON; }
            set { _StoredJSON = value; }
        }
    }

    class Point
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

}
