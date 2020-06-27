using System;
using System.Collections.Generic;

namespace CamundaClient.Dto
{
    public class FetchAndLockRequest
    {
        public string WorkerId { get; set; }

        public int MaxTasks { get; set; }

        public bool UsePriority { get; set; }

        public IList<FetchAndLockTopic> Topics { get; set; }

        public FetchAndLockRequest()
        {
            Topics = new List<FetchAndLockTopic>();
        }
    }
}
