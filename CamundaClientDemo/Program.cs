using System;
using System.Collections.Generic;
using CamundaClient;
using CamundaClient.Dto;

namespace CamundaClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting !");

            byte[] bytes = System.IO.File.ReadAllBytes("calculation.bpmn");
            IDictionary<string, byte[]> filesToUpload = new Dictionary<string, byte[]>();
            filesToUpload.Add("calculator.bpmn", bytes);

            ICamundaEngineClient camundaEngineClient = new CamundaEngineClient();
            camundaEngineClient.DeploymentService().Deploy("testDeployment", filesToUpload);

            var subscriptionWorker = new ChargeCardSubscriptionWorker();
            var topicSubscriptionInfo = new TopicSubscriptionInfo()
            {
                TopicName = "calculate",
                PollingIntervalInMilliseconds = 2000
            };

            using var topicSubscription = camundaEngineClient.SubscribeTopic(topicSubscriptionInfo).Worker(subscriptionWorker);
            topicSubscription.Start();

            Console.WriteLine("Press any key to stop the workers...");
            Console.ReadKey();
        }
    }
}
