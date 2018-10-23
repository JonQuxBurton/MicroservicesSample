using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using SmsSender.Config;

namespace SmsSender
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly ILoggerFactory loggerFactory;

        public CustomBootstrapper(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory)
        {
            this.applicationBuilder = applicationBuilder;
            this.loggerFactory = loggerFactory;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            IOptions<AppSettings> options =
                this.applicationBuilder.ApplicationServices.GetService<IOptions<AppSettings>>();
            container.Register(options);
            container.Register(this.loggerFactory);
        }
    }
}