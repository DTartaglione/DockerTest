using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Link_Consumer
{
    class Link_Consumer_PublicDataFunctions
    {
        public static bool GetStaticData()
        {
            
            if (GetCurrentLinkData())
            {
                if (GetLinkListForArchive())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetCurrentLinkData()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT l.id, l.native_id, d.last_update from staticds.tbllink l " +
                "left join dynamicds.tbllinktrafficdata d on l.id = d.id;";
            DateTime dtEarliestDate = new DateTime();

            Link_Consumer_DataStructures.Link_Consumer_StoredLinkData _LinkData = new Link_Consumer_DataStructures.Link_Consumer_StoredLinkData();


            try
            {
                //Link_Consumer_Globals.dictHERELinkIDMap.Clear();
                Link_Consumer_Globals.dictLinkIDAndListData.Clear();
                using (NpgsqlConnection sConn = new NpgsqlConnection(Link_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Link_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows){
                            while (rsRead.Read())
                            {

                                if (!Link_Consumer_Globals.dictLinkIDAndListData.ContainsKey(rsRead[0].ToString())){
                                    _LinkData = new Link_Consumer_DataStructures.Link_Consumer_StoredLinkData();
                                    _LinkData.sLinkID = rsRead[0].ToString();

                                    //if datetime is empty in DB (no new data), hardcode with 01/01/0001 so will always process
                                    _LinkData.sLastUpdate = String.IsNullOrEmpty(rsRead[2].ToString()) ? dtEarliestDate.ToString() : rsRead[2].ToString();
                                    _LinkData.sLinkData = "";

                                    Link_Consumer_Globals.dictLinkIDAndListData.Add(rsRead[0].ToString(), _LinkData);
                                }
                            }
                        }
                    }

                }

                Link_Consumer_Globals.LogFile.Log("Successfully read link data in GetCurrentLinkData().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_PublicDataFunctions.GetCurrentLinkData()");
                Link_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static bool GetAgencyInfo()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sSQL = "SELECT _id, _agency_name from db_objects.sp_MOD_DI_getagencyinfo() WHERE _id in (" + Link_Consumer_Globals.sSubscribeToAgencyIDs + "); ";

            try
            {
                Link_Consumer_Globals.dictAgencyInfo.Clear();

                using (NpgsqlConnection sConn = new NpgsqlConnection(Link_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Link_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read()) {      
                                
                                if (!Link_Consumer_Globals.dictAgencyInfo.ContainsKey(Convert.ToInt32(rsRead[0].ToString())))
                                {
                                    Link_Consumer_Globals.dictAgencyInfo.Add(Convert.ToInt32(rsRead[0].ToString()), rsRead[1].ToString());
                                }
                            }

                        }
                    }
                }

                Link_Consumer_Globals.LogFile.Log("Successfully read agency data in GetAgencyInfo().");

                bSuccess = true;

            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_PublicDataFunctions.GetAgencyInfo()");
                Link_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

        public static string GetLinkBoundingBox()
        {
            string sLinkBoundingBox = "";
            string sBoundingBoxSQL = "SELECT param_value from staticds.tblparams WHERE param_name = 'link_bounding_box';";
            NpgsqlDataReader rsRead = null;

            try
            {
                using (NpgsqlConnection sConn = new NpgsqlConnection(Link_Consumer_Globals.sPG_MOD_DBConn))
                {
                    sConn.Open();

                    using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sBoundingBoxSQL, sConn))
                    {
                        cmdSQL.CommandTimeout = Link_Consumer_Globals.iDBCommandTimeoutInSeconds;

                        rsRead = cmdSQL.ExecuteReader();

                        if (rsRead.HasRows)
                        {
                            while (rsRead.Read())
                            {
                                sLinkBoundingBox = rsRead[0].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_PublicDataFunctions.GetLinkBoundingBox()");
                Link_Consumer_Globals.LogFile.LogError(ex);
                sLinkBoundingBox = "";
            }

            return sLinkBoundingBox;
        }

        public static bool GetLinkListForArchive()
        {
            bool bSuccess = false;
            NpgsqlDataReader rsRead = null;
            string sGetLinkList = "";
            string sLinkBoundingBox = "";

            try
            {
                Link_Consumer_Globals.hshLinksToArchive.Clear();
                

                if ((sLinkBoundingBox = GetLinkBoundingBox()) != "")
                {
                    sGetLinkList = "SELECT id from staticds.tbllink where ST_Intersects(ST_GeomFromText('Polygon ((" + sLinkBoundingBox + "))',4326),geom) = TRUE " +
                        "AND owner_org_id in (" + Link_Consumer_Globals.sSubscribeToAgencyIDs + ");";

                    using (NpgsqlConnection sConn = new NpgsqlConnection(Link_Consumer_Globals.sPG_MOD_DBConn))
                    {

                        sConn.Open();
                        using (NpgsqlCommand cmdSQL = new NpgsqlCommand(sGetLinkList, sConn))
                        {
                            cmdSQL.CommandTimeout = Link_Consumer_Globals.iDBCommandTimeoutInSeconds;
                            rsRead = cmdSQL.ExecuteReader();

                            if (rsRead.HasRows)
                            {
                                while (rsRead.Read())
                                {
                                    if (!Link_Consumer_Globals.hshLinksToArchive.Contains(rsRead[0].ToString()))
                                    {
                                        Link_Consumer_Globals.hshLinksToArchive.Add(rsRead[0].ToString());
                                    }
                                }
                                Link_Consumer_Globals.LogFile.Log("Successfully read link data in GetLinkBoundingBoxAndLinkList().");
                                bSuccess = true;
                            }
                            else
                            {
                                bSuccess = false;
                                Link_Consumer_Globals.LogFile.Log("Error reading in link list for archiving. No records returned within link bounding box set in tblparams.");
                            }
                        }
                    }
                }
                else
                {
                    bSuccess = false;
                    Link_Consumer_Globals.LogFile.Log("Error reading in link list for archiving. link_bounding_box parameter in staticds.tblparams is empty.");
                }
            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_PublicDataFunctions.GetLinkBoundingBoxAndLinkList()");
                Link_Consumer_Globals.LogFile.LogError(ex);
            }

            return bSuccess;
        }

    }
}
