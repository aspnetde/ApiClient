using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SixtyNineDegrees.ApiClient.Json.FluentSyntax
{
    public class FluentApiStep2 : IFluentStep2
    {
        private readonly Func<Task<HttpResponseMessage>> _createResponseTask;

        public FluentApiStep2(Func<Task<HttpResponseMessage>> createResponseTask)
        {
            _createResponseTask = createResponseTask;
        }

        public async Task<object> AndRespond()
        {
            return await AndRespondWith<object>();
        }

        public async Task<T> AndRespondWith<T>()
        {
            using (var responseMessage = await _createResponseTask())
            {
                string responseString = await responseMessage.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(responseString);
            }
        }
    }
}
