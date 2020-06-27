using System;
namespace CamundaClient.Engine
{
    public interface IEngineClient
    {

    }

    public class EngineClient : IEngineClient
    {
        protected const string EXTERNAL_TASK_RESOURCE_PATH = "/external-task";
        protected const string FETCH_AND_LOCK_RESOURCE_PATH = EXTERNAL_TASK_RESOURCE_PATH + "/fetchAndLock";
        public const string ID_PATH_PARAM = "{id}";
        protected const string ID_RESOURCE_PATH = EXTERNAL_TASK_RESOURCE_PATH + "/" + ID_PATH_PARAM;
        public const string UNLOCK_RESOURCE_PATH = ID_RESOURCE_PATH + "/unlock";
        public const string COMPLETE_RESOURCE_PATH = ID_RESOURCE_PATH + "/complete";
        public const string FAILURE_RESOURCE_PATH = ID_RESOURCE_PATH + "/failure";
        public const string BPMN_ERROR_RESOURCE_PATH = ID_RESOURCE_PATH + "/bpmnError";
        public const string EXTEND_LOCK_RESOURCE_PATH = ID_RESOURCE_PATH + "/extendLock";
        public const string NAME_PATH_PARAM = "{name}";
        public const string EXECUTION_RESOURCE_PATH = "/execution";
        public const string EXECUTION_ID_RESOURCE_PATH = EXECUTION_RESOURCE_PATH + "/" + ID_PATH_PARAM;
        public const string GET_LOCAL_VARIABLE = EXECUTION_ID_RESOURCE_PATH + "/localVariables/" + NAME_PATH_PARAM;
        public const string GET_LOCAL_BINARY_VARIABLE = GET_LOCAL_VARIABLE + "/data";


        public EngineClient(string workerId, int maxTasks, long asyncResponseTimeout, string baseUrl)
        {
            //this.workerId = workerId;
            //this.asyncResponseTimeout = asyncResponseTimeout;
            //this.maxTasks = maxTasks;
            //this.usePriority = usePriority;
            //this.engineInteraction = engineInteraction;
            //this.baseUrl = baseUrl;
        }

        //public EngineClient(string workerId, int maxTasks, long asyncResponseTimeout, string baseUrl)
        //{
            
        //}
    }
}
