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

namespace Marine_Traffic_Data_Consumer
{
    class MTDC_MongoCode
    {
        public static bool DoBatchUpdate(List<BerthItem> lstData)
        {
            bool bSuccess = false;

            try
            {
                
                var _databaseName = MongoUrl.Create(MTDC_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(MTDC_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<BerthItem> collData = _MongoDB.GetCollection<BerthItem>("tblmarinetrafficdata");
                List<BerthInfo> lstEventData = new List<BerthInfo>();

                MTDC_Globals.LogFile.Log("Connecting to Mongo host " + MTDC_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                MTDC_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                MTDC_Globals.LogFile.LogErrorText("Error inserting records into MongoDB document store in DoBatchUpdate().");
                MTDC_Globals.LogFile.Log(ex.ToString());
                MTDC_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
