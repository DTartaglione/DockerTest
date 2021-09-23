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

namespace Weather_DI
{
    class Weather_DI_MongoCode
    {
        public static bool DoBatchUpdate(List<Weather_DI_DataStructures.Periods> lstData)
        {
            bool bSuccess = false;

            try
            {

                var _databaseName = MongoUrl.Create(Weather_DI_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(Weather_DI_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<Weather_DI_DataStructures.Periods> collWeatherData = _MongoDB.GetCollection<Weather_DI_DataStructures.Periods>("tblweather");
                List<Weather_DI_DataStructures.Periods> lstLinkData = new List<Weather_DI_DataStructures.Periods>();

                Weather_DI_Globals.LogFile.Log("Connecting to Mongo host " + Weather_DI_Globals.sMongo_DBConn + " for insert.");
                collWeatherData.InsertMany(lstData);
                bSuccess = true;

                Weather_DI_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " weather records into MongoDB document store " + collWeatherData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                Weather_DI_Globals.LogFile.LogErrorText("Error inserting weather records into MongoDB document store in Weather_DI_MongoCode.DoBatchUpdate().");
                Weather_DI_Globals.LogFile.Log(ex.ToString());
                Weather_DI_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
