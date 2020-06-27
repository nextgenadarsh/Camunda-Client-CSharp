
using System;
using System.Threading;
using System.Threading.Tasks;
using CamundaClient.Dto;
using CamundaClient.Services;

namespace CamundaClient
{
    public interface ITopicSubscription
    {
        TopicSubscription Worker(SubscriptionWorker topicSubscriptionHandler);

        TopicSubscription Start();
    }

    public class TopicSubscription : ITopicSubscription, IDisposable
    {
        private readonly TopicSubscriptionInfo _topicSubscriptionInfo;
        private readonly IExternalTaskService _externalTaskService;
        private SubscriptionWorker _subscriptionWorker;

        private Timer _taskTimer;
        
        public TopicSubscription(TopicSubscriptionInfo topicSubscriptionInfo, IExternalTaskService externalTaskService)
        {
            _topicSubscriptionInfo = topicSubscriptionInfo;
            _externalTaskService = externalTaskService;
        }

        public TopicSubscription Worker(SubscriptionWorker subscriptionWorker)
        {
            _subscriptionWorker = subscriptionWorker;
            return this;
        }

        private void DoPolling()
        {
            var tasks = _externalTaskService.FetchAndLockTasks(_subscriptionWorker.WorkerId, _topicSubscriptionInfo.MaxTasksToFetch, _topicSubscriptionInfo.TopicName, _topicSubscriptionInfo.LockDurationInMilliseconds, null);

            Parallel.ForEach(
                    tasks,
                    new ParallelOptions { MaxDegreeOfParallelism = _topicSubscriptionInfo.MaxDegreeOfParallelism },
                    externalTask => _subscriptionWorker.ExecuteTask(externalTask, _externalTaskService)
                );


            // schedule next run (if not stopped in between)
            if (_taskTimer != null)
            {
                _taskTimer.Change(TimeSpan.FromMilliseconds(_topicSubscriptionInfo.PollingIntervalInMilliseconds), TimeSpan.FromMilliseconds(Timeout.Infinite));
            }
        }

        public TopicSubscription Start()
        {
            this._taskTimer = new Timer(_ => DoPolling(), null, _topicSubscriptionInfo.PollingIntervalInMilliseconds, Timeout.Infinite);
            return this;
        }

        public void Stop()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this._taskTimer != null)
            {
                this._taskTimer.Dispose();
                this._taskTimer = null;
            }
        }
    }
}
