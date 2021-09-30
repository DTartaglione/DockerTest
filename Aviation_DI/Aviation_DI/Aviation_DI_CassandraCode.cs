using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;

namespace Aviation_DI
{
    class Aviation_DI_CassandraCode
    {
        public static bool DoBatchUpdate(string sPreparedStatement, List<List<object>> lstData)
        {
            bool bSuccess = false;
            int i = 0;
            int iProcessedCount = 0;

            try
            {
                Aviation_DI_Globals.LogFile.Log("Connecting to Cassandra host for batch insert.");

                var cluster = Cluster.Builder()
                  .AddContactPoint(Aviation_DI_Globals.sCassandra_DBConn)
                  .Build();

                //Create connections to the nodes using a keyspace
                ISession session = cluster.Connect();
                PreparedStatement noSQLStatement = session.Prepare(sPreparedStatement);
                BoundStatement bndSQLStatement = new BoundStatement(noSQLStatement);

                //...you should reuse the prepared statement
                //Bind the parameters and add the statement to the batch batch
                BatchStatement batchStatement = new BatchStatement();

                Aviation_DI_Globals.LogFile.Log("Beginning batch storage.");

                foreach (List<object> lstItem in lstData)
                {
                    batchStatement.Add(noSQLStatement.Bind(lstItem.ToArray()));

                    i++;
                    iProcessedCount++;

                    if (i == Aviation_DI_Globals.iCassandraBatchInsertSize || (iProcessedCount == lstData.Count))
                    {
                        session.Execute(batchStatement);
                        i = 0;
                        batchStatement = new BatchStatement();
                    }
                }

                Aviation_DI_Globals.LogFile.Log("Finished inserting into Cassandra DB.");
                bSuccess = true;

            }
            catch (Exception ex)
            {
                Aviation_DI_Globals.LogFile.LogErrorText("Error in Aviation_DI_CassandraCode.DoBatchUpdate().");
                Aviation_DI_Globals.LogFile.Log(ex.ToString());
                Aviation_DI_Globals.LogFile.LogError(ex);
            
                bSuccess = false;
            }

            return bSuccess;
        }
    }
}
