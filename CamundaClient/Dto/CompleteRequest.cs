using System;
using System.Collections.Generic;

namespace CamundaClient.Dto
{
    public class CompleteRequest
    {
        public string BusinessKey { get; set; }
        public Dictionary<string, Variable> Variables { get; set; }
        public string WorkerId { get; set; }
    }
}
