using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SixtyNineDegrees.ApiClient.Json.FluentSyntax
{
    public class MultipartApiStep1 : IFluentStep1
    {
        private readonly string _httpMethod;
        private readonly MultipartFormDataContent _httpContents;
        private readonly HttpClient _httpClient;
        private readonly CancellationToken _requestCancellationToken;

        public MultipartApiStep1(string httpMethod, MultipartFormDataContent httpContents, HttpClient httpClient, CancellationToken requestCancellationToken)
        {
            _httpMethod = httpMethod;
            _httpContents = httpContents;
            _httpClient = httpClient;
            _requestCancellationToken = requestCancellationToken;
        }

        public IFluentStep2 To(string endpointFormat, params object[] parameters)
        {
            string requestUri = string.Format(endpointFormat, parameters);

            var message = new HttpRequestMessage
            {
                Method = new HttpMethod(_httpMethod),
                Content = _httpContents,
                RequestUri = new Uri(_httpClient.BaseAddress, requestUri)
            };

            Func<Task<HttpResponseMessage>> createResponseTask = () => _httpClient.SendAsync(message, _requestCancellationToken);

            return new FluentApiStep2(createResponseTask);
        }
    }
}
