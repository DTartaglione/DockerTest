// Copyright 2016-2018 Confluent Inc., 2015-2016 Andreas Heider
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Derived from: rdkafka-dotnet, licensed under the 2-clause BSD License.
//
// Refer to LICENSE for more information.

using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


/// <summary>
///     Demonstrates use of the Consumer client.
/// </summary>
namespace PA_Alerts_Data_Consumer
{
    public class PA_Alerts_Data_Consumer_KafkaConsumer
    {
        /// <summary>
        ///     In this example
        ///         - offsets are manually committed.
        ///         - no extra thread is created for the Poll (Consume) loop.
        /// </summary>
        /// 
        public static void StartKafkaConsumer()
        {
            var sBrokerList = PA_Alerts_Data_Consumer_Globals.sKafkaBrokerList;
            var sKafkaTopics = PA_Alerts_Data_Consumer_Globals.sKafkaTopic;

            PA_Alerts_Data_Consumer_Globals.LogFile.Log("Start Kafka Consumer at: " + sBrokerList + ", listen on topic(s): " + sKafkaTopics);

            Run_Consume(sBrokerList, sKafkaTopics.Split(',').ToList());
        }


        //public static void Run_Consume(string brokerList, List<string> topics, CancellationToken cancellationToken)
        public static void Run_Consume(string brokerList, List<string> topics)
        {
            int iCount = 0;
            string sVal = "";
            List<dynamic> lstData = new List<dynamic>();
            bool bPrintedOffset = false;
            bool bHasMessagesToSend = false;


            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = PA_Alerts_Data_Consumer_Globals.sKafkaConsumerGroup,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 1000,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 30000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                //EnableAutoOffsetStore = true,
                EnablePartitionEof = true,
                SecurityProtocol = (SecurityProtocol)PA_Alerts_Data_Consumer_Globals.iSecurityProtocol, //kafka needs 0, event hubs 3
                SaslMechanism = (SaslMechanism)PA_Alerts_Data_Consumer_Globals.iSaslMechanism, //Kafka needs 1, event hubs 1
                SaslUsername = PA_Alerts_Data_Consumer_Globals.sSaslUsername,
                SaslPassword = PA_Alerts_Data_Consumer_Globals.sSaslPassword
            };

            //const int commitPeriod = 1000;

