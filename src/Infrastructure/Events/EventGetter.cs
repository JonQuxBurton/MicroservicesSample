using RestSharp;

namespace Infrastructure.Events
{
    public class EventGetter : IEventGetter
    {
        private readonly IRestClient restClient;

        public EventGetter(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public IRestResponse Get(string url, long start, int chunkSize)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-type", "application/json");
            request.AddQueryParameter("start", start.ToString());
            request.AddQueryParameter("end", (start + chunkSize).ToString());

            return this.restClient.Execute(request);
        }
    }
}
