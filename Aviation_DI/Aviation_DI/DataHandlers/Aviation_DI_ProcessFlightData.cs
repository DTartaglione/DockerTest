using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Aviation_DI
{
    class Aviation_DI_ProcessFlightData
    {

        public static bool UpdateFlightData(Aviation_DI_FlightData _FlightData)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_MOD_DI_updateflightdata";

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Aviation_DI_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Aviation_DI_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_source_flight_id", NpgsqlDbType.Varchar, _FlightData.SourceFlightID);
                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Integer, _FlightData.AgencyID);
                        cmdSQL.Parameters.AddWithValue("_airline_code", NpgsqlDbType.Varchar, _FlightData.AirlineCode);
                        cmdSQL.Parameters.AddWithValue("_airline_name", NpgsqlDbType.Varchar, _FlightData.AirlineName);
                        cmdSQL.Parameters.AddWithValue("_flight_number", NpgsqlDbType.Varchar, _FlightData.FlightNum);
                        cmdSQL.Parameters.AddWithValue("_origin_airport_code", NpgsqlDbType.Varchar, _FlightData.OriginAirportCode);
                        cmdSQL.Parameters.AddWithValue("_destination_airport_code", NpgsqlDbType.Varchar, _FlightData.DestinationAirportCode);
                        cmdSQL.Parameters.AddWithValue("_aircraft_type", NpgsqlDbType.Varchar, _FlightData.AircraftType);
                        cmdSQL.Parameters.AddWithValue("_current_speed", NpgsqlDbType.Double, _FlightData.Speed);
                        cmdSQL.Parameters.AddWithValue("_current_altitude", NpgsqlDbType.Double, _FlightData.Altitude);
                        cmdSQL.Parameters.AddWithValue("_flight_status", NpgsqlDbType.Varchar, _FlightData.Status);
                        cmdSQL.Parameters.AddWithValue("_scheduled_departure_datetime", NpgsqlDbType.Varchar, (object)_FlightData.ScheduledDeparture ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_estimated_departure_datetime", NpgsqlDbType.Varchar, (object)_FlightData.EstimatedDeparture ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_actual_departure_datetime", NpgsqlDbType.Varchar, (object)_FlightData.ActualDeparture ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_departure_gate", NpgsqlDbType.Varchar, (object)_FlightData.DepartureGate ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_scheduled_arrival_datetime", NpgsqlDbType.Varchar, (object)_FlightData.ScheduledArrival ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_estimated_arrival_datetime", NpgsqlDbType.Varchar, (object)_FlightData.EstimatedArrival ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_actual_arrival_datetime", NpgsqlDbType.Varchar, (object)_FlightData.ActualArrival ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_arrival_gate", NpgsqlDbType.Varchar, (object)_FlightData.ArrivalGate ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_latitude", NpgsqlDbType.Double, _FlightData.Latitude);
                        cmdSQL.Parameters.AddWithValue("_longitude", NpgsqlDbType.Double, _FlightData.Longitude);
                        cmdSQL.Parameters.AddWithValue("_direction", NpgsqlDbType.Double, _FlightData.Direction);
                        cmdSQL.Parameters.AddWithValue("_departure_delay", NpgsqlDbType.Integer, _FlightData.DepartureDelay);
                        cmdSQL.Parameters.AddWithValue("_arrival_delay", NpgsqlDbType.Integer, _FlightData.ArrivalDelay);
                        cmdSQL.Parameters.AddWithValue("_num_seats", NpgsqlDbType.Integer, _FlightData.NumSeats);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _FlightData.LastUpdate);
                        cmdSQL.Parameters.AddWithValue("_flight_type_id", NpgsqlDbType.Integer, _FlightData.FlightTypeID);
                        cmdSQL.Parameters.AddWithValue("_code_shares", NpgsqlDbType.Varchar, (object)_FlightData.CodeShares ?? DBNull.Value);

                        cmdSQL.ExecuteNonQuery();

                        bResult = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Aviation_DI_Globals.LogFile.LogErrorText("Error in Aviation_DI_Aviation_ProcessFlightData.UpdateFlightData()");
                Aviation_DI_Globals.LogFile.LogErrorText(ex.ToString());
                Aviation_DI_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

    }
}
