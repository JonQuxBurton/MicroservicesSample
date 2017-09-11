using RestSharp;

namespace Infrastructure.Events
{
    public interface IEventGetter
    {
        IRestResponse Get(string url, long start, int chunkSize);
    }
}
