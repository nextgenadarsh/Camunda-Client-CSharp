using System;
using System.Collections.Generic;
using System.Net;
using CamundaClient.Common;
using CamundaClient.Dto;

namespace CamundaClient.Services
{
    public interface IDeploymentService
    {
        string Deploy(string deploymentName, IDictionary<string, byte[]> files);
    }

    public class DeploymentService : IDeploymentService
    {
        private readonly IHttpClientService _httpClientService;

        public DeploymentService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IList<ProcessDefinition> LoadProcessDefinitions(bool onlyLatest)
        {
            var processDefHttpRequest = new HttpClientRequest<NullOrVoid>("process-definition/?latestVersion=" + (onlyLatest ? "true" : "false"), default(NullOrVoid));

            return _httpClientService.GetAsync<NullOrVoid, IList<ProcessDefinition>>(processDefHttpRequest);

            //if (response.IsSuccessStatusCode)
            //{
                //var result = JsonConvert.DeserializeObject<IEnumerable<ProcessDefinition>>(response.Content.ReadAsStringAsync().Result);

                //// Could be extracted into separate method call if you run a lot of process definitions and want to optimize performance
                //foreach (ProcessDefinition pd in result)
                //{
                //    http = helper.HttpClient();
                //    HttpResponseMessage response2 = http.GetAsync("process-definition/" + pd.Id + "/startForm").Result;
                //    var startForm = JsonConvert.DeserializeObject<StartForm>(response2.Content.ReadAsStringAsync().Result);

                //    pd.StartFormKey = startForm.Key;
                //}
                //return new List<ProcessDefinition>(result);
            //}

        }


        public string LoadProcessDefinitionXml(String processDefinitionId)
        {
            var processDefHttpRequest = new HttpClientRequest<NullOrVoid>("process-definition/" + processDefinitionId + "/xml", default(NullOrVoid));

            var response = _httpClientService.GetAsync<NullOrVoid, ProcessDefinitionXml>(processDefHttpRequest);
            return response?.Bpmn20Xml;
        }

        public void DeleteDeployment(string deploymentId)
        {
            var deleteDeploymentHttpRequest = new HttpClientRequest<NullOrVoid>("deployment/" + deploymentId + "?cascade=true", default(NullOrVoid));
            _httpClientService.DeleteAsync<NullOrVoid>(deleteDeploymentHttpRequest);
        }

        public string Deploy(string deploymentName, IDictionary<string, byte[]> files)
        {
            var fileParameters = new List<object>();
            foreach(var file in files)
            {
                fileParameters.Add(new FileParameter(file.Value, file.Key));
            }

            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("deployment-name", deploymentName);
            postParameters.Add("deployment-source", "C# Process Application");
            postParameters.Add("enable-duplicate-filtering", "true");
            postParameters.Add("data", fileParameters);

            var createDeploymentHttpRequest = new HttpClientRequest<IDictionary<string, object>>("deployment/create", postParameters);
            Deployment deployment = _httpClientService.MultipartFormDataPost<IDictionary<string, object>, Deployment>(createDeploymentHttpRequest);

            return deployment?.Id;
        }
    }

}
