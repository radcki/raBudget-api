using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using raBudget.EfPersistence.Contexts;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace WebApi
{
    public class Program
    {
        #region Methods

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
                                                logging.AddConsole();
                                            })
                          .UseStartup<Startup>()
                          .UseUrls("http://localhost:4002")
                          .Build();
        }

        #endregion
    }
}