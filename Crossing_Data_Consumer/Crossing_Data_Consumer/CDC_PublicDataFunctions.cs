using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Crossing_Data_Consumer
{
    class Crossing_Data_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            if (GetCrossings())
            {
                return true;
            }

            return false;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo(); ";

            try
            {
                Crossing_Data_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Crossing_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Crossing_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {

                                if (!Crossing_Data_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    Crossing_Data_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                            //if ((Crossing_Data_Consumer_Globals.iAgencyID = Crossing_Data_Consumer_Globals.SetAgencyID(Crossing_Data_Consumer_Globals.sAgencyName)) == 0)
                           // {
                            //    Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error setting agency ID for agency " + Crossing_Data_Consumer_Globals.sAgencyName + ". Cannot handle any functions for this agency.");
                             //   return false;
                           // }
                        }
                    }

                }

                Crossing_Data_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error in Crossing_Data_Consumer_PublicDataFunctions.GetAgencyInfo()");
                Crossing_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetCrossings()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "";
            int i = 0;
            CrossingData _CrossingData = new CrossingData();

            try
            {
                Crossing_Data_Consumer_Globals.dictCrossingLookup.Clear();

                sSQL = "SELECT id, source_facility_id, source_facility_name, local_facility_id, local_facility_name, agency_id from xref_tables.tblfacilityxref;";

                using (NpgsqlConnection sConn = new NpgsqlConnection(Crossing_Data_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Crossing_Data_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                _CrossingData = new CrossingData();
                                _CrossingData.SourceFacilityId = Convert.ToInt32(rsRead[1].ToString());
                                _CrossingData.SourceFacilityName = rsRead[2].ToString();
                                _CrossingData.LocalFacilityId = Convert.ToInt32(rsRead[3].ToString());
                                _CrossingData.LocalFacilityName = rsRead[4].ToString();
                                _CrossingData.AgencyId = Convert.ToInt32(rsRead[5].ToString());

                                if (!Crossing_Data_Consumer_Globals.dictCrossingLookup.ContainsKey(_CrossingData.LocalFacilityId))
                                {
                                    Crossing_Data_Consumer_Globals.dictCrossingLookup.Add(_CrossingData.LocalFacilityId, _CrossingData);
                                }
                            }
                        }
                    }

                }

                Crossing_Data_Consumer_Globals.LogFile.Log("Successfully read crossing data in GetCrossings().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText("Error in PublicDataFunctions.GetCrossings()");
                Crossing_Data_Consumer_Globals.LogFile.LogErrorText(ex.ToString());
                Crossing_Data_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
