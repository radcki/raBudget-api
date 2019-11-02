using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using raBudget.Api.Filters;
using raBudget.Api.Providers;
using raBudget.Core.Dto.User.Response;
using raBudget.Core.Infrastructure;
using raBudget.Core.Infrastructure.AutoMapper;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.EfPersistence.Contexts;
using raBudget.EfPersistence.RepositoryImplementations;

namespace raBudget.Api
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
                    services.AddDbContext<IDataContext, DataContext>(options => options.UseMySql(Configuration.GetConnectionString("mysql")));
                    break;

                case "sqlserver":
                    services.AddDbContext<IDataContext, DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("sqlserver")));
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

            // Add AutoMapper
            services.AddAutoMapper(new Assembly[] {typeof(AutoMapperProfile).GetTypeInfo().Assembly});

            services.AddScoped(typeof(IDataContext), typeof(DataContext));
            services.AddScoped(typeof(DataContext), typeof(DataContext));

            services.AddScoped(typeof(IBudgetRepository<Budget>), typeof(BudgetRepository));
            services.AddScoped(typeof(IUserRepository<User>), typeof(UserRepository));

            // Add MediatR
            services.AddMediatR(typeof(AddUserResponse).GetTypeInfo().Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            //services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "raBudget API", Version = "v1" }); });

            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<ForwardedHeadersOptions>(options =>
                                                        {
                                                            options.ForwardedHeaders =
                                                                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                                                        });


            var appSettingsSection = Configuration.GetSection("AppSettings");

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


            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
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