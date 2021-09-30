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

namespace Aviation_DI
{
    class Aviation_DI_MongoCode
    {
        public static bool DoBatchUpdate(List<Aviation_DI_FlightData> lstData)
        {
            bool bSuccess = false;

            try
            {
                var _databaseName = MongoUrl.Create(Aviation_DI_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(Aviation_DI_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<Aviation_DI_FlightData> collAviationData = _MongoDB.GetCollection<Aviation_DI_FlightData>("tblflightdata");
                List<Aviation_DI_FlightData> lstAviationData = new List<Aviation_DI_FlightData>();

                Aviation_DI_Globals.LogFile.Log("Connecting to Mongo host " + Aviation_DI_Globals.sMongo_DBConn + " for insert.");
                collAviationData.InsertMany(lstData);
                bSuccess = true;

                Aviation_DI_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " Aviation records into MongoDB document store " + collAviationData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                Aviation_DI_Globals.LogFile.LogErrorText("Error inserting Aviation records into MongoDB document store in Aviation_DI_MongoCode.DoBatchUpdate().");
                Aviation_DI_Globals.LogFile.Log(ex.ToString());
                Aviation_DI_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
