using Polly;
using RestSharp;

namespace Infrastructure.Rest
{
    public class RestPosterFactory : IRestPosterFactory
    {
        private readonly ISyncPolicy policy;

        public RestPosterFactory(ISyncPolicy policy)
        {
            this.policy = policy;
        }

        public IRestPoster Create(string url)
        {
            var restClient = new RestClient(url);
            var restPoster = new RestPoster(restClient, this.policy);

            return restPoster;
        }
    }
}
