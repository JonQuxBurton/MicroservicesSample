using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Rest
{
    public class RestGetter : IRestGetter
    {
        private readonly IRestClient restClient;

        public RestGetter(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<IRestResponse> Get(IRestRequest restRequest)
        {
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = this.restClient.ExecuteAsync(restRequest, r => taskCompletion.SetResult(r));

            return await taskCompletion.Task;
        }
    }
}
