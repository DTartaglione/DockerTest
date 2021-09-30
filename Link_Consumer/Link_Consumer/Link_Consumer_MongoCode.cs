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

namespace Link_Consumer
{
    class Link_Consumer_MongoCode
    {
        public static bool DoBatchUpdate(List<Link_Consumer_DataStructures.Link_Consumer_LinkData> lstData)
        {
            bool bSuccess = false;

            try
            {
                
                var _databaseName = MongoUrl.Create(Link_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(Link_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<Link_Consumer_DataStructures.Link_Consumer_LinkData> collTrafficData = _MongoDB.GetCollection<Link_Consumer_DataStructures.Link_Consumer_LinkData>("tbllinktrafficdata");
                List<Link_Consumer_DataStructures.Link_Consumer_LinkData> lstLinkData = new List<Link_Consumer_DataStructures.Link_Consumer_LinkData>();

                Link_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + Link_Consumer_Globals.sMongo_DBConn + " for insert.");
                collTrafficData.InsertMany(lstData);
                bSuccess = true;

                Link_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " link records into MongoDB document store " + collTrafficData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error inserting link records into MongoDB document store in Link_Consumer_MongoCode.DoBatchUpdate().");
                Link_Consumer_Globals.LogFile.Log(ex.ToString());
                Link_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
