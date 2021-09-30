using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace CCTV_Consumer
{
    class CCTV_Consumer_ProcessCCTV
    {
        public bool UpdateCCTV(CCTVData _CCTV, StoredCCTVData _StoredCCTV)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_updatecctv";
            int iResult = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(CCTV_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = CCTV_Consumer_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Integer, _CCTV.AgencyID);
                        cmdSQL.Parameters.AddWithValue("_native_cctv_id", NpgsqlDbType.Varchar, _CCTV.SourceCCTVID);
                        cmdSQL.Parameters.AddWithValue("_cctv_name", NpgsqlDbType.Varchar, _CCTV.CCTVName);
                        cmdSQL.Parameters.AddWithValue("_roadway", NpgsqlDbType.Varchar, _CCTV.Roadway);
                        cmdSQL.Parameters.AddWithValue("_direction", NpgsqlDbType.Varchar, _CCTV.Direction);
                        cmdSQL.Parameters.AddWithValue("_snapshot_url", NpgsqlDbType.Varchar, _CCTV.SnapshotURL);
                        cmdSQL.Parameters.AddWithValue("_video_url", NpgsqlDbType.Varchar, _CCTV.VideoURL);
                        cmdSQL.Parameters.AddWithValue("_blocked", NpgsqlDbType.Boolean, _CCTV.Blocked);
                        cmdSQL.Parameters.AddWithValue("_status", NpgsqlDbType.Integer, _CCTV.Status);
                        cmdSQL.Parameters.AddWithValue("_latitude", NpgsqlDbType.Double, _CCTV.Coords.Latitude);
                        cmdSQL.Parameters.AddWithValue("_longitude", NpgsqlDbType.Double, _CCTV.Coords.Longitude);
                        cmdSQL.Parameters.AddWithValue("_last_update", NpgsqlDbType.TimestampTZ, _CCTV.LastUpdate);

                        iResult = (Int32)cmdSQL.ExecuteScalar();

                        //Add to CCTV ID map - first clear original if exists
                        if (CCTV_Consumer_Globals.dictStoredCCTV.ContainsKey(_CCTV.AgencyID + "|" + _CCTV.SourceCCTVID))
                        {
                            CCTV_Consumer_Globals.dictStoredCCTV.Remove(_CCTV.AgencyID + "|" + _CCTV.SourceCCTVID);
                        }

                        CCTV_Consumer_Globals.dictStoredCCTV.Add(_CCTV.AgencyID + "|" + _CCTV.SourceCCTVID, _StoredCCTV);
                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                CCTV_Consumer_Globals.LogFile.LogErrorText("Error in UpdateCCTV().");
                CCTV_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                CCTV_Consumer_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
