using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using WebApi.Helpers;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .ConfigureLogging((hostingContext, logging) =>
                                            {
                                                logging.AddFilter<EntityFrameworkLoggerProvider<DataContext>>("Microsoft", LogLevel.Warning);
                                                logging.AddFilter<EntityFrameworkLoggerProvider<DataContext>>("System", LogLevel.Warning);
                                                logging.AddEntityFramework<DataContext>();
                                            })
                          .UseStartup<Startup>()
                          .UseUrls("http://localhost:4000")
                          .Build();
        }
    }
}