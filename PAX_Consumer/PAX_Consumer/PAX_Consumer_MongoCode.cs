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

namespace PAX_Consumer
{
    class PAX_Consumer_MongoCode
    {
        public static bool DoBatchUpdate(List<PAX> lstData)
        {
            bool bSuccess = false;

            try
            {
                var _databaseName = MongoUrl.Create(PAX_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(PAX_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<PAX> collData = _MongoDB.GetCollection<PAX>("tblpaxvolume");
                List<PAX> lstRouteData = new List<PAX>();

                PAX_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + PAX_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                PAX_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error inserting records into MongoDB document store in PAX_Consumer_MongoCode.DoBatchUpdate().");
                PAX_Consumer_Globals.LogFile.Log(ex.ToString());
                PAX_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }

    }
}
