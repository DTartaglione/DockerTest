using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace Link_Consumer
{
    class Link_Consumer_CassandraCode
    {
        public static bool DoBatchUpdate(string sPreparedStatement, List<Link_Consumer_DataStructures.Link_Consumer_LinkData> lstData)
       // public static bool DoBatchUpdate(string sPreparedStatement, Dictionary<string, Link_Consumer_DataStructures.Link_Consumer_LinkData> dictData)
        {
            bool bSuccess = false;
            int i = 0; 
            int iProcessedCount = 0;
            Object[] linkData = null;

            try
            {
                Link_Consumer_Globals.LogFile.Log("Connecting to Cassandra host for batch insert.");

                var cluster = Cluster.Builder()
                  .AddContactPoint(Link_Consumer_Globals.sCassandra_DBConn)
                  .Build();

                //Create connections to the nodes using a keyspace
                ISession session = cluster.Connect();
                PreparedStatement noSQLStatement = session.Prepare(sPreparedStatement);
                BoundStatement bndSQLStatement = new BoundStatement(noSQLStatement);

                //...you should reuse the prepared statement
                //Bind the parameters and add the statement to the batch batch
                BatchStatement batchStatement = new BatchStatement();

                Link_Consumer_Globals.LogFile.Log("Beginning batch storage.");

                foreach (Link_Consumer_DataStructures.Link_Consumer_LinkData lstItem in lstData)
               // foreach (KeyValuePair<string, Link_Consumer_DataStructures.Link_Consumer_LinkData> dictItem in dictData)
                {
                    linkData = new object[]
                    {
                        lstItem.LinkID,
                        Convert.ToInt32(lstItem.AgencyID),
                        lstItem.Speed,
                        lstItem.TravelTime,
                        lstItem.Volume,
                        lstItem.Occupancy,
                        lstItem.DataType,
                        String.IsNullOrEmpty(lstItem.LastUpdate.ToString()) ? (DateTime?)DateTime.Now : DateTime.Parse(lstItem.LastUpdate.ToString())
                    };

                    //   batchStatement.Add(noSQLStatement.Bind(linkData));
                    batchStatement.Add(noSQLStatement.Bind(linkData));

                    i++;
                    iProcessedCount++;

                    if (i == Link_Consumer_Globals.iCassandraBatchInsertSize || (iProcessedCount == lstData.Count))
                    {
                        session.Execute(batchStatement);
                        i = 0;
                        batchStatement = new BatchStatement();
                    }
                }

                Link_Consumer_Globals.LogFile.Log("Finished inserting into Cassandra DB.");
                bSuccess = true;

            }
            catch (Exception ex)
            {
                Link_Consumer_Globals.LogFile.LogErrorText("Error in Link_Consumer_CassandraCode.DoBatchUpdate().");
                Link_Consumer_Globals.LogFile.Log(ex.ToString());
                Link_Consumer_Globals.LogFile.LogError(ex);
            
                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
