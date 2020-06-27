using System;
namespace CamundaClient.Dto
{
    public class BpmnErrorRequest
    {
        public string WorkerId { get; set; }
        public string ErrorCode { get; set; }
    }
}
