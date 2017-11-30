using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Rest
{
    public class RestGetterFactory : IRestGetterFactory
    {
        public IRestGetter Create(string url)
        {
            var restClient = new RestClient(url);
            var restGetter = new RestGetter(restClient);

            return restGetter;
        }
   
    }
}
