using System;
using FakeBt.Config;
using FakeBt.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Polly;
using Polly.Retry;

namespace FakeBt
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationBuilder applicationBuilder;

        public CustomBootstrapper(IApplicationBuilder applicationBuilder)
        {
            this.applicationBuilder = applicationBuilder;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            IOptions<AppSettings> options = this.applicationBuilder.ApplicationServices.GetService<IOptions<AppSettings>>();
            container.Register(options);

            RetryPolicy retry = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(40),
                    TimeSpan.FromSeconds(80)
                });

            var dataStore = new BtOrdersDataStore(options);

            retry.Execute(() => dataStore.SetupDatabase());
        }
    }
}
