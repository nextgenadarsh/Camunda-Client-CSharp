using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CamundaClient.Common
{
    public class HttpClientProvider
    {
        public const string CONTENT_TYPE_JSON = "application/json";

        public Uri EngineUri { get; protected set; }
        public string Username { get; }
        public string Password { get; }

        private static HttpClient client;

        public HttpClientProvider(Uri engineUri) : this(engineUri, null, null)
        {
        }

        public HttpClientProvider(Uri engineUri, string username, string password)
        {
            EngineUri = engineUri;
            Username = username;
            Password= password;
        }


        public HttpClient HttpClient()
        {
            if (client == null)
            {
                if (!string.IsNullOrWhiteSpace(Username))
                {
                    var credentials = new NetworkCredential(Username, Password);
                    client = new HttpClient(new HttpClientHandler() { Credentials = credentials });
                }
                else
                {
                    client = new HttpClient();
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite); // Infinite / really?
                }
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CONTENT_TYPE_JSON));
                client.BaseAddress = EngineUri;
            }

            return client;
        }        
    }
}
