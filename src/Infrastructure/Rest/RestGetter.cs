using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Rest
{
    public class RestGetter : IRestGetter
    {
        private readonly IRestClient restClient;

        public RestGetter(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public IRestResponse Get(IRestRequest restRequest)
        {
            return this.restClient.Execute(restRequest);
        }
    }
}
