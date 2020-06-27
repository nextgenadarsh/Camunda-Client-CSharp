using System;
using System.Collections.Generic;

namespace CamundaClient.Dto
{
    public class TopicSubscriptionInfo
    {
        public long PollingIntervalInMilliseconds { get; set; }
        public long LockDurationInMilliseconds { get; set; }
        public int Retries { get; set; }
        public long RetryTimeout { get; set; }
        public Type Type { get; set; }
        public string TopicName { get; set; }
        public List<string> VariablesToFetch { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
        public int MaxTasksToFetch { get; set; }

        public TopicSubscriptionInfo()
        {
            PollingIntervalInMilliseconds = 30 * 1000; // 30 second
            LockDurationInMilliseconds = 1 * 60 * 1000; // 1 minute
            MaxDegreeOfParallelism = 5;
            MaxTasksToFetch = 5;
        }
    }
}