            // Note: If a key or value deserializer is not set (as is the case below), the 
            // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
            // will be used automatically (where available). The default deserializer for string
            // is UTF8. The default deserializer for Ignore returns null for all input data
            // (including non-null data).
            using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                // Note: All handlers are called on the main .Consume thread.
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                //.SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                    // possibly manually specify start offsets or override the partition assignment provided by
                    // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                    // 
                    // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build())
            {

                consumer.Subscribe(topics);

                try
                {

                    while (true)
                    {
                        try
                        {
                            ConsumeResult<Ignore, string> consumeResult;

                            if (PA_Alerts_Data_Consumer_Globals.iSecurityProtocol == 0)
                            {
                                //Kafka consume result. Use cancellation token as it works with Kafka.
                                //Cancellation Token does not work with Event Hubs
                                consumeResult = consumer.Consume(new CancellationToken());
                                if (!bPrintedOffset)
                                {
                                    PA_Alerts_Data_Consumer_Globals.LogFile.Log("Launched consumer for topic " + consumeResult.Topic + ", beginning at offset: " + consumeResult.Offset + ".");
                                    bPrintedOffset = true;
                                }

                                if (consumeResult.IsPartitionEOF)
                                {
                                    PA_Alerts_Data_Consumer_Globals.LogFile.Log("Reached end of topic " + consumeResult.Topic + ", partition " +
                                           consumeResult.Partition + ", offset " + consumeResult.Offset + ".");

                                    if (lstData.Count > 0)
                                    {
                                        PA_Alerts_Data_Consumer_HandleResponse.HandleResponse(lstData);

                                        lstData.Clear();
                                        //lstKafkaData.Clear();
                                    }

                                    iCount = 0;
                                    continue;
                                }

                                try
                                {
                                    sVal = consumeResult.Message.Value.ToString();
                                    lstData.Add(sVal);

                                    if (iCount == 0)
                                    {
                                        iCount++;
                                        PA_Alerts_Data_Consumer_Globals.LogFile.Log("Begin receiving data on topic " + PA_Alerts_Data_Consumer_Globals.sKafkaTopic +
                                            " starting at offset " + consumeResult.Offset.ToString() + ".");
                                    }
                                }
                                catch (KafkaException e)
                                {
                                    Console.WriteLine($"Commit error: {e.Error.Reason}");
                                }
                            }
                            else if (PA_Alerts_Data_Consumer_Globals.iSecurityProtocol == 3)
                            {
                                //Event Hubs consume result. Need to use timeout vs. cancellation token. Cancellation Token does not work with Event Hubs
                                consumeResult = consumer.Consume(PA_Alerts_Data_Consumer_Globals.iKafkaConsumerConsumeTimeoutInMillieconds);

                                //Console.WriteLine("in consume " + DateTime.Now.ToString("HH:mm:ss"));

                                if (consumeResult == null)
                                {
                                    if (bHasMessagesToSend)
                                    {
                                        Console.WriteLine("sending messages after timeout");

                                        if (consumeResult != null)
                                        {
                                            PA_Alerts_Data_Consumer_Globals.LogFile.Log("Reached end of topic " + consumeResult.Topic + ", partition " +
                                                   consumeResult.Partition + ", offset " + consumeResult.Offset + ".");
                                        }

                                        if (lstData.Count > 0)
                                        {
                                            PA_Alerts_Data_Consumer_HandleResponse.HandleResponse(lstData);
                                            lstData.Clear();
                                        }

                                        iCount = 0;
                                        bHasMessagesToSend = false;
                                        continue;
                                    }
                                    continue;
                                }
                                else
                                {
                                    if (consumeResult.Message == null)
                                    {
                                        continue;
                                    }
                                }

                                if (!bPrintedOffset)
                                {
                                    PA_Alerts_Data_Consumer_Globals.LogFile.Log("Launched consumer for topic " + consumeResult.Topic + ", beginning at offset: " + consumeResult.Offset + ".");
                                    bPrintedOffset = true;
                                }

                                try
                                {

                                    if (iCount == 0)
                                    {
                                        iCount++;
                                        PA_Alerts_Data_Consumer_Globals.LogFile.Log("Begin receiving data on topic " + PA_Alerts_Data_Consumer_Globals.sKafkaTopic +
                                            " starting at offset " + consumeResult.Offset.ToString() + ".");
                                    }

                                    sVal = consumeResult.Message.Value.ToString();
                                    lstData.Add(sVal);
                                    bHasMessagesToSend = true;
                                }
                                catch (KafkaException e)
                                {
                                    Console.WriteLine($"Commit error: {e.Error.Reason}");
                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }

                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }
        }

        /// <summary>
        ///     In this example
        ///         - consumer group functionality (i.e. .Subscribe + offset commits) is not used.
        ///         - the consumer is manually assigned to a partition and always starts consumption
        ///           from a specific offset (0).
        /// </summary>
        //public static void Run_ManualAssign(string brokerList, List<string> topics, CancellationToken cancellationToken)
        //{
        //    var config = new ConsumerConfig
        //    {
        //        // the group.id property must be specified when creating a consumer, even 
        //        // if you do not intend to use any consumer group functionality.
        //        GroupId = new Guid().ToString(),
        //        BootstrapServers = brokerList,
        //        // partition offsets can be committed to a group even by consumers not
        //        // subscribed to the group. in this example, auto commit is disabled
        //        // to prevent this from occuring.
        //        EnableAutoCommit = true
        //    };

        //    using (var consumer =
        //        new ConsumerBuilder<Ignore, string>(config)
        //            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
        //            .Build())
        //    {
        //        consumer.Assign(topics.Select(topic => new TopicPartitionOffset(topic, 0, Offset.Beginning)).ToList());

        //        try
        //        {
        //            while (true)
        //            {
        //                try
        //                {
        //                    var consumeResult = consumer.Consume(cancellationToken);
        //                    // Note: End of partition notification has not been enabled, so
        //                    // it is guaranteed that the ConsumeResult instance corresponds
        //                    // to a Message, and not a PartitionEOF event.
        //                    Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: ${consumeResult.Value}");
        //                }
        //                catch (ConsumeException e)
        //                {
        //                    Console.WriteLine($"Consume error: {e.Error.Reason}");
        //                }
        //            }
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            Console.WriteLine("Closing consumer.");
        //            consumer.Close();
        //        }
        //    }
        //}

        //private static void PrintUsage()
        //    => Console.WriteLine("Usage: .. <subscribe|manual> <broker,broker,..> <topic> [topic..]");


    }
}
