using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SixtyNineDegrees.ApiClient.Json.FluentSyntax;

namespace SixtyNineDegrees.ApiClient.Json
{
    public class JsonApiClient
    {
        private readonly CancellationToken _requestCancellationToken;
        private readonly HttpClient _httpClient;

        public JsonApiClient(Uri apiRoot)
            : this(apiRoot, CancellationToken.None)
        {

        }

        public JsonApiClient(Uri apiRoot, CancellationToken requestCancellationToken)
        {
            _httpClient = new HttpClient { BaseAddress = apiRoot };
            _requestCancellationToken = requestCancellationToken;
        }

        public IFluentStep2 Get(string relativeUri)
        {
            return new FluentApiStep2(() => _httpClient.GetAsync(relativeUri, _requestCancellationToken));
        }

        public Task<T> Get<T>(string relativeUriFormat, params object[] parameters)
        {
            return new FluentApiStep2(() => _httpClient.GetAsync(string.Format(relativeUriFormat, parameters), _requestCancellationToken))
                .AndRespondWith<T>();
        }

        public IFluentStep2 Delete(string relativeUriFormat, params object[] parameters)
        {
            return new FluentApiStep2(() => _httpClient.DeleteAsync(string.Format(relativeUriFormat, parameters), _requestCancellationToken));
        }

        public IFluentStep1 Post()
        {
            return Post(string.Empty);
        }

        public IFluentStep1 Post(object data)
        {
            return Request(data, "POST");
        }

        public IFluentStep1 Patch(object data)
        {
            return Request(data, "PATCH");
        }

        public IFluentStep1 Request(object data, string httpMethod)
        {
            return new FluentApiStep1(data, httpMethod, _httpClient, _requestCancellationToken);
        }

        public IFluentStep1 PostMultipart(IEnumerable<KeyValuePair<string, string>> formValues, params ByteArrayContent[] media)
        {
            return MultipartRequest("POST", formValues, media);
        }

        public IFluentStep1 PatchMultipart(IEnumerable<KeyValuePair<string, string>> formValues, params ByteArrayContent[] media)
        {
            return MultipartRequest("PATCH", formValues, media);
        }

        private IFluentStep1 MultipartRequest(string httpMethod, IEnumerable<KeyValuePair<string, string>> formValues, params ByteArrayContent[] media)
        {
            var httpContents = new MultipartFormDataContent();

            foreach (var formValue in formValues)
            {
                var contentPart = new StringContent(formValue.Value ?? string.Empty, Encoding.UTF8);
                httpContents.Add(contentPart, formValue.Key);
            }

            foreach (var mediaContent in media)
            {
                httpContents.Add(mediaContent);
            }

            return new MultipartApiStep1(httpMethod, httpContents, _httpClient, _requestCancellationToken);
        }
    }
}
