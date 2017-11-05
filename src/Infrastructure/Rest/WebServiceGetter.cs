using Polly;
using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.Rest
{
    public class WebServiceGetter : IWebServiceGetter
    {
        private readonly IRestClient restClient;
        private readonly ISyncPolicy policy;

        public WebServiceGetter(IRestClient restClient, ISyncPolicy policy)
        {
            this.restClient = restClient;
            this.policy = policy;
        }

        public IRestResponse Get(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            request.AddHeader("Content-type", "application/json");

            IRestResponse response = null;

            this.policy.Execute(async () =>
            {
                TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
                RestRequestAsyncHandle handle = this.restClient.ExecuteAsync(request, r => taskCompletion.SetResult(r));
                response = (RestResponse)(await taskCompletion.Task);

                //response = this.restClient.Execute(request);
            });

            return response;
        }
    }
}
