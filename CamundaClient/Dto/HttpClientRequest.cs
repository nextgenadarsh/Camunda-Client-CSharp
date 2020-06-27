
namespace CamundaClient.Dto
{
    public class HttpClientRequest<T>
    {
        public string RequestUri { get; protected set; }

        public T Request { get; set; }

        public HttpClientRequest(string requestUri, T request)
        {
            RequestUri = requestUri;
            Request = request;
        }
    }
}
