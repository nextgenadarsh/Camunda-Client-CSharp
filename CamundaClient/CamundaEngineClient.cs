
using System;
using System.Collections.Generic;
using CamundaClient.Common;
using CamundaClient.Dto;
using CamundaClient.Services;

namespace CamundaClient
{
    public interface ICamundaEngineClient
    {
        IDeploymentService DeploymentService();

        ITopicSubscription SubscribeTopic(TopicSubscriptionInfo topicSubscriptionInfo);
    }

    public class CamundaEngineClient : ICamundaEngineClient
    {
        public static string ENGINE_URL = "http://localhost:8080/engine-rest/engine/default/";
        public static string COCKPIT_URL = "http://localhost:8080/camunda/app/cockpit/default/";

        protected IHttpClientService _httpClientService;
        protected IList<ITopicSubscription> _topicSubscriptions;

        public Uri EngineUri { get; private set; }
        public string UserName { get; protected set; }
        public string Password { get; protected set; }

        private IExternalTaskService _externalTaskService => new ExternalTaskService(_httpClientService);
        private IDeploymentService _deploymentService => new DeploymentService(_httpClientService);
        public IDeploymentService DeploymentService()
        {
            return _deploymentService;
        }

        public CamundaEngineClient() : this(new Uri(ENGINE_URL), null, null) { }

        public CamundaEngineClient(Uri engineUri, string userName, string password)
        {
            EngineUri = engineUri;
            UserName = userName;
            Password = password;

            _httpClientService = new HttpClientService(new HttpClientProvider(EngineUri, UserName, Password));
            _topicSubscriptions = new List<ITopicSubscription>();
        }

        public ITopicSubscription SubscribeTopic(TopicSubscriptionInfo topicSubscriptionInfo)
        {
            var newTopicSubscription = new TopicSubscription(topicSubscriptionInfo, _externalTaskService);
            _topicSubscriptions.Add(newTopicSubscription);

            return newTopicSubscription;
        }

        public string BaseUrl { protected set; get; }

        public string CockpitUrl { protected set; get; }

        public string WorkerId { protected set; get; }

        public int MaxTasks { protected set; get; }

        public void Initialize()
        {
            
        }
    }
}
