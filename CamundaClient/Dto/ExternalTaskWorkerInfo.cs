﻿using System;
using System.Collections.Generic;

namespace CamundaClient.Dto
{
    public class ExternalTaskWorkerInfo
    {
        public int Retries { get; set; }
        public long RetryTimeout { get; set; }
        public Type Type { get; set; }
        public string TopicName { get; set; }
        public List<string> VariablesToFetch { get; set; }
    }
}
