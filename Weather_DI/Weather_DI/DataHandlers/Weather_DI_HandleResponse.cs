using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;

namespace Weather_DI
{
    class Weather_DI_HandleResponse
    {
        

        public static void HandleWeatherResponse(List<dynamic> lWeatherData) //1.
        {
            Weather_DI_DataStructures.Periods _WeatherData = new Weather_DI_DataStructures.Periods();
            List<Weather_DI_DataStructures.Periods> lstPeriods = new List<Weather_DI_DataStructures.Periods>();
            DateTime dtStartTime = new DateTime();
            DateTime dtMaxStartTime = DateTime.Now.AddHours(Weather_DI_Globals.dHourlyForecastHoursAhead);
            String sDataType = "";
            int dWindDirection = 0;

            try
            {                
                foreach (dynamic lstItem in lWeatherData) //1.
                {
                    //Using the dynamic string list from Kafka weather_data topic, we will convert each value to JObject and parse the properties into the _WeatherData class
                    try
                    {

                        JObject jObj = JObject.Parse(lstItem.ToString());
                        dtStartTime = Convert.ToDateTime(jObj["StartTime"].ToString());
                        sDataType = jObj["DataType"].ToString();

                        

                        if ((dtStartTime > dtMaxStartTime) && (sDataType == "hourly_forecast"))
                        {
                            //do nothing
                        }
                        else
                        {
                            _WeatherData = new Weather_DI_DataStructures.Periods();
                            _WeatherData.LocationID = Convert.ToInt32(jObj["LocationID"].ToString());
                            _WeatherData.DataType = sDataType;
                            _WeatherData.LocationName = jObj["LocationName"].ToString();
                            _WeatherData.PeriodNumber = Convert.ToInt32(jObj["PeriodNumber"].ToString());
                            _WeatherData.PeriodName = jObj["PeriodName"].ToString();
                            _WeatherData.StartTime = jObj["StartTime"].ToString();
                            _WeatherData.EndTime = jObj["EndTime"].ToString();
                            _WeatherData.IsDayTime = Convert.ToBoolean(jObj["IsDayTime"]);
                            _WeatherData.Temperature = Convert.ToInt32(jObj["Temperature"].ToString());
                            _WeatherData.TemperatureUnit = jObj["TemperatureUnit"].ToString();
                            _WeatherData.TemperatureTrend = jObj["TemperatureTrend"].ToString();
                            _WeatherData.WindSpeed = jObj["WindSpeed"].ToString();
                            _WeatherData.WindDirection = jObj["WindDirection"].ToString();
                            _WeatherData.Icon = jObj["Icon"].ToString();
                            _WeatherData.ShortForecast = jObj["ShortForecast"].ToString();
                            _WeatherData.DetailedForecast = jObj["DetailedForecast"].ToString();

                            if (sDataType == "current")
                            {
                                //Add " mph" to keep wind_speed column consistent
                                _WeatherData.WindSpeed = _WeatherData.WindSpeed + " mph";

                                //If current weather is in C, convert to F
                                if (_WeatherData.TemperatureUnit.Contains("C"))
                                {
                                    _WeatherData.Temperature = Convert.ToInt32((Convert.ToDouble(_WeatherData.Temperature) * 1.8) + 32);
                                }
                                
                                //If wind direction is in degrees, convert to cardinal direction
                                try
                                {
                                    dWindDirection = Convert.ToInt32(_WeatherData.WindDirection);
                                    _WeatherData.WindDirection = GetCardinalDirection(dWindDirection);
                                }
                                catch (Exception ex3)
                                {
                                    Weather_DI_Globals.LogFile.LogErrorText("Error converting current wind direction from degrees to cardinal direction. Leaving as original value.");
                                    Weather_DI_Globals.LogFile.LogError(ex3);
                                }
                            }

                            lstPeriods.Add(_WeatherData);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Weather_DI_Globals.LogFile.LogErrorText("Error in parsing converted periods object.");
                        Weather_DI_Globals.LogFile.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                Weather_DI_Globals.LogFile.LogErrorText("Error in Weather_DI_HandleResponse()");
                Weather_DI_Globals.LogFile.LogError(ex);
            }
            finally
            {
                Weather_DI_Globals.LogFile.Log("Finished processing weather data from Kafka.");
                Weather_DI_Globals.LogFile.Log("Resetting Weather data timer.");

                Weather_DI_Globals.tmGetWeatherForecastData.Start();
            }
            //Call methods to update databases
            Weather_DI_ProcessWeather.UpdateWeather(lstPeriods);
            if (Weather_DI_Globals.bSendToMongoDB)
            {
                Weather_DI_MongoCode.DoBatchUpdate(lstPeriods);
            }            
        }

        public static string GetCardinalDirection(int iDegrees)
        {
            String sWindDirection = "";

            if (iDegrees >= 337 || iDegrees <= 23)
            {
                sWindDirection = "N";
            }
            else if (iDegrees > 23 && iDegrees < 67)
            {
                sWindDirection = "NE";
            }
            else if (iDegrees >= 67 && iDegrees <= 113)
            {
                sWindDirection = "E";
            }
            else if (iDegrees > 113 && iDegrees < 157)
            {
                sWindDirection = "SE";
            }
            else if (iDegrees >= 157 && iDegrees <= 203)
            {
                sWindDirection = "S";
            }
            else if (iDegrees > 203 && iDegrees < 247)
            {
                sWindDirection = "SW";
            }
            else if (iDegrees >= 247 && iDegrees <= 293)
            {
                sWindDirection = "W";
            }            
            else if (iDegrees > 293 && iDegrees < 337)
            {
                sWindDirection = "NW";
            }
            else
            {
                sWindDirection = iDegrees.ToString();
            }

            return sWindDirection;
        }
    }
}
