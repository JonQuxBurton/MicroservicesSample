using Polly;
using RestSharp;
using System;

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

        public IRestResponse Post(string resource, object body)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(body);

            IRestResponse response = null;

            this.policy.Execute(() =>
            {
                response = this.restClient.Execute(request);
            });

            return response;
        }
    }
}
