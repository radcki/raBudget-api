using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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

            services.AddLogging(builder =>
                                    builder
                                       .AddDebug()
                                       .AddConsole()
                                       .AddConfiguration(Configuration.GetSection("Logging"))
                                       .SetMinimumLevel(LogLevel.Debug)
                               );
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
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
                                       {
                                           x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       })
                    .AddJwtBearer(x =>
                                  {
                                      x.Events = new JwtBearerEvents
                                                 {
                                                     OnTokenValidated = context =>
                                                                        {
                                                                            var userService =
                                                                                context.HttpContext.RequestServices
                                                                                       .GetRequiredService<UserService>();
                                                                            var userId =
                                                                                int.Parse(context.Principal
                                                                                                 .FindFirst(ClaimTypes
                                                                                                               .NameIdentifier)
                                                                                                 .Value);
                                                                            var user = userService.GetById(userId);
                                                                            if (user == null)
                                                                                context.Fail("Unauthorized");

                                                                            return Task.CompletedTask;
                                                                        },
                                                     OnAuthenticationFailed = context =>
                                                                              {
                                                                                  if (context.Exception.GetType() ==
                                                                                      typeof(
                                                                                          SecurityTokenExpiredException)
                                                                                  )
                                                                                      context.Response.Headers
                                                                                             .Add("Token-Expired",
                                                                                                  "true");
                                                                                  return Task.CompletedTask;
                                                                              },
                                                     OnMessageReceived = context =>
                                                                         {
                                                                             var accessToken = context.Request.Query["access_token"];
                                                                             var path = context.HttpContext.Request.Path;
                                                                             if (!string.IsNullOrEmpty(accessToken) &&
                                                                                 (path.StartsWithSegments("/hubs")))
                                                                             {
                                                                                 context.Token = accessToken;
                                                                             }

                                                                             return Task.CompletedTask;
                                                                         }
                                                 };
                                      x.RequireHttpsMetadata = !IsDebug;
                                      x.SaveToken = true;
                                      x.TokenValidationParameters = new TokenValidationParameters
                                                                    {
                                                                        ValidateIssuerSigningKey = true,
                                                                        ValidateAudience = false,
                                                                        IssuerSigningKey =
                                                                            new SymmetricSecurityKey(key),
                                                                        ValidateIssuer = false,
                                                                        ValidateLifetime = true
                                                                    };
                                  });

            services.AddAuthorization(options =>
                                      {
                                          options.AddPolicy("AdminOnly",
                                                            policy => policy.RequireClaim(ClaimTypes.Role,
                                                                                          eRole.Admin.ToString()));
                                          options.AddPolicy("UsersOnly",
                                                            policy => policy.RequireClaim(ClaimTypes.Role,
                                                                                          eRole.User.ToString()));
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