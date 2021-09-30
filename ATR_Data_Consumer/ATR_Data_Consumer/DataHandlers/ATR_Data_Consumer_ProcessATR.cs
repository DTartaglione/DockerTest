using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace ATR_Data_Consumer
{
    class ATR_Data_Consumer_ProcessATR
    {
        public static bool UpdateATRRecord(ATRData _ATRData)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertatrdata";
            int iResult = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(ATR_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = ATR_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_device_id", NpgsqlDbType.Integer, _ATRData.DeviceID);
                        cmdSQL.Parameters.AddWithValue("_device_type_id", NpgsqlDbType.Integer, _ATRData.DeviceTypeID);
                        cmdSQL.Parameters.AddWithValue("_device_subtype_id", NpgsqlDbType.Integer, _ATRData.DeviceSubTypeID);
                        cmdSQL.Parameters.AddWithValue("_name", NpgsqlDbType.Varchar, _ATRData.DeviceName);
                        cmdSQL.Parameters.AddWithValue("_description", NpgsqlDbType.Varchar, _ATRData.DeviceName);
                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Integer, _ATRData.AgencyID);
                        cmdSQL.Parameters.AddWithValue("_parent_device_id", NpgsqlDbType.Integer, _ATRData.DeviceParentID);
                        cmdSQL.Parameters.AddWithValue("_reporting_interval_in_minutes", NpgsqlDbType.Integer, _ATRData.ReportingIntervalInMinutes);
                        cmdSQL.Parameters.AddWithValue("_volume", NpgsqlDbType.Integer, _ATRData.Volume);
                        cmdSQL.Parameters.AddWithValue("_speed_in_mph", NpgsqlDbType.Integer, _ATRData.SpeedInMph);
                        cmdSQL.Parameters.AddWithValue("_occupancy", NpgsqlDbType.Double, _ATRData.Occupancy);
                        cmdSQL.Parameters.AddWithValue("_interval_end", NpgsqlDbType.TimestampTZ, _ATRData.IntervalEnd);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.TimestampTZ, _ATRData.LastUpdate);

                        iResult = (Int32)cmdSQL.ExecuteScalar();

                        ////Add to Sensor ID map - first clear original if exists
                        if (ATR_Data_Consumer_Globals.dictSensorMapping.ContainsKey(_ATRData.AgencyID + "|" + _ATRData.DeviceName 
                            + "|" + _ATRData.DeviceTypeID + "|" + _ATRData.DeviceSubTypeID))
                        {
                            ATR_Data_Consumer_Globals.dictSensorMapping.Remove(_ATRData.AgencyID + "|" + _ATRData.DeviceName
                            + "|" + _ATRData.DeviceTypeID + "|" + _ATRData.DeviceSubTypeID);
                        }

                        ATR_Data_Consumer_Globals.dictSensorMapping.Add(_ATRData.AgencyID + "|" + _ATRData.DeviceName
                            + "|" + _ATRData.DeviceTypeID + "|" + _ATRData.DeviceSubTypeID, iResult);
                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                ATR_Data_Consumer_Globals.LogFile.LogErrorText("Error in ATR_Data_Consumer_ProcessATR().");
                ATR_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                ATR_Data_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
