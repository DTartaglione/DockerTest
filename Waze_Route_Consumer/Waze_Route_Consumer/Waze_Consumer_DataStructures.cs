using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Waze_Route_Consumer
{
    
    class Route
    {
        private int _AgencyID = 0;
        private int _RouteID = 0;
        private string _RouteName = "";
        private string _FromName = "";
        private string _ToName = "";
        private int _Length = 0;
        private int _HistoricTime = 0;
        private int _CurrentTime = 0;
        private int _JamLevel = 0;
        private string _Line = "";
        private List<SubRoute> _SubRoutes = new List<SubRoute>();
        private DateTime _LastUpdate = new DateTime();


        public int AgencyID
        {
            get { return _AgencyID; }
            set { _AgencyID = value; }
        }

        public int RouteID
        {
            get { return _RouteID; }
            set { _RouteID = value; }
        }

        public string RouteName
        {
            get { return _RouteName; }
            set { _RouteName = value; }
        }

        public string FromName
        {
            get { return _FromName; }
            set { _FromName = value; }
        }

        public string ToName
        {
            get { return _ToName; }
            set { _ToName = value; }
        }

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        public int HistoricTime
        {
            get { return _HistoricTime; }
            set { _HistoricTime = value; }
        }

        public int CurrentTime
        {
            get { return _CurrentTime; }
            set { _CurrentTime = value; }
        }

        public int JamLevel
        {
            get { return _JamLevel; }
            set { _JamLevel = value; }
        }

        public string Line
        {
            get { return _Line; }
            set { _Line = value; }
        }

        public List<SubRoute> SubRoutes
        {
            get { return _SubRoutes; }
            set { _SubRoutes = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

    }

    class SubRoute
    {
        private int _RouteID = 0;
        private string _FromName = "";
        private string _ToName = "";
        private int _Length = 0;
        private int _HistoricTime = 0;
        private int _CurrentTime = 0;
        private int _JamLevel = 0;
        private string _Line = "";
        private DateTime _LastUpdate = new DateTime();

        public int RouteID
        {
            get { return _RouteID; }
            set { _RouteID = value; }
        }

        public string FromName
        {
            get { return _FromName; }
            set { _FromName = value; }
        }

        public string ToName
        {
            get { return _ToName; }
            set { _ToName = value; }
        }

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        public int HistoricTime
        {
            get { return _HistoricTime; }
            set { _HistoricTime = value; }
        }

        public int CurrentTime
        {
            get { return _CurrentTime; }
            set { _CurrentTime = value; }
        }

        public int JamLevel
        {
            get { return _JamLevel; }
            set { _JamLevel = value; }
        }

        public string Line
        {
            get { return _Line; }
            set { _Line = value; }
        }
        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }

    }

    class StoredRouteData
    {
        private string _SourceRouteID = "";
        private DateTime _LastUpdate = new DateTime();

        public string SourceRouteID
        {
            get { return _SourceRouteID; }
            set { _SourceRouteID = value; }
        }


        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

    class StoredSubrouteData
    {
        private string _SourceRouteID = "";
        public string _SubrouteName = "";
        private DateTime _LastUpdate = new DateTime();

        public string SourcerouteID
        {
            get { return _SourceRouteID; }
            set { _SourceRouteID = value; }
        }

        public string SubrouteName
        {
            get { return _SubrouteName; }
            set { _SubrouteName = value; }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set { _LastUpdate = value; }
        }
    }

}
