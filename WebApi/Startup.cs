using Microsoft.OpenApi.Models;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.Filters;
using WebApi.Helpers;
using WebApi.Services;
using WebApi.Contexts;
using WebApi.Models.Enum;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using WebApi.Controllers;
using WebApi.Hubs;
using WebApi.Providers;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();


            switch (Configuration["Data:ServerType"])
            {
                case "mysql":
                    services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies()
                                                                         .UseMySql(Configuration["Data:ConnectionString"]));
                    break;

                case "sqlserver":
                    services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies()
                                                                         .UseSqlServer(Configuration["Data:ConnectionString"]));
                    break;
            }

            /* AZURE IN-APP MYSQL
            string connectionString = Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb");
            services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies()
                                                                    .UseMySql(AzureMySQL.ToMySQLStandard(connectionString) + ";CHARSET=utf8;"));
                                                                    */

            if (Configuration.GetSection("Data").GetValue<bool>("AutoMigration"))
            {
                services.BuildServiceProvider().GetService<DataContext>().Database.Migrate();
            }

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "raBudget API", Version = "v1"}); });

            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); });
            services.Configure<ForwardedHeadersOptions>(options =>
                                                        {
                                                            options.ForwardedHeaders =
                                                                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                                                        });
            services.AddAutoMapper(typeof(UsersController));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // jwt authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                                  {
                                      options.Authority = Configuration["Authentication:Authority"];
                                      options.Audience = Configuration["Authentication:Audience"];
                                      if (IsDebug)
                                      {
                                          options.RequireHttpsMetadata = false;
                                          options.Events = new JwtBearerEvents()
                                                           {
                                                               OnAuthenticationFailed = c =>
                                                                                        {
                                                                                            c.NoResult();
                                                                                            c.Response.StatusCode = 500;
                                                                                            c.Response.ContentType = "text/plain";

                                                                                            return c.Response.WriteAsync(c.Exception.ToString());
                                                                                        }
                                                           };
                                      }
                                      options.SaveToken = true;
                                      options.Validate();
                                  });

            services.AddAuthorization(options =>
                                      {
                                          options.AddPolicy("admin",
                                                            policy => policy.RequireClaim(ClaimTypes.Role,
                                                                                          eRole.Admin.ToString()));
                                      }
                                     );

            // DEPENDENCY INJECTION
            services.AddScoped<UserService>();
            services.AddScoped<BudgetsNotifier>();
            services.AddScoped<TransactionsNotifier>();

            services.AddSignalR();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /* FOR NGINX REVERSE PROXY */
            app.UseForwardedHeaders();
            app.UsePathBase("/api");

            /*
             * CORS
             */
            if (IsDebug)
            {
                app.UseCors(x => x
                                .WithOrigins("http://localhost:8080")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .WithExposedHeaders("Token-Expired")
                           );

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCors(x => x
                                .WithOrigins("http://budget.rabt.pl")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .WithExposedHeaders("Token-Expired")
                           );
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "raBudget API V1"); });

            app.UseMvc();
            app.UseHttpsRedirection();

            app.UseSignalR(routes =>
                           {
                               routes.MapHub<BudgetsHub>("/hubs/budgets");
                               routes.MapHub<TransactionsHub>("/hubs/transactions");
                           });
        }

        public static bool IsDebug
        {
            get
            {
                bool isDebug = false;
#if DEBUG
                isDebug = true;
#endif
                return isDebug;
            }
        }
    }
}