using System;
using CamundaClient.Services;

namespace CamundaClient
{
    public interface ISubscriptionWorker
    {
        void ExecuteTask<T>(T taskResult, IExternalTaskService externalTaskService);
    }

    public abstract class SubscriptionWorker : ISubscriptionWorker
    {
        public string WorkerId { get; protected set; }

        public SubscriptionWorker()
        {
            WorkerId = Guid.NewGuid().ToString();
        }

        public virtual void ExecuteTask<T>(T taskResult, IExternalTaskService externalTaskService)
        {
            throw new NotImplementedException();
        }
    }
}
