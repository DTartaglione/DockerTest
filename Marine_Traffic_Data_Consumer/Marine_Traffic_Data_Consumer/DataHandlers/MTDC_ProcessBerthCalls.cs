using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Marine_Traffic_Data_Consumer
{
    class MTDC_ProcessBerthCalls
    {
        public static bool UpdateBerthCallRecord(BerthItem _BerthItem)
        {
            bool bResult = false;
            string sSQL = "db_objects.sp_insertberthcalldata";
            int iResult = 0;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(MTDC_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = MTDC_Globals.iDBCommandTimeoutInSeconds;
                        cmdSQL.CommandType = CommandType.StoredProcedure;

                        cmdSQL.Parameters.AddWithValue("_port_id", NpgsqlDbType.Integer, _BerthItem.PortInfo.ID);
                        cmdSQL.Parameters.AddWithValue("_agency_id", NpgsqlDbType.Integer, _BerthItem.PortInfo.AgencyID);
                        cmdSQL.Parameters.AddWithValue("_unlocode", NpgsqlDbType.Varchar, _BerthItem.PortInfo.UNLOCODE);
                        cmdSQL.Parameters.AddWithValue("_port_name", NpgsqlDbType.Varchar, _BerthItem.PortInfo.Name);
                        cmdSQL.Parameters.AddWithValue("_port_state", NpgsqlDbType.Varchar, _BerthItem.PortInfo.State);
                        cmdSQL.Parameters.AddWithValue("_terminal_id", NpgsqlDbType.Integer, _BerthItem.TerminalInfo.ID);
                        cmdSQL.Parameters.AddWithValue("_terminal_name", NpgsqlDbType.Varchar, _BerthItem.TerminalInfo.Name);
                        cmdSQL.Parameters.AddWithValue("_berth_id", NpgsqlDbType.Integer, _BerthItem.BerthInfo.ID);
                        cmdSQL.Parameters.AddWithValue("_berth_name", NpgsqlDbType.Varchar, _BerthItem.BerthInfo.Name);
                        cmdSQL.Parameters.AddWithValue("_berth_lat", NpgsqlDbType.Double, _BerthItem.BerthInfo.Latitude);
                        cmdSQL.Parameters.AddWithValue("_berth_lon", NpgsqlDbType.Double, _BerthItem.BerthInfo.Longitude);
                        cmdSQL.Parameters.AddWithValue("_ship_id", NpgsqlDbType.Integer, _BerthItem.ShipInfo.ID);
                        cmdSQL.Parameters.AddWithValue("_source_ship_id", NpgsqlDbType.Integer, _BerthItem.ShipInfo.SourceShipID);
                        cmdSQL.Parameters.AddWithValue("_mmsi_id", NpgsqlDbType.Integer, _BerthItem.ShipInfo.MMSI_ID);
                        cmdSQL.Parameters.AddWithValue("_imo_id", NpgsqlDbType.Integer, _BerthItem.ShipInfo.IMO_ID);
                        cmdSQL.Parameters.AddWithValue("_ship_name", NpgsqlDbType.Varchar, _BerthItem.ShipInfo.Name);
                        cmdSQL.Parameters.AddWithValue("_ship_type_name", NpgsqlDbType.Varchar, _BerthItem.ShipInfo.TypeName);
                        cmdSQL.Parameters.AddWithValue("_ship_deadweight", NpgsqlDbType.Integer, _BerthItem.ShipInfo.DWT);
                        cmdSQL.Parameters.AddWithValue("_ship_grosstonnage", NpgsqlDbType.Integer, _BerthItem.ShipInfo.GRT);
                        cmdSQL.Parameters.AddWithValue("_ship_year_built", NpgsqlDbType.Integer, _BerthItem.ShipInfo.YearBuilt);
                        cmdSQL.Parameters.AddWithValue("_arrival_timestamp", NpgsqlDbType.TimestampTZ, (object)_BerthItem.ShipInfo.ArrivalTimestamp ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_departure_timestamp", NpgsqlDbType.TimestampTZ, (object)_BerthItem.ShipInfo.DepartureTimestamp ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_dock_timestamp", NpgsqlDbType.TimestampTZ, (object)_BerthItem.ShipInfo.DockTimestamp ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_undock_timestamp", NpgsqlDbType.TimestampTZ, (object)_BerthItem.ShipInfo.UndockTimestamp ?? DBNull.Value);
                        cmdSQL.Parameters.AddWithValue("_time_at_berth", NpgsqlDbType.Integer, _BerthItem.ShipInfo.TimeAtBerth);
                        cmdSQL.Parameters.AddWithValue("_time_at_port", NpgsqlDbType.Integer, _BerthItem.ShipInfo.TimeAtPort);
                        cmdSQL.Parameters.AddWithValue("_arrival_load_status", NpgsqlDbType.Integer, _BerthItem.ShipInfo.ArrivalLoadStatus);
                        cmdSQL.Parameters.AddWithValue("_departure_load_status", NpgsqlDbType.Integer, _BerthItem.ShipInfo.DepartureLoadStatus);

                        cmdSQL.ExecuteNonQuery();

                       
                        bResult = true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MTDC_Globals.LogFile.LogErrorText("Error in UpdateBerthCallRecord().");
                MTDC_Globals.LogFile.LogErrorText(ex.ToString());
                MTDC_Globals.LogFile.LogError(ex);
            }
            return bResult;
        }
    }
}
