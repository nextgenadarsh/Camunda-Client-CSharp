
namespace CamundaClient.Dto
{
    public class FailureRequest
    {
        public string WorkerId { get; set; }
        public string ErrorMessage { get; set; }
        public int Retries { get; set; }
        public long RetryTimeout { get; set; }
    }
}
