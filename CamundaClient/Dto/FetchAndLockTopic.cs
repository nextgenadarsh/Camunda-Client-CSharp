
using System.Collections.Generic;

namespace CamundaClient.Dto
{
    public class FetchAndLockTopic
    {
        public string TopicName { get; set; }

        public long LockDuration { get; set; }

        public IList<string> Variables { get; set; }

        public FetchAndLockTopic()
        {
            Variables = new List<string>();
        }
    }
}
