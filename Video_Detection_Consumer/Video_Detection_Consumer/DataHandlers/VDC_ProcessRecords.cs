using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Video_Detection_Consumer
{
    class Video_Detection_Consumer_ProcessVRC
    {
        public static bool UpdateVDSRecord(SensorZone _SensorZone)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertvdsdata";
            int iLocalDeviceId = 0;
            
            
            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Video_Detection_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Video_Detection_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_device_id", NpgsqlDbType.Integer, _SensorZone.DeviceID);
                        cmdSQL.Parameters.AddWithValue("_native_device_id", NpgsqlDbType.Varchar, _SensorZone.NativeDeviceID);
                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Integer, _SensorZone.AgencyID);
                        cmdSQL.Parameters.AddWithValue("_device_type_id", NpgsqlDbType.Integer, _SensorZone.DeviceTypeID);
                        cmdSQL.Parameters.AddWithValue("_device_subtype_id", NpgsqlDbType.Integer, _SensorZone.DeviceSubTypeID);
                        cmdSQL.Parameters.AddWithValue("_device_name", NpgsqlDbType.Varchar, _SensorZone.DeviceName);
                        cmdSQL.Parameters.AddWithValue("_reporting_interval_in_minutes", NpgsqlDbType.Integer, _SensorZone.ReportingIntervalInMinutes);
                        cmdSQL.Parameters.AddWithValue("_occupancy", NpgsqlDbType.Double, _SensorZone.Occupancy);
                        cmdSQL.Parameters.AddWithValue("_volume", NpgsqlDbType.Integer, _SensorZone.Volume);
                        cmdSQL.Parameters.AddWithValue("_speed_in_mph", NpgsqlDbType.Integer, _SensorZone.SpeedInMph);
                        cmdSQL.Parameters.AddWithValue("_direction_id", NpgsqlDbType.Integer, _SensorZone.DirectionId);
                        cmdSQL.Parameters.AddWithValue("_interval_timestamp", NpgsqlDbType.Timestamp, _SensorZone.IntervalTimestamp);

                        iLocalDeviceId = (int)cmdSQL.ExecuteScalar();
                        _SensorZone.DeviceID = iLocalDeviceId;

                        if (Video_Detection_Consumer_Globals.dictSensorZones.ContainsKey(_SensorZone.NativeDeviceID + "|" + _SensorZone.AgencyID))
                        {
                            Video_Detection_Consumer_Globals.dictSensorZones.Remove(_SensorZone.NativeDeviceID + "|" + _SensorZone.AgencyID);
                        }

                        Video_Detection_Consumer_Globals.dictSensorZones.Add(_SensorZone.NativeDeviceID + "|" + _SensorZone.AgencyID, _SensorZone);

                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Video_Detection_Consumer_Globals.LogFile.LogErrorText("Error in UpdateVMCRecord().");
                Video_Detection_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Video_Detection_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
