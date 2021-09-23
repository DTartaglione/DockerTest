using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Configuration;

namespace EventHubs_SampleProducer
{
    class EventHubs_SampleProducer_KafkaProducer
    {

        public static void DoBatchUpdateAsync(List<StringBuilder> lstData)
        {
            string sBrokerList = EventHubs_SampleProducer_Globals.sKafkaBrokerList;
            string sTopicName = EventHubs_SampleProducer_Globals.sKafkaTopic;

            DateTime dtBeginTime;
            DateTime dtEndTime;

            var config = new ProducerConfig
            {
                BootstrapServers = sBrokerList,
                QueueBufferingMaxKbytes = 1000000,
                BatchNumMessages = 15000,//,
                SecurityProtocol = (SecurityProtocol)EventHubs_SampleProducer_Globals.iSecurityProtocol, //kafka needs 0, event hubs 3
                SaslMechanism = (SaslMechanism)EventHubs_SampleProducer_Globals.iSaslMechanism, //Kafka needs 1, event hubs 1
                SaslUsername = EventHubs_SampleProducer_Globals.sSaslUsername,
                SaslPassword = EventHubs_SampleProducer_Globals.sSaslPassword       
                // Debug = "security,broker,protocol"
            };
            string sKey = "";
            string sVal = "";
            int iSuccessCount = 0;
            int iFailCount = 0;
            int i = 0;

            EventHubs_SampleProducer_Globals.LogFile.Log("Begin sending " + lstData.Count.ToString() + " data records to Kafka at " + sBrokerList + ", topic: " + sTopicName + ".");

            dtBeginTime = DateTime.Now;

            List<Task> taskList = new List<Task>();

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {

                foreach (StringBuilder sbItem in lstData)
                {
                    try
                    {
                        sKey = EventHubs_SampleProducer_Globals.sAppName + "|" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                        sVal = sbItem.ToString();

                        taskList.Add(producer.ProduceAsync(
                            sTopicName, new Message<string, string> { Key = sKey, Value = sVal }));

                        iSuccessCount++;
                        i++;

                        if (i == config.BatchNumMessages || iSuccessCount == lstData.Count)
                        {
                            i = 0;
                            WaitForKafka(taskList);
                        }
                    }
                    catch (ProduceException<string, string> e)
                    {
                        EventHubs_SampleProducer_Globals.LogFile.LogErrorText($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                        iFailCount++;
                    }

                }


            }

            dtEndTime = DateTime.Now;

            EventHubs_SampleProducer_Globals.LogFile.Log("Kafka completed in " + (dtEndTime - dtBeginTime).TotalSeconds.ToString() + " seconds.");

            EventHubs_SampleProducer_Globals.LogFile.Log("Kafka Producer finished sending records to " + sTopicName + " topic. Success count is " +
                iSuccessCount.ToString() + ", FailCount is: " + iFailCount.ToString() + ".");
        }

        
        private static string WaitForKafka(List<Task> taskList)
        {
            Task t;
            string sResult = "";

            try
            {
                // Task t = await Task.WhenAll(taskList);
                t = Task.WhenAll(taskList);
                t.Wait();
                sResult = t.Status.ToString();
            }
            catch (KafkaException ke)
            {
                EventHubs_SampleProducer_Globals.LogFile.LogError(ke);
                EventHubs_SampleProducer_Globals.LogFile.LogErrorText(ke.ToString());
            }

            return sResult;
        }
        
    }
}
