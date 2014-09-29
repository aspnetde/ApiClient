using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SixtyNineDegrees.ApiClient.Extensions;

namespace SixtyNineDegrees.ApiClient.Json.FluentSyntax
{
    public class FluentApiStep1 : IFluentStep1
    {
        private readonly object _data;
        private readonly string _httpMethod;
        private readonly HttpClient _httpClient;
        private readonly CancellationToken _requestCancellationToken;

        public FluentApiStep1(object data, string httpMethod, HttpClient httpClient, CancellationToken requestCancellationToken)
        {
            _data = data;
            _httpMethod = httpMethod;
            _httpClient = httpClient;
            _requestCancellationToken = requestCancellationToken;
        }

        public IFluentStep2 To(string endpointFormat, params object[] parameters)
        {
            string endpoint = string.Format(endpointFormat, parameters);
            HttpContent jsonContent = GetSerializedJsonContent();
            var httpMethod = GetHttpMethod();

            return new FluentApiStep2(() => httpMethod(endpoint, jsonContent, _requestCancellationToken));
        }

        private Func<string, HttpContent, CancellationToken, Task<HttpResponseMessage>> GetHttpMethod()
        {
            switch (_httpMethod)
            {
                case "POST": return _httpClient.PostAsync;
                case "PATCH": return _httpClient.PatchAsync;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private HttpContent GetSerializedJsonContent()
        {
            return new StringContent(JsonConvert.SerializeObject(_data))
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };
        }
    }
}
