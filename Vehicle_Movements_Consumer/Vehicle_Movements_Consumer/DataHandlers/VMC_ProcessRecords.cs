using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Vehicle_Movements_Consumer
{
    class Vehicle_Movements_Consumer_ProcessVRC
    {
        public static bool UpdateVMCRecord(VehicleMovement _VehicleMovement)
        {
            bool bResult = false;
            string sSQL = "wejods.sp_vehiclemovement";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Vehicle_Movements_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Vehicle_Movements_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_data_point_id", NpgsqlDbType.Varchar, _VehicleMovement.DataPointId);
                        cmdSQL.Parameters.AddWithValue("_journey_id", NpgsqlDbType.Varchar, _VehicleMovement.JourneyId);
                        cmdSQL.Parameters.AddWithValue("_vehicle_id_num", NpgsqlDbType.Varchar, _VehicleMovement.VehicleIdNum);
                        cmdSQL.Parameters.AddWithValue("_vehicle_latitude", NpgsqlDbType.Double, _VehicleMovement.VehicleLatitude);
                        cmdSQL.Parameters.AddWithValue("_vehicle_longitude", NpgsqlDbType.Double, _VehicleMovement.VehicleLongitude);
                        cmdSQL.Parameters.AddWithValue("_geo_hash", NpgsqlDbType.Varchar, _VehicleMovement.GeoHash);
                        cmdSQL.Parameters.AddWithValue("_speed_in_mph", NpgsqlDbType.Double, _VehicleMovement.Speed);
                        cmdSQL.Parameters.AddWithValue("_heading_degrees", NpgsqlDbType.Double, _VehicleMovement.Heading);
                        cmdSQL.Parameters.AddWithValue("_ignition_status", NpgsqlDbType.Varchar, _VehicleMovement.IgntitionStatus);
                        cmdSQL.Parameters.AddWithValue("_captured_timestamp", NpgsqlDbType.Timestamp, _VehicleMovement.CapturedTimestamp);
                        cmdSQL.Parameters.AddWithValue("_vehicle_make", NpgsqlDbType.Varchar, (object)_VehicleMovement.VehicleMake ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_vehicle_model", NpgsqlDbType.Varchar, (object)_VehicleMovement.VehicleModel ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_vehicle_year", NpgsqlDbType.Integer, (object)_VehicleMovement.VehicleYear ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _VehicleMovement.LastUpdate);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Vehicle_Movements_Consumer_Globals.LogFile.LogErrorText("Error in UpdateVMCRecord().");
                Vehicle_Movements_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Vehicle_Movements_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
