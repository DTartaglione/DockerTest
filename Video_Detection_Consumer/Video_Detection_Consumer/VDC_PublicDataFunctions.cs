using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Video_Detection_Consumer
{
    class Video_Detection_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            if (GetSensorZoneInfo())
            {
                return true;
            }

            return false;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                Video_Detection_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Video_Detection_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Video_Detection_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!Video_Detection_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    Video_Detection_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            if ((Video_Detection_Consumer_Globals.iAgencyID = Video_Detection_Consumer_Globals.SetAgencyID(Video_Detection_Consumer_Globals.sAgencyName)) == 0)
                            {
                                Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + Video_Detection_Consumer_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                                return false;
                            }
                        }
                    }

                }

                Video_Detection_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error in Video_Detection_Consumer_PublicDataFunctions.GetAgencyInfo()");
                Video_Detection_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetSensorZoneInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "db_objects.sp_getvdssensorzones";
            SensorZone _SensorZone = new SensorZone();

            try
            {
                Video_Detection_Consumer_Globals.dictSensorZones.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Video_Detection_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Video_Detection_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = System.Data.CommandType.StoredProcedure;
                        cmdSQL.Parameters.AddWithValue("_device_type_id", NpgsqlTypes.NpgsqlDbType.Integer, Video_Detection_Consumer_Globals.iSensorZoneDeviceTypeID);
                        cmdSQL.Parameters.AddWithValue("_device_subtype_id", NpgsqlTypes.NpgsqlDbType.Integer, Video_Detection_Consumer_Globals.iSensorZoneDeviceSubtypeID);

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                _SensorZone = new SensorZone();
                                _SensorZone.DeviceID = Convert.ToInt32(rsRead[0].ToString());
                                _SensorZone.DeviceTypeID = Convert.ToInt32(rsRead[1].ToString());
                                _SensorZone.DeviceType = rsRead[2].ToString();
                                _SensorZone.DeviceSubTypeID = Convert.ToInt32(rsRead[3].ToString());
                                _SensorZone.DeviceSubType = rsRead[4].ToString();
                                _SensorZone.DeviceName = rsRead[5].ToString();
                                _SensorZone.AgencyID = Convert.ToInt32(rsRead[6]);
                                _SensorZone.NativeDeviceID = rsRead[7].ToString();

                                if (!Video_Detection_Consumer_Globals.dictSensorZones.ContainsKey(_SensorZone.NativeDeviceID + "|" + _SensorZone.AgencyID))
                                {
                                    Video_Detection_Consumer_Globals.dictSensorZones.Add(_SensorZone.NativeDeviceID + "|" + _SensorZone.AgencyID, _SensorZone);
                                }

                            }
                        }

                        Video_Detection_Consumer_Globals.LogFile.Log("Successfully read sensor zone data in GetSensorZoneInfo().");
                        bSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error in Sensys_Data_Producer_PublicDataFunctions.GetSensorZoneInfo()");
                Video_Detection_Consumer_Globals.LogFile.LogError(ex);
                bSuccess = false;
            }

            return bSuccess;
        }

        //public static bool GetSensorDeviceInfo()
        //{
        //    bool bSuccess = false;
        //    NpgsqlDataReader rsRead = null;
        //    string sSQL = "db_objects.sp_getsensyssensors";
        //    Sensor _Sensor = new Sensor();

        //    try
        //    {
        //        Video_Detection_Consumer_Globals.dictSensors.Clear();

        //        using (NpgsqlConnection sConn = new NpgsqlConnection(Video_Detection_Consumer_Globals.sPG_MOD_DBConn))
        //        {
        //            sConn.Open();

        //            using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
        //            {
        //                cmdSQL.CommandTimeout = Video_Detection_Consumer_Globals.iDBCommandTimeoutInSeconds;
        //                cmdSQL.CommandType = System.Data.CommandType.StoredProcedure;

        //                rsRead = cmdSQL.ExecuteReader();

        //                if (rsRead.HasRows)
        //                {
        //                    while (rsRead.Read())
        //                    {
        //                        _Sensor = new Sensor();
        //                        _Sensor.DeviceID = Convert.ToInt32(rsRead[0].ToString());
        //                        _Sensor.ParentDeviceID = Convert.ToInt32(rsRead[1].ToString());

        //                        if (!Video_Detection_Consumer_Globals.dictSensors.ContainsKey(_Sensor.DeviceID))
        //                        {
        //                            Video_Detection_Consumer_Globals.dictSensors.Add(_Sensor.DeviceID, _Sensor);
        //                        }
        //                    }

        //                    Video_Detection_Consumer_Globals.LogFile.Log("Successfully read sensor device data in GetSensorDeviceInfo().");

        //                }
        //                else
        //                {
        //                    Video_Detection_Consumer_Globals.LogFile.Log("No sensor data exists in tblatrdata yet. No dynamic sensor data loaded. Continue.");
        //                }

        //                bSuccess = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error in Video_Detection_Consumer_PublicDataFunctions.GetSensorDeviceInfo()");
        //        Video_Detection_Consumer_Globals.LogFile.LogError(ex);
        //        bSuccess = false;
        //    }

        //    return bSuccess;
        //}

    }
}
