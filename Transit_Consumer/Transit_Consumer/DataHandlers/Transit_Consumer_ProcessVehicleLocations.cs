using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Transit_Consumer
{
    class Transit_Consumer_ProcessVehicleLocations
    {
        public static bool ProcessVehicleLocations(List<VehicleLocations> lstVehicleLocations)
        {
            bool bResult = true;
            DateTime dtStoredDateTime = new DateTime();
            StringBuilder sbVehicleLocationsOut = new StringBuilder();

            foreach (VehicleLocations _VehicleLocation in lstVehicleLocations)
            {
                try
                {
                    if (Transit_Consumer_Globals.dictStoredVehicleLocations.ContainsKey(_VehicleLocation.Id + "|" +
                            _VehicleLocation.TripId + "|" + _VehicleLocation.Route + "|" + _VehicleLocation.StopId))
                    {
                        //vehicle location exists. check timestamp to see if updated
                        dtStoredDateTime = Transit_Consumer_Globals.dictStoredVehicleLocations[_VehicleLocation.Id + "|" +
                            _VehicleLocation.TripId + "|" + _VehicleLocation.Route + "|" + _VehicleLocation.StopId];

                        if (_VehicleLocation.LastUpdate <= dtStoredDateTime)
                        {
                            //vehicle position older or same as in database. skip it
                            continue;
                        }
                    }

                    //process the update
                    if (!InsertVehicleLocations(_VehicleLocation))
                    {
                        Transit_Consumer_Globals.LogFile.LogErrorText("Vehicle location failed to insert. See logs for details.");
                    }
                }
                catch (Exception ex)
                {
                    Transit_Consumer_Globals.LogFile.LogErrorText("Error in ProcessVehicleLocations().");
                    Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                    Transit_Consumer_Globals.LogFile.LogError(ex);
                    bResult = false;
                }
            }

            if (Transit_Consumer_PublicDataFunctions.GetCurrentVehiclePositionData())
            {
                Transit_Consumer_Globals.LogFile.Log("Refreshed vehicle location data storage in memory.");
            }

            return bResult;

        }

        public static bool InsertVehicleLocations(VehicleLocations _VehicleLocation)
        {
            bool bResult = false;
            string sSQL = "transitds.sp_updatevehicleposition";
            Coordinate _Coords = new Coordinate();
            string sVehicleLabel = "";

            try
            {
                _Coords = _VehicleLocation.Coords;
                sVehicleLabel = _VehicleLocation.Route + "||" + _VehicleLocation.Id;

                using (NpgsqlConnection sConn = new NpgsqlConnection(Transit_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Transit_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_trip_id", NpgsqlDbType.Varchar, (object)_VehicleLocation.TripId ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_route_id", NpgsqlDbType.Varchar, _VehicleLocation.Route);
                        cmdSQL.Parameters.AddWithValue("_stop_id", NpgsqlDbType.Integer, (object)_VehicleLocation.StopId ?? 0);
                        cmdSQL.Parameters.AddWithValue("_stop_code", NpgsqlDbType.Varchar, (object)_VehicleLocation.StopId ?? "0");
                        cmdSQL.Parameters.AddWithValue("_vehicle_id", NpgsqlDbType.Varchar, _VehicleLocation.Id);
                        cmdSQL.Parameters.AddWithValue("_direction_id", NpgsqlDbType.Integer, (object)_VehicleLocation.DirectionId ?? 0);
                        cmdSQL.Parameters.AddWithValue("_latitude", NpgsqlDbType.Double, _Coords.Latitude);
                        cmdSQL.Parameters.AddWithValue("_longitude", NpgsqlDbType.Double, _Coords.Longitude);
                        cmdSQL.Parameters.AddWithValue("_heading", NpgsqlDbType.Double, Convert.ToDouble(_VehicleLocation.Heading));
                        cmdSQL.Parameters.AddWithValue("_speed_km_hr", NpgsqlDbType.Double, Convert.ToDouble(_VehicleLocation.SpeedKmHr));
                        cmdSQL.Parameters.AddWithValue("_odometer", NpgsqlDbType.Double, Convert.ToDouble(0));
                        cmdSQL.Parameters.AddWithValue("_license_plate", NpgsqlDbType.Varchar, DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_vehicle_label", NpgsqlDbType.Varchar, sVehicleLabel);
                        cmdSQL.Parameters.AddWithValue("_start_date", NpgsqlDbType.Varchar, (object)_VehicleLocation.StartDate ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _VehicleLocation.LastUpdate);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Transit_Consumer_Globals.LogFile.LogErrorText("Error in InsertVehicleLocation().");
                Transit_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Transit_Consumer_Globals.LogFile.LogError(ex);
                bResult = false;
            }

            return bResult;
        }

    }
}
