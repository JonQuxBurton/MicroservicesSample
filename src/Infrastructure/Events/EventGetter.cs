using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.Events
{
    public class EventGetter : IEventGetter
    {
        private readonly IRestClient restClient;

        public EventGetter(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<IRestResponse> Get(string url, long start, int chunkSize)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Content-type", "application/json");
            request.AddQueryParameter("start", start.ToString());
            request.AddQueryParameter("end", (start + chunkSize).ToString());

            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = this.restClient.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            return (RestResponse)(await taskCompletion.Task);

            //return this.restClient.Execute(request);
        }
    }
}
