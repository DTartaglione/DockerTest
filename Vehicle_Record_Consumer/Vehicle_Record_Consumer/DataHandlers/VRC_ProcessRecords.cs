using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Vehicle_Record_Consumer
{
    class Vehicle_Record_Consumer_ProcessVRC
    {
        public static bool UpdateVRCRecord(VehicleRecord _VehicleRecord)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertvehiclerecorddata";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Vehicle_Record_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Vehicle_Record_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_device_id", NpgsqlDbType.Integer, _VehicleRecord.DeviceID);
                        cmdSQL.Parameters.AddWithValue("_speed_in_mph", NpgsqlDbType.Integer, _VehicleRecord.Speed);
                        cmdSQL.Parameters.AddWithValue("_vehicle_length_in_feet", NpgsqlDbType.Integer, _VehicleRecord.VehicleLength);
                        cmdSQL.Parameters.AddWithValue("_gap_in_seconds", NpgsqlDbType.Double, _VehicleRecord.Gap);
                        cmdSQL.Parameters.AddWithValue("_record_timestamp", NpgsqlDbType.Timestamp, _VehicleRecord.RecordTimestamp);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _VehicleRecord.LastUpdate);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText("Error in UpdateVRCRecord().");
                Vehicle_Record_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Vehicle_Record_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
