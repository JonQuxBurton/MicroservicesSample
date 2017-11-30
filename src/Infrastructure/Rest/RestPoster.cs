using Polly;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Rest
{
    public class RestPoster : IRestPoster
    {
        private readonly IRestClient restClient;
        private readonly ISyncPolicy policy;

        public RestPoster(IRestClient restClient, ISyncPolicy policy)
        {
            this.restClient = restClient;
            this.policy = policy;
        }

        public async Task<IRestResponse> Post(string resource, object body)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(body);

            IRestResponse response = null;

            await this.policy.Execute(async () =>
            {
                response = this.restClient.Execute(request);
            });

            return response;
        }
    }
}
