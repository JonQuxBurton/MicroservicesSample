using Infrastructure.Rest;
using RestSharp;

namespace Infrastructure.Events
{
    public class EventGetter : IEventGetter
    {
        private readonly IRestGetter restGetter;

        public EventGetter(IRestGetter restGetter)
        {
            this.restGetter = restGetter;
        }

        public IRestResponse Get(string url, long start, int chunkSize)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-type", "application/json");
            request.AddQueryParameter("start", start.ToString());
            request.AddQueryParameter("end", (start + chunkSize).ToString());

            return this.restGetter.Get(request);
        }
    }
}
