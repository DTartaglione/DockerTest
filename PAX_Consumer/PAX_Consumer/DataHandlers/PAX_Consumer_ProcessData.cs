using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using NpgsqlTypes;


namespace PAX_Consumer
{
    class PAX_Consumer_ProcessData
    {

        public static bool UpdatePAXData(PAX _PAX)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertpaxdata";
            //0 for hourly, 1 for daily
            int iDataTimeType = 0;

            
            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    foreach (PAXVolume _PAXVolume in _PAX.Volume)
                    {
                        //Daily emails will have PAXHourTimestamp set to 1/1/0000 12:00:00 AM
                        if (_PAXVolume.PAXHourTimestamp < DateTime.Parse("1/1/2000 12:00:00 am"))
                        {
                            iDataTimeType = 1;
                        }

                        using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                        {
                            cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;
                            cmdSQL.CommandType = CommandType.StoredProcedure;

                            cmdSQL.Parameters.AddWithValue("_data_time_type", NpgsqlDbType.Integer, iDataTimeType);
                            cmdSQL.Parameters.AddWithValue("_airline_id", NpgsqlDbType.Integer, _PAX.AirlineID);
                            cmdSQL.Parameters.AddWithValue("_airport_id", NpgsqlDbType.Integer, _PAX.AirportID);
                            cmdSQL.Parameters.AddWithValue("_pax_type_id", NpgsqlDbType.Integer, _PAX.PAXTypeID);
                            if (iDataTimeType == 0)
                            {
                                cmdSQL.Parameters.AddWithValue("_pax_timestamp", NpgsqlDbType.Timestamp, _PAXVolume.PAXHourTimestamp);
                            }
                            else
                            {
                                cmdSQL.Parameters.AddWithValue("_pax_timestamp", NpgsqlDbType.Timestamp, _PAX.PAXDate);
                            }
                            cmdSQL.Parameters.AddWithValue("_volume", NpgsqlDbType.Integer, _PAXVolume.Volume);
                            cmdSQL.Parameters.AddWithValue("_flight_type_id", NpgsqlDbType.Integer, _PAX.FlightType);
                            cmdSQL.Parameters.AddWithValue("_flight_count", NpgsqlDbType.Integer, _PAXVolume.NumFlights);
                            cmdSQL.Parameters.AddWithValue("_load_factor_percentage", NpgsqlDbType.Numeric, _PAX.LoadFactorPercentage);
                            cmdSQL.Parameters.AddWithValue("_extra_info", NpgsqlDbType.Varchar, _PAX.ExtraInfo);
                            cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _PAX.LastUpdate);

                            cmdSQL.ExecuteNonQuery();

                            bResult = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_Aviation_ProcessData.UpdateFlightData()");
                PAX_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        public static bool UpdateTSAEnplanementData(PAX _Enplanement)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertenplanementdata";
            //0 for hourly, 1 for daily
            int iDataTimeType = 0;
            int iTSALevel = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    foreach (PAXVolume _PAXVolume in _Enplanement.Volume)
                    {
                        if (_PAXVolume.PAXHourTimestamp < DateTime.Parse("1/1/2000 12:00:00 am"))
                        {
                            iDataTimeType = 1;
                        }

                        if (_PAXVolume.Volume < PAX_Consumer_Globals.iTSALevelOneMax)
                        {
                            iTSALevel = 1;
                        }
                        else if (_PAXVolume.Volume < PAX_Consumer_Globals.iTSALevelTwoMax)
                        {
                            iTSALevel = 2;
                        }
                        else if (_PAXVolume.Volume < PAX_Consumer_Globals.iTSALevelThreeMax)
                        {
                            iTSALevel = 3;
                        }
                        else
                        {
                            iTSALevel = 4;
                        }

                        using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                        {
                            cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;
                            cmdSQL.CommandType = CommandType.StoredProcedure;

                            cmdSQL.Parameters.AddWithValue("_data_time_type", NpgsqlDbType.Integer, iDataTimeType);
                            cmdSQL.Parameters.AddWithValue("_airport_id", NpgsqlDbType.Integer, _Enplanement.AirportID);
                            if (iDataTimeType == 0)
                            {
                                cmdSQL.Parameters.AddWithValue("_pax_timestamp", NpgsqlDbType.Timestamp, _PAXVolume.PAXHourTimestamp);
                            }
                            else
                            {
                                cmdSQL.Parameters.AddWithValue("_pax_timestamp", NpgsqlDbType.Timestamp, _Enplanement.PAXDate);
                            }
                            cmdSQL.Parameters.AddWithValue("_enplanements", NpgsqlDbType.Integer, _PAXVolume.Volume);
                            cmdSQL.Parameters.AddWithValue("_tsa_level", NpgsqlDbType.Integer, iTSALevel);
                            cmdSQL.Parameters.AddWithValue("_flight_type_id", NpgsqlDbType.Integer, _Enplanement.FlightType);
                            cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _Enplanement.LastUpdate);

                            cmdSQL.ExecuteNonQuery();

                            bResult = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_ProcessData.UpdateTSAEnplanementData()");
                PAX_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

        public static bool UpdateCBPEnplanementData(PAX _PAX)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertcbppaxvolumedata";


            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(PAX_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    foreach (PAXVolume _PAXVolume in _PAX.Volume)
                    {
                        using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                        {
                            cmdSQL.CommandTimeout = PAX_Consumer_Globals.iDBCommandTimeoutInSeconds;
                            cmdSQL.CommandType = CommandType.StoredProcedure;

                            cmdSQL.Parameters.AddWithValue("_airport_id", NpgsqlDbType.Integer, _PAX.AirportID);
                            cmdSQL.Parameters.AddWithValue("_terminal", NpgsqlDbType.Varchar, _PAXVolume.Terminal);
                            cmdSQL.Parameters.AddWithValue("_pax_hour_timestamp", NpgsqlDbType.Timestamp, _PAX.PAXDate);
                            cmdSQL.Parameters.AddWithValue("_pax_type", NpgsqlDbType.Integer, _PAX.PAXClassification);
                            cmdSQL.Parameters.AddWithValue("_volume", NpgsqlDbType.Integer, _PAXVolume.Volume);
                            cmdSQL.Parameters.AddWithValue("_flight_count", NpgsqlDbType.Integer, _PAXVolume.NumFlights);
                            cmdSQL.Parameters.AddWithValue("_avg_wait_time_in_mins", NpgsqlDbType.Integer, _PAXVolume.AvgWaitTime);
                            cmdSQL.Parameters.AddWithValue("_max_wait_time_in_mins", NpgsqlDbType.Integer, _PAXVolume.MaxWaitTime);
                            cmdSQL.Parameters.AddWithValue("_avg_us_wait_time_in_mins", NpgsqlDbType.Integer, _PAXVolume.AvgUSWaitTime);
                            cmdSQL.Parameters.AddWithValue("_max_us_wait_time_in_mins", NpgsqlDbType.Integer, _PAXVolume.MaxUSWaitTime);
                            cmdSQL.Parameters.AddWithValue("_avg_non_us_wait_time_in_mins", NpgsqlDbType.Integer, _PAXVolume.AvgNonUSWaitTime);
                            cmdSQL.Parameters.AddWithValue("_max_non_us_wait_time_in_mins", NpgsqlDbType.Integer, _PAXVolume.MaxNonUSWaitTime);
                            cmdSQL.Parameters.AddWithValue("_num_booths", NpgsqlDbType.Integer, _PAXVolume.NumBooths);
                            cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.Timestamp, _PAX.LastUpdate);

                            cmdSQL.ExecuteNonQuery();

                            bResult = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                PAX_Consumer_Globals.LogFile.LogErrorText("Error in PAX_Consumer_ProcessData.UpdateCBPEnplanementData()");
                PAX_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                PAX_Consumer_Globals.LogFile.LogError(ex);
            }

            return bResult;
        }

    }
}
