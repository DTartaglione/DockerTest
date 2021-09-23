using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather_DI
{
    class Weather_DI_ProcessWeather
    {

        public static void UpdateWeather(List<Weather_DI_DataStructures.Periods> lstPeriods)
        {            
            string sSQLHourly = "db_objects.sp_mod_di_updateweatherhourly";
            string sSQLWeather = "db_objects.sp_mod_di_updateweather";
            //bool bResult = false;
            string sDataType = "";

            //Postgres: latest pull
            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Weather_DI_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();
                    Weather_DI_Globals.LogFile.Log("Begin updating Postgres dynamic weather tables.");
                        
                        foreach(var lstItem in lstPeriods)
                        {
                            try
                            {
                                sDataType = lstItem.DataType;

                                if (sDataType == "extended_forecast" || sDataType == "current")
                                {
                                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQLWeather, sConn))
                                    {
                                        cmdSQL.CommandTimeout = Weather_DI_Globals.iDBCommandTimeoutInSeconds;
                                        cmdSQL.CommandType = CommandType.StoredProcedure;

                                        cmdSQL.Parameters.AddWithValue("_source_id", NpgsqlDbType.Varchar, lstItem.PeriodNumber.ToString());
                                        cmdSQL.Parameters.AddWithValue("_location_id", NpgsqlDbType.Integer, lstItem.LocationID);
                                        cmdSQL.Parameters.AddWithValue("_period_name", NpgsqlDbType.Varchar, lstItem.PeriodName);

                                        //Pass null value if there is no start or end time (empty string is set)
                                        if (lstItem.StartTime != "")
                                        {
                                            cmdSQL.Parameters.AddWithValue("_start_time", NpgsqlDbType.Timestamp, Convert.ToDateTime(lstItem.StartTime));
                                        }
                                        else
                                        {
                                            cmdSQL.Parameters.AddWithValue("_start_time", DBNull.Value);
                                        }
                                        if (lstItem.EndTime != "")
                                        {
                                            cmdSQL.Parameters.AddWithValue("_end_time", NpgsqlDbType.Timestamp, Convert.ToDateTime(lstItem.EndTime));
                                        }
                                        else
                                        {
                                            cmdSQL.Parameters.AddWithValue("_end_time", DBNull.Value);
                                        }

                                        cmdSQL.Parameters.AddWithValue("_temperature", NpgsqlDbType.Integer, lstItem.Temperature);
                                        cmdSQL.Parameters.AddWithValue("_wind_speed", NpgsqlDbType.Varchar, lstItem.WindSpeed);
                                        cmdSQL.Parameters.AddWithValue("_wind_direction", NpgsqlDbType.Varchar, lstItem.WindDirection);
                                        cmdSQL.Parameters.AddWithValue("_icon", NpgsqlDbType.Varchar, lstItem.Icon);
                                        cmdSQL.Parameters.AddWithValue("_short_forecast", NpgsqlDbType.Varchar, lstItem.ShortForecast);
                                        cmdSQL.Parameters.AddWithValue("_detailed_forecast", NpgsqlDbType.Varchar, lstItem.DetailedForecast);

                                        cmdSQL.ExecuteNonQuery();
                                    }
                                }

                            else if (sDataType == "hourly_forecast")
                            {
                                using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQLHourly, sConn))
                                {
                                    cmdSQL.CommandTimeout = Weather_DI_Globals.iDBCommandTimeoutInSeconds;
                                    cmdSQL.CommandType = CommandType.StoredProcedure;

                                    cmdSQL.Parameters.AddWithValue("_source_id", NpgsqlDbType.Varchar, lstItem.PeriodNumber.ToString());
                                    cmdSQL.Parameters.AddWithValue("_location_id", NpgsqlDbType.Integer, lstItem.LocationID);
                                    cmdSQL.Parameters.AddWithValue("_period_name", NpgsqlDbType.Varchar, lstItem.PeriodName);

                                    //Pass null value if there is no start or end time (empty string is set)
                                    if (lstItem.StartTime != "")
                                    {
                                        cmdSQL.Parameters.AddWithValue("_start_time", NpgsqlDbType.Timestamp, Convert.ToDateTime(lstItem.StartTime));
                                    }
                                    else
                                    {
                                        cmdSQL.Parameters.AddWithValue("_start_time", DBNull.Value);
                                    }
                                    if (lstItem.EndTime != "")
                                    {
                                        cmdSQL.Parameters.AddWithValue("_end_time", NpgsqlDbType.Timestamp, Convert.ToDateTime(lstItem.EndTime));
                                    }
                                    else
                                    {
                                        cmdSQL.Parameters.AddWithValue("_end_time", DBNull.Value);
                                    }

                                    cmdSQL.Parameters.AddWithValue("_temperature", NpgsqlDbType.Integer, lstItem.Temperature);
                                    cmdSQL.Parameters.AddWithValue("_wind_speed", NpgsqlDbType.Varchar, lstItem.WindSpeed);
                                    cmdSQL.Parameters.AddWithValue("_wind_direction", NpgsqlDbType.Varchar, lstItem.WindDirection);
                                    cmdSQL.Parameters.AddWithValue("_icon", NpgsqlDbType.Varchar, lstItem.Icon);
                                    cmdSQL.Parameters.AddWithValue("_short_forecast", NpgsqlDbType.Varchar, lstItem.ShortForecast);
                                    cmdSQL.Parameters.AddWithValue("_detailed_forecast", NpgsqlDbType.Varchar, lstItem.DetailedForecast);

                                    cmdSQL.ExecuteNonQuery();
                                }
                            }

                            else
                            {
                                Weather_DI_Globals.LogFile.Log(lstItem.PeriodName + "(StartTime: " + lstItem.StartTime + ") did not have a recognizable DataType. Please verify.");
                            }
                        }
                            catch (Exception ex2)
                            {
                                Weather_DI_Globals.LogFile.LogErrorText("Error when updating dynamic weather in Postgres for " + lstItem.PeriodName + 
                                    "(StartTime: " + lstItem.StartTime + ")");
                                Weather_DI_Globals.LogFile.LogErrorText(ex2.ToString());
                                Weather_DI_Globals.LogFile.LogError(ex2);
                            }

                        }
                        Weather_DI_Globals.LogFile.Log("Finished updating Postgres dynamic weather tables.");
                }

            }
            catch (Exception ex)
            {
                Weather_DI_Globals.LogFile.LogErrorText("Error in Weather_DI_ProcessWeather.UpdateWeather. Please verify Postgres connection.");
                Weather_DI_Globals.LogFile.LogErrorText(ex.ToString());
                Weather_DI_Globals.LogFile.LogError(ex);
            }
        }
    }
}
