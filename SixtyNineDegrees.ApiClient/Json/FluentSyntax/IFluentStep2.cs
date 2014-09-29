using System.Threading.Tasks;

namespace SixtyNineDegrees.ApiClient.Json.FluentSyntax
{
    public interface IFluentStep2
    {
        Task<object> AndRespond();
        Task<T> AndRespondWith<T>();
    }
}
