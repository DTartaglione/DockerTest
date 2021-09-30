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
namespace Crossing_Data_Consumer
{
    public class Crossing_Data_Consumer_KafkaConsumer
    {
        /// <summary>
        ///     In this example
        ///         - offsets are manually committed.
        ///         - no extra thread is created for the Poll (Consume) loop.
        /// </summary>
        /// 
        public static void StartKafkaConsumer()
        {
            var sBrokerList = Crossing_Data_Consumer_Globals.sKafkaBrokerList;
            var sKafkaTopics = Crossing_Data_Consumer_Globals.sKafkaTopic;

            Crossing_Data_Consumer_Globals.LogFile.Log("Start Kafka Consumer at: " + sBrokerList + ", listen on topic(s): " + sKafkaTopics);

            Run_Consume(sBrokerList, sKafkaTopics.Split(',').ToList());
        }


        //public static void Run_Consume(string brokerList, List<string> topics, CancellationToken cancellationToken)
        public static void Run_Consume(string brokerList, List<string> topics)
        {
            int iCount = 0;
           // Crossing_Data_Consumer_DataStructures.Crossing_Data_Consumer_LinkData _LinkData = new Crossing_Data_Consumer_DataStructures.Crossing_Data_Consumer_LinkData();
            string sVal = "";
            //List<string> lstKafkaData = new List<string>();
            List<string> lstData = new List<string>();
            bool bPrintedOffset = false;


            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = Crossing_Data_Consumer_Globals.sKafkaConsumerGroup,
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 1000,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 30000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                //EnableAutoOffsetStore = true,
                EnablePartitionEof = true
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
                            var consumeResult = consumer.Consume(new CancellationToken());

                            if (!bPrintedOffset)
                            {
                                Crossing_Data_Consumer_Globals.LogFile.Log("Launched consumer for topic " + consumeResult.Topic + ", beginning at offset: " + consumeResult.Offset + ".");
                                bPrintedOffset = true;
                            }

                            if (consumeResult.IsPartitionEOF)
                            {
                                Crossing_Data_Consumer_Globals.LogFile.Log("Reached end of topic " + consumeResult.Topic + ", partition " + 
                                       consumeResult.Partition + ", offset " +consumeResult.Offset + ".");

                               // consumer.StoreOffset(consumeResult);
                                //consumer.Commit(consumeResult);

                                if (lstData.Count > 0)
                                {
                                    Crossing_Data_Consumer_HandleResponse.HandleResponse(lstData);

                                    lstData.Clear();
                                    //lstKafkaData.Clear();
                                }

                                iCount = 0;

                                //commit last transaction and store offset
                                //consumer.StoreOffset(consumeResult.Offset);
                                //consumer.com

                                continue;
                            }

                            //Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                            

                           // if (consumeResult.Offset % commitPeriod == 0)
                           // {
                                // The Commit method sends a "commit offsets" request to the Kafka
                                // cluster and synchronously waits for the response. This is very
                                // slow compared to the rate at which the consumer is capable of
                                // consuming messages. A high performance application will typically
                                // commit offsets relatively infrequently and be designed handle
                                // duplicate messages in the event of failure.
                            try
                            {
                                //consumer.Commit(consumeResult);
                                sVal = consumeResult.Value.ToString();
                                //lstKafkaData = sVal.Split(',').ToList();
                                lstData.Add(sVal);

                                if (iCount == 0)
                                {
                                    iCount++;
                                    Crossing_Data_Consumer_Globals.LogFile.Log("Begin receiving data on topic " + Crossing_Data_Consumer_Globals.sKafkaTopic + 
                                        " starting at offset " + consumeResult.Offset.ToString() + ".");
                                }
                             }
                            catch (KafkaException e)
                            {
                                Console.WriteLine($"Commit error: {e.Error.Reason}");
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
