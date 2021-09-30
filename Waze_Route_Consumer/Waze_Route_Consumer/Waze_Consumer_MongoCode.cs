using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Waze_Route_Consumer
{
    class Waze_Route_Consumer_MongoCode
    {
        public static bool DoBatchUpdateRoutes(List<Route> lstData)
        {
            bool bSuccess = false;

            try
            {
                var _databaseName = MongoUrl.Create(Waze_Route_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(Waze_Route_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<Route> collData = _MongoDB.GetCollection<Route>("tblwazeroutedata");
                List<Route> lstRouteData = new List<Route>();

                Waze_Route_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + Waze_Route_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                Waze_Route_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " Waze route records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error inserting Waze Route records into MongoDB document store in Waze_Route_Consumer_MongoCode.DoBatchUpdateRoutes().");
                Waze_Route_Consumer_Globals.LogFile.Log(ex.ToString());
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }

        public static bool DoBatchUpdateSubRoutes(List<SubRoute> lstData)
        {
            bool bSuccess = false;

            try
            {
                var _databaseName = MongoUrl.Create(Waze_Route_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(Waze_Route_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<SubRoute> collData = _MongoDB.GetCollection<SubRoute>("tblwazesubroutedata");
                List<SubRoute> lstRouteData = new List<SubRoute>();

                Waze_Route_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + Waze_Route_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                Waze_Route_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " Waze SubRoute records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                Waze_Route_Consumer_Globals.LogFile.LogErrorText("Error inserting Waze SubRoute records into MongoDB document store in Waze_Route_Consumer_MongoCode.DoBatchUpdateSubRoutes().");
                Waze_Route_Consumer_Globals.LogFile.Log(ex.ToString());
                Waze_Route_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
