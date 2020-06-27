using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using CamundaClient.Common.Exceptions;
using CamundaClient.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CamundaClient.Common
{


    public interface IHttpClientService
    {
        void GetAsync<S>(HttpClientRequest<S> clientRequest);

        T GetAsync<S, T>(HttpClientRequest<S> clientRequest);

        void PostAsync<S>(HttpClientRequest<S> clientRequest);

        T PostAsync<S, T>(HttpClientRequest<S> clientRequest);

        void DeleteAsync<S>(HttpClientRequest<S> clientRequest);

        T MultipartFormDataPost<S, T>(HttpClientRequest<IDictionary<string, object>> clientRequest);
    }

    public class HttpClientService : IHttpClientService
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        private HttpClientProvider _httpClientProvider;

        public HttpClientService(HttpClientProvider httpClientProvider)
        {
            _httpClientProvider = httpClientProvider;
        }

        public void GetAsync<S>(HttpClientRequest<S> clientRequest)
        {
            GetAsync<S, NullOrVoid>(clientRequest);
        }

        public T GetAsync<S, T>(HttpClientRequest<S> clientRequest)
        {
            var httpClient = _httpClientProvider.HttpClient();
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(clientRequest.Request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, HttpClientProvider.CONTENT_TYPE_JSON);
                var response = httpClient.GetAsync(clientRequest.RequestUri);
                var responseResult = response.Result;
                if (responseResult.IsSuccessStatusCode)
                {
                    if (clientRequest.Request is NullOrVoid)
                    {
                        // Don't deserialize if nothing is supposed to be returned
                    }
                    else
                    {
                        var tasks = JsonConvert.DeserializeObject<T>(responseResult.Content.ReadAsStringAsync().Result);
                        return tasks;
                    }
                }
                else
                {
                    throw new EngineException("Error while executing: " + clientRequest.RequestUri + " : " + responseResult.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // TODO: Handle Exception, add back off
                throw;
            }

            return default(T);
        }

        public void PostAsync<S>(HttpClientRequest<S> clientRequest)
        {
            PostAsync<S, NullOrVoid>(clientRequest);
        }

        public T PostAsync<S, T>(HttpClientRequest<S> clientRequest)
        {
            var httpClient = _httpClientProvider.HttpClient();
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(clientRequest.Request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, HttpClientProvider.CONTENT_TYPE_JSON);
                var response = httpClient.PostAsync(clientRequest.RequestUri, requestContent);
                var responseResult = response.Result;
                if (responseResult.IsSuccessStatusCode)
                {
                    if (clientRequest.Request is NullOrVoid)
                    {
                        // Don't deserialize if nothing is supposed to be returned
                    } else
                    {
                        var tasks = JsonConvert.DeserializeObject<T>(responseResult.Content.ReadAsStringAsync().Result);
                        return tasks;
                    }
                }
                else
                {
                    throw new EngineException("Error while executing: " + clientRequest.RequestUri + " : " + responseResult.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // TODO: Handle Exception, add back off
                throw;
            }

            return default(T);
        }

        public void DeleteAsync<S>(HttpClientRequest<S> clientRequest)
        {
            var httpClient = _httpClientProvider.HttpClient();
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(clientRequest.Request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, HttpClientProvider.CONTENT_TYPE_JSON);
                var response = httpClient.DeleteAsync(clientRequest.RequestUri);
                var responseResult = response.Result;
                if (!responseResult.IsSuccessStatusCode)
                {
                    throw new EngineException("Error while executing: " + clientRequest.RequestUri + " : " + responseResult.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // TODO: Handle Exception, add back off
                throw;
            }
        }

        public T MultipartFormDataPost<S, T>(HttpClientRequest<IDictionary<string, object>> clientRequest)
        {
            string multipartDataBoundry = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + multipartDataBoundry;

            byte[] formData = GetMultipartFormData(clientRequest.Request, multipartDataBoundry);

            HttpWebRequest request = WebRequest.Create(_httpClientProvider.EngineUri + clientRequest.RequestUri) as HttpWebRequest;

            if (request == null)
            {
                throw new EngineException("request is not a HTTP request");
            }

            request.Method = "POST";
            request.ContentType = contentType;
            //request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            if (!string.IsNullOrWhiteSpace(_httpClientProvider.Username))
            {
                request.PreAuthenticate = true;
                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(_httpClientProvider.Username + ":" + _httpClientProvider.Password)));
            }

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
                requestStream.Dispose();
            }
            var webResponse = request.GetResponse() as HttpWebResponse;
            using (var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
            {
                var result = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                return result;
            }
        }

        private static byte[] GetMultipartFormData(IDictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();

            // Thanks to feedback from commenter's, add a CRLF to allow multiple parameters to be added.
            // Skip it on the first parameter, add it to subsequent parameters.
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (param.Value is List<object>)
                {
                    // list of files
                    foreach (var value in (List<object>)param.Value)
                    {
                        if (needsCLRF)
                            formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                        AddFormData(boundary, formDataStream, param.Key, value);
                        needsCLRF = true;
                    }
                }
                else
                {
                    // only a single file
                    if (needsCLRF)
                        formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                    AddFormData(boundary, formDataStream, param.Key, param.Value);
                    needsCLRF = true;
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            formDataStream.Dispose();

            return formData;
        }

        private static void AddFormData(string boundary, Stream formDataStream, String key, object value)
        {
            var fileToUpload = value as FileParameter;
            if (fileToUpload != null)
            {
                // Add just the first part of this parameter, since we will write the file data directly to the Stream
                string header = string.Format(
                    CultureInfo.InvariantCulture,
                    "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                    boundary,
                    fileToUpload.FileName ?? key,
                    fileToUpload.FileName ?? key,
                    fileToUpload.ContentType ?? "application/octet-stream");

                formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                // Write the file data directly to the Stream, rather than serializing it to a string.
                formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
            }
            else
            {
                string postData = string.Format(
                    CultureInfo.InvariantCulture,
                    "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    key,
                    value);
                formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
            }
        }

        public static Dictionary<string, Variable> ConvertVariables(Dictionary<string, object> variables)
        {
            var result = new Dictionary<string, Variable>();
            if (variables == null)
            {
                return result;
            }
            foreach (var variable in variables)
            {
                Variable camundaVariable = new Variable
                {
                    Value = variable.Value
                };
                result.Add(variable.Key, camundaVariable);
            }
            return result;
        }
    }
}
