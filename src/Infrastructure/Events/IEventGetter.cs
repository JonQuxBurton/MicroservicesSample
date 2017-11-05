using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.Events
{
    public interface IEventGetter
    {
        Task<IRestResponse> Get(string url, long start, int chunkSize);
    }
}
