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

namespace VMS_Consumer
{
    class VMS_Consumer_MongoCode
    {
        public static bool DoBatchUpdate(List<VMSData> lstData)
        {
            bool bSuccess = false;

            try
            {
                
                var _databaseName = MongoUrl.Create(VMS_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(VMS_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<VMSData> collData = _MongoDB.GetCollection<VMSData>("tblvms");
                List<VMSData> lstVMSData = new List<VMSData>();

                VMS_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + VMS_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                VMS_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                VMS_Consumer_Globals.LogFile.LogErrorText("Error inserting records into MongoDB document store in DoBatchUpdate().");
                VMS_Consumer_Globals.LogFile.Log(ex.ToString());
                VMS_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
