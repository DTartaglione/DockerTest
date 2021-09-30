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

namespace CCTV_Consumer
{
    class CCTV_Consumer_MongoCode
    {
        public static bool DoBatchUpdate(List<CCTVData> lstData)
        {
            bool bSuccess = false;

            try
            {
                
                var _databaseName = MongoUrl.Create(CCTV_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(CCTV_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<CCTVData> collData = _MongoDB.GetCollection<CCTVData>("tblcctv");
                List<CCTVData> lstCCTVData = new List<CCTVData>();

                CCTV_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + CCTV_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                CCTV_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                CCTV_Consumer_Globals.LogFile.LogErrorText("Error inserting records into MongoDB document store in DoBatchUpdate().");
                CCTV_Consumer_Globals.LogFile.Log(ex.ToString());
                CCTV_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
