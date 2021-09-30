using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Vehicle_Record_Consumer
{
    class Vehicle_Record_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            //if (GetSensorZoneInfo())
            //{
            //    if (GetSensorDeviceInfo())
            //    {
            return true;
            //}
        //}
        //return true;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                Vehicle_Record_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Vehicle_Record_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Vehicle_Record_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!Vehicle_Record_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    Vehicle_Record_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((Vehicle_Record_Consumer_Globals.iAgencyID = Vehicle_Record_Consumer_Globals.SetAgencyID(Vehicle_Record_Consumer_Globals.sAgencyName)) == 0)
                            {
                                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + Vehicle_Record_Consumer_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                                return false;
                            }
                        }
                    }

                }

                Vehicle_Record_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error in Vehicle_Record_Consumer_PublicDataFunctions.GetAgencyInfo()");
                Vehicle_Record_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        //public static bool GetSensorZoneInfo()
        //{
        //    bool bSuccess = false;
        //    NpgsqlDataReader rsRead = null;
        //    string sSQL = "db_objects.sp_getsensyssensorzones";
        //    SensorZone _SensorZone = new SensorZone();

        //    try
        //    {
        //        Vehicle_Record_Consumer_Globals.dictSensorZones.Clear();

        //        using (NpgsqlConnection sConn = new NpgsqlConnection(Vehicle_Record_Consumer_Globals.sPG_MOD_DBConn))
        //        {
        //            sConn.Open();

        //            using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
        //            {
        //                cmdSQL.CommandTimeout = Vehicle_Record_Consumer_Globals.iDBCommandTimeoutInSeconds;
        //                cmdSQL.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlTypes.NpgsqlDbType.Integer, Vehicle_Record_Consumer_Globals.iAgencyID);

        //                rsRead = cmdSQL.ExecuteReader();

        //                if (rsRead.HasRows)
        //                {
        //                    while (rsRead.Read())
        //                    {
        //                        _SensorZone = new SensorZone();
        //                        _SensorZone.DeviceID = Convert.ToInt32(rsRead[0].ToString());
        //                        _SensorZone.DeviceTypeID = Convert.ToInt32(rsRead[1].ToString());
        //                        _SensorZone.DeviceType = rsRead[2].ToString();
        //                        _SensorZone.DeviceSubTypeID = Convert.ToInt32(rsRead[3].ToString());
        //                        _SensorZone.DeviceSubType = rsRead[4].ToString();
        //                        _SensorZone.DeviceName = rsRead[5].ToString();

        //                        if (!Vehicle_Record_Consumer_Globals.dictSensorZones.ContainsKey(_SensorZone.DeviceName))
        //                        {
        //                            Vehicle_Record_Consumer_Globals.dictSensorZones.Add(_SensorZone.DeviceName, _SensorZone);
        //                        }
        //                    }
        //                }

        //                Vehicle_Record_Consumer_Globals.LogFile.Log("Successfully read sensor zone data in GetSensorZoneInfo().");
        //                bSuccess = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error in Vehicle_Record_Consumer_PublicDataFunctions.GetSensorZoneInfo()");
        //        Vehicle_Record_Consumer_Globals.LogFile.LogError(ex);
        //        bSuccess = false;
        //    }

        //    return bSuccess;
        //}

        //public static bool GetSensorDeviceInfo()
        //{
        //    bool bSuccess = false;
        //    NpgsqlDataReader rsRead = null;
        //    string sSQL = "db_objects.sp_getsensyssensors";
        //    Sensor _Sensor = new Sensor();

        //    try
        //    {
        //        Vehicle_Record_Consumer_Globals.dictSensors.Clear();

        //        using (NpgsqlConnection sConn = new NpgsqlConnection(Vehicle_Record_Consumer_Globals.sPG_MOD_DBConn))
        //        {
        //            sConn.Open();

        //            using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
        //            {
        //                cmdSQL.CommandTimeout = Vehicle_Record_Consumer_Globals.iDBCommandTimeoutInSeconds;
        //                cmdSQL.CommandType = System.Data.CommandType.StoredProcedure;

        //                rsRead = cmdSQL.ExecuteReader();

        //                if (rsRead.HasRows)
        //                {
        //                    while (rsRead.Read())
        //                    {
        //                        _Sensor = new Sensor();
        //                        _Sensor.DeviceID = Convert.ToInt32(rsRead[0].ToString());
        //                        _Sensor.ParentDeviceID = Convert.ToInt32(rsRead[1].ToString());

        //                        if (!Vehicle_Record_Consumer_Globals.dictSensors.ContainsKey(_Sensor.DeviceID))
        //                        {
        //                            Vehicle_Record_Consumer_Globals.dictSensors.Add(_Sensor.DeviceID, _Sensor);
        //                        }
        //                    }

        //                    Vehicle_Record_Consumer_Globals.LogFile.Log("Successfully read sensor device data in GetSensorDeviceInfo().");

        //                }
        //                else
        //                {
        //                    Vehicle_Record_Consumer_Globals.LogFile.Log("No sensor data exists in tblatrdata yet. No dynamic sensor data loaded. Continue.");
        //                }

        //                bSuccess = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error in Vehicle_Record_Consumer_PublicDataFunctions.GetSensorDeviceInfo()");
        //        Vehicle_Record_Consumer_Globals.LogFile.LogError(ex);
        //        bSuccess = false;
        //    }

        //    return bSuccess;
        //}

    }
}
