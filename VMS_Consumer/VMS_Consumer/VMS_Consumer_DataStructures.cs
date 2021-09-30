using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace VMS_Consumer
{
    
    class VMSData
    {
        private int _LocalVMSID = 0;
        private string _SourceVMSID = "";
        private int _AgencyID = 0;
        private string _VMSName = "";
        private string _Roadway = "";
        private string _Direction = "";
        private Point _Coords = null;
        private string _Message = "";
        private DateTime _LastUpdate;

        public int LocalVMSID
        {
            get { return _LocalVMSID; }
            set { _LocalVMSID = value; }
        }

        public string SourceVMSID
        {
            get { return _SourceVMSID; }
            set { _SourceVMSID = value; }
        }
        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public string VMSName
        {
            get { return _VMSName; }
            set { _VMSName = value; }
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

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    //class StoredVMSData
    //{
    //    private string _SourceVMSID = "";
    //    private string _StoredJSON = "";

    //    public string SourceVMSID
    //    {
    //        get { return _SourceVMSID; }
    //        set { _SourceVMSID = value; }
    //    }

    //    public string StoredJSON
    //    {
    //        get { return _StoredJSON; }
    //        set { _StoredJSON = value; }
    //    }
    //}

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
