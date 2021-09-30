using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Crossing_Data_Consumer
{

    class CrossingData
    {
        private int _SourceFacilityId = 0;
        private string _SourceFacilityName = "";
        private int _AgencyId = 0;
        private int _LocalFacilityId = 0;
        private string _LocalFacilityName = "";
        private int _CAWaitTimeMins = 0;
        private int _USWaitTimeMins = 0;
        private DateTime _LastUpdate;


        public int SourceFacilityId
        {
            get { return _SourceFacilityId; }
            set { _SourceFacilityId = value; }
        }

        public string SourceFacilityName
        {
            get { return _SourceFacilityName; }
            set { _SourceFacilityName = value; }
        }

        public int AgencyId
        {
            get { return _AgencyId; }
            set { _AgencyId = value; }
        }

        public int LocalFacilityId
        {
            get { return _LocalFacilityId; }
            set { _LocalFacilityId = value; }
        }

        public string LocalFacilityName
        {
            get { return _LocalFacilityName; }
            set { _LocalFacilityName = value; }
        }

        public int CAWaitTimeMins
        {
            get { return _CAWaitTimeMins; }
            set { _CAWaitTimeMins = value; }
        }

        public int USWaitTimeMins
        {
            get { return _USWaitTimeMins; }
            set { _USWaitTimeMins = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }
}

