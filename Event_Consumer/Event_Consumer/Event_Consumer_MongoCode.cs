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

namespace Event_Consumer
{
    class Event_Consumer_MongoCode
    {
        public static bool DoBatchUpdate(List<EventData> lstData)
        {
            bool bSuccess = false;

            try
            {
                
                var _databaseName = MongoUrl.Create(Event_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(Event_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<EventData> collData = _MongoDB.GetCollection<EventData>("tbleventdata");
                List<EventData> lstEventData = new List<EventData>();

                Event_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + Event_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                Event_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                Event_Consumer_Globals.LogFile.LogErrorText("Error inserting records into MongoDB document store in DoBatchUpdate().");
                Event_Consumer_Globals.LogFile.Log(ex.ToString());
                Event_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
