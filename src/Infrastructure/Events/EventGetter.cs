using Infrastructure.Rest;
using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.Events
{
    public class EventGetter : IEventGetter
    {
        private readonly IRestGetterFactory restGetterFactory;

        public EventGetter(IRestGetterFactory restGetterFactory)
        {
            this.restGetterFactory = restGetterFactory;
        }

        public async Task<IRestResponse> Get(string url, long start, int chunkSize)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-type", "application/json");
            request.AddQueryParameter("start", start.ToString());
            request.AddQueryParameter("end", (start + chunkSize).ToString());

            var getter = this.restGetterFactory.Create(url);

            return await getter.Get(request);
        }
    }
}
