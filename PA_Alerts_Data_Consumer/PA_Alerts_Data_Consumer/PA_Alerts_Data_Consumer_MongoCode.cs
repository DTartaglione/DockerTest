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

namespace PA_Alerts_Data_Consumer
{
    class PA_Alerts_Data_Consumer_MongoCode
    {
        public static bool DoBatchUpdateAOCTrafficStatus(List<AOC_Traffic_Status> lstData)
        {
            bool bSuccess = false;

            try
            {
                var _databaseName = MongoUrl.Create(PA_Alerts_Data_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(PA_Alerts_Data_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<AOC_Traffic_Status> collData = _MongoDB.GetCollection<AOC_Traffic_Status>("aoc_status_code");
                List<AOC_Traffic_Status> lstRouteData = new List<AOC_Traffic_Status>();

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + PA_Alerts_Data_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " AOC status code records into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error inserting AOC status code records into MongoDB document store in PA_Alerts_Data_Consumer_MongoCode.DoBatchUpdateAOCTrafficStatus().");
                PA_Alerts_Data_Consumer_Globals.LogFile.Log(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }

        public static bool DoBatchUpdateAlerts(List<Alerts> lstData)
        {
            bool bSuccess = false;

            try
            {
                var _databaseName = MongoUrl.Create(PA_Alerts_Data_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(PA_Alerts_Data_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<Alerts> collData = _MongoDB.GetCollection<Alerts>("pa_alerts");
                List<Alerts> lstRouteData = new List<Alerts>();

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + PA_Alerts_Data_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " PA alerts into MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error inserting PA alerts into MongoDB document store in PA_Alerts_Data_Consumer_MongoCode.DoBatchUpdateAlerts().");
                PA_Alerts_Data_Consumer_Globals.LogFile.Log(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }

        public static bool DoBatchUpdatePAXVolume(List<PAXUpdate> lstData)
        {
            bool bSuccess = false;

            try
            {
                var _databaseName = MongoUrl.Create(PA_Alerts_Data_Consumer_Globals.sMongo_DBConn).DatabaseName;
                MongoClient _MongoClient = new MongoClient(PA_Alerts_Data_Consumer_Globals.sMongo_DBConn);
                IMongoDatabase _MongoDB = _MongoClient.GetDatabase(_databaseName);
                IMongoCollection<PAXUpdate> collData = _MongoDB.GetCollection<PAXUpdate>("pax_volume");
                List<PAXUpdate> lstRouteData = new List<PAXUpdate>();

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Connecting to Mongo host " + PA_Alerts_Data_Consumer_Globals.sMongo_DBConn + " for insert.");
                collData.InsertMany(lstData);
                bSuccess = true;

                PA_Alerts_Data_Consumer_Globals.LogFile.Log("Successfully inserted " + lstData.Count.ToString() + " PAX volume entries MongoDB document store " + collData.CollectionNamespace.ToString());
            }
            catch (Exception ex)
            {
                PA_Alerts_Data_Consumer_Globals.LogFile.LogErrorText("Error inserting PAX volume info into MongoDB document store in PA_Alerts_Data_Consumer_MongoCode.DoBatchUpdatePAXVolume().");
                PA_Alerts_Data_Consumer_Globals.LogFile.Log(ex.ToString());
                PA_Alerts_Data_Consumer_Globals.LogFile.LogError(ex);

                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
