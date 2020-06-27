
using System.Collections.Generic;
using System.Linq;
using CamundaClient.Common;
using CamundaClient.Dto;

namespace CamundaClient.Services
{
    public interface IExternalTaskService
    {
        IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, string topicName, long lockDurationInMilliseconds, IEnumerable<string> variablesToFetch = null);

        IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, IEnumerable<string> topicNames, long lockDurationInMilliseconds, IEnumerable<string> variablesToFetch = null);

        void Complete(string workerId, string externalTaskId, Dictionary<string, object> variablesToPassToProcess = null);

        void Error(string workerId, string externalTaskId, string errorCode);

        void Failure(string workerId, string externalTaskId, string errorMessage, int retries, long retryTimeout);
    }

    public class ExternalTaskService : IExternalTaskService
    {
        private IHttpClientService _httpClientService;

        public ExternalTaskService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, string topicName, long lockDurationInMilliseconds, IEnumerable<string> variablesToFetch = null)
        {
            return FetchAndLockTasks(workerId, maxTasks, new List<string> { topicName }, lockDurationInMilliseconds, variablesToFetch);
        }

        public IList<ExternalTask> FetchAndLockTasks(string workerId, int maxTasks, IEnumerable<string> topicNames, long lockDurationInMilliseconds, IEnumerable<string> variablesToFetch = null)
        {
            var fetchAndLockRequest = new FetchAndLockRequest
            {
                WorkerId = workerId,
                MaxTasks = maxTasks
            };

            var fetchAndLockHttpRequest = new HttpClientRequest<FetchAndLockRequest>("external-task/fetchAndLock", fetchAndLockRequest);

            foreach (var topicName in topicNames)
            {
                var lockTopic = new FetchAndLockTopic
                {
                    TopicName = topicName,
                    LockDuration = lockDurationInMilliseconds,
                    Variables = variablesToFetch?.ToList()
                };
                fetchAndLockRequest.Topics.Add(lockTopic);
            }

            return _httpClientService.PostAsync<FetchAndLockRequest, IList<ExternalTask>>(fetchAndLockHttpRequest);
        }

        public void Complete(string workerId, string externalTaskId, Dictionary<string, object> variablesToPassToProcess = null)
        {
            var request = new CompleteRequest() {
                WorkerId = workerId,
                Variables = HttpClientService.ConvertVariables(variablesToPassToProcess)
            };

            var completeHttpRequest = new HttpClientRequest<CompleteRequest>("external-task/" + externalTaskId + "/complete", request);

            _httpClientService.PostAsync<CompleteRequest>(completeHttpRequest);
        }

        public void Error(string workerId, string externalTaskId, string errorCode)
        {
            var request = new BpmnErrorRequest() {
                WorkerId = workerId,
                ErrorCode = errorCode
            };

            var errorHttpRequest = new HttpClientRequest<BpmnErrorRequest>("external-task/" + externalTaskId + "/bpmnError", request);

            _httpClientService.PostAsync<BpmnErrorRequest>(errorHttpRequest);
        }

        public void Failure(string workerId, string externalTaskId, string errorMessage, int retries, long retryTimeout)
        {
            var request = new FailureRequest();
            request.WorkerId = workerId;
            request.ErrorMessage = errorMessage;
            request.Retries = retries;
            request.RetryTimeout = retryTimeout;

            var failureHttpRequest = new HttpClientRequest<FailureRequest>("external-task/" + externalTaskId + "/failure", request);

            _httpClientService.PostAsync<FailureRequest>(failureHttpRequest);
        }
    }
}
