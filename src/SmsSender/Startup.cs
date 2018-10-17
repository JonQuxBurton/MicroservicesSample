using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nancy.Owin;
using Polly;
using Polly.Retry;
using SmsSender.Config;
using SmsSender.Data;

namespace SmsSender
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<AppSettings>(Configuration);
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseOwin(x => x.UseNancy());

            IOptions<AppSettings> options = app.ApplicationServices.GetService<IOptions<AppSettings>>();

            Console.WriteLine("AppSettings:");
            Console.WriteLine($"ConnectionString: {options.Value.ConnectionString}");

            RetryPolicy retry = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(40),
                    TimeSpan.FromSeconds(80)
                });

            var dataStore = new SmsSenderDataStore(options);

            retry.Execute(() => dataStore.SetupDatabase());
        }
    }
}
