using System.IO;
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
using Microsoft.AspNetCore.SpaServices.Webpack;
using System;
using System.Collections.Generic;
using kedzior.io.ConnectionStringConverter;
using WebApi.Contexts;
using WebApi.Models.Enum;
using Microsoft.AspNetCore.HttpOverrides;

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

            
            services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies()
                                                                    .UseMySql("server=localhost;uid=root;pwd=;database=localdb"));
            /* SQL SERVER 
            services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies()
                                                                    .UseSqlServer(Configuration
                                                                                    ["Data:DefaultConnection:ConnectionString"]));
            */

            /* AZURE IN-APP MYSQL
            string connectionString = Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb");
            services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies()
                                                                    .UseMySql(AzureMySQL.ToMySQLStandard(connectionString) + ";CHARSET=utf8;"));
                                                                    */
            
            /* AUTOMIGRATION
            services.BuildServiceProvider().GetService<DataContext>().Database.Migrate();
            */

            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); });
            services.AddAutoMapper();

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

            services.AddSpaStaticFiles(config =>
                                       {
                                           config.RootPath = @"./wwwroot";
                                       });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
          
            /*
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            */
            /* FOR NGINX REVERSE PROXY */
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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
            app.UseMvc();
            app.UseHttpsRedirection();

            if (!IsDebug)
            {                
                app.UseStaticFiles();
                app.UseSpaStaticFiles();
                app.UseSpa(config => { });
            }
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