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

        public async Task<IRestResponse> Get(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            request.AddHeader("Content-type", "application/json");

            IRestResponse response = null;

            await this.policy.Execute(async () =>
            {
                response = this.restClient.Get(request);
            });

            return response;
        }
    }
}
