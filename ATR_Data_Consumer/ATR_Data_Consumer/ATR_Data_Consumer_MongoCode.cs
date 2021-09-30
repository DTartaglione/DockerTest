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

namespace ATR_Data_Consumer
{
    class ATR_Data_Consumer_MongoCode
    {
        public static bool DoBatchUpdate(List<ATRData> lstData)
        {
            bool bSuccess = false;

            try
            {
                
                var _databaseName = MongoUrl.Create(ATR_Data_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(ATR_Data_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<ATRData> collData = _MongoDB.GetCollection<ATRData>("tblatrdata");
                List<ATRData> lstEventData = new List<ATRData>();

                ATR_Data_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + ATR_Data_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                ATR_Data_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error inserting records into MongoDB document store in DoBatchUpdate().");
                ATR_Data_Consumer_Globals.LogFile.Log(ex.ToString());
                ATR_Data_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
