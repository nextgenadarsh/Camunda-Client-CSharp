using System;
using CamundaClient;
using CamundaClient.Dto;
using CamundaClient.Services;

namespace CamundaClientDemo
{
    public class ChargeCardSubscriptionWorker : SubscriptionWorker
    {
        public string WorkderId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void ExecuteTask<T>(T task, IExternalTaskService externalTaskService)
        {
            var t = task as ExternalTask;

            Console.WriteLine("Processing task Id: " + t.Id);
            Console.WriteLine("Charging card");
            externalTaskService.Complete(t.WorkerId, t.Id, null);
        }
    }
}
