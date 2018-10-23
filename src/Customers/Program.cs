using System.IO;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Customers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
            host.Run();
        }
    }
}