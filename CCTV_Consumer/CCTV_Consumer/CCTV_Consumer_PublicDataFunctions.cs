using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;

namespace CCTV_Consumer
{
    class CCTV_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            if (GetAgencyInfo())
            {
                return true;
            }

            return false;
        }

        public static bool GetCurrentDynamicData()
        {
            if (GetCurrentCCTVData())
            {
                return true;
            }
            return false;
        }

        public static bool GetCurrentCCTVData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "select _id, _native_CCTV_id, _agency_id, _cctv_name, " +
                "_roadway, _direction, _snapshot_url, _video_url, _blocked, _status, " +
                "_latitude, _longitude, _last_update FROM db_objects.sp_getCCTV();";// + 
                //"where _agency_id = " + CCTV_Consumer_Globals.iAgencyID + ";";
            CCTVData _CCTV = new CCTVData();
            StoredCCTVData _StoredCCTV = new StoredCCTVData();
            Point _Coords = new Point();
            DateTime dtTmpDate = new DateTime();

            try
            {
                CCTV_Consumer_Globals.dictStoredCCTV.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(CCTV_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = CCTV_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                if (!CCTV_Consumer_Globals.dictStoredCCTV.ContainsKey(rsRead[2].ToString() + "|" + rsRead[1].ToString()))
                                {
                                    _CCTV = new CCTVData();
                                    _StoredCCTV = new StoredCCTVData();

                                    //_CCTV.LocalCCTVID = Convert.ToInt32(rsRead[0]);
                                    _CCTV.SourceCCTVID = rsRead[1].ToString();
                                    _CCTV.AgencyID = Convert.ToInt32(rsRead[2]);
                                    _CCTV.CCTVName = rsRead[3].ToString();
                                    _CCTV.Roadway = string.IsNullOrEmpty(rsRead[4].ToString()) ? DBNull.Value.ToString() : rsRead[4].ToString();
                                    _CCTV.Direction = string.IsNullOrEmpty(rsRead[5].ToString()) ? DBNull.Value.ToString() : rsRead[5].ToString(); ;
                                    _CCTV.SnapshotURL = string.IsNullOrEmpty(rsRead[6].ToString()) ? DBNull.Value.ToString() : rsRead[6].ToString();
                                    _CCTV.VideoURL = string.IsNullOrEmpty(rsRead[7].ToString()) ? DBNull.Value.ToString() : rsRead[7].ToString();
                                    _CCTV.Blocked = string.IsNullOrEmpty(rsRead[8].ToString()) ? false : Convert.ToBoolean(rsRead[8]);
                                    _CCTV.Status = string.IsNullOrEmpty(rsRead[9].ToString()) ? 0 : Convert.ToInt16(rsRead[9]);

                                    _Coords.Latitude = Convert.ToDouble(rsRead[10]);
                                    _Coords.Longitude = Convert.ToDouble(rsRead[11]);
                                    _CCTV.Coords = _Coords;

                                    _CCTV.LastUpdate = string.IsNullOrEmpty(rsRead[12].ToString()) ? dtTmpDate : Convert.ToDateTime(rsRead[12].ToString());

                                    _StoredCCTV.SourceCCTVID = rsRead[2].ToString() + "|" + rsRead[1].ToString();
                                    _StoredCCTV.StoredJSON.Append(JsonConvert.SerializeObject(_CCTV, Newtonsoft.Json.Formatting.None).Trim());

                                    CCTV_Consumer_Globals.dictStoredCCTV.Add(_StoredCCTV.SourceCCTVID, _StoredCCTV);
                                    _CCTV = null;
                                    _StoredCCTV = null;
                                }
                            }
                        }
                    }

                }

                CCTV_Consumer_Globals.LogFile.Log("Successfully read CCTV data in GetCurrentCCTVData().");
                bSuccess = true;

            }
            catch (Exception ex)
            {
                CCTV_Consumer_Globals.LogFile.LogErrorText("Error in CCTV_Consumer_PublicDataFunctions.GetCurrentCCTVData()");
                CCTV_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                CCTV_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(CCTV_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = CCTV_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!CCTV_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    CCTV_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }
                        }
                    }

                }

                CCTV_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                CCTV_Consumer_Globals.LogFile.LogErrorText("Error in GetAgencyInfo()");
                CCTV_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
