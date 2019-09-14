using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using raBudget.Core.Dto.Base;
using raBudget.Core.Dto.User;
using raBudget.Core.Dto.User.Response;
using raBudget.Core.Infrastructure;
using raBudget.Core.Infrastructure.AutoMapper;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using raBudget.EfPersistence.Contexts;
using raBudget.EfPersistence.RepositoryImplementations;
using raBudget.WebApi.Providers;
using WebApi.Filters;
using WebApi.Hubs;
using WebApi.Providers;
using WebApi.Services;
using ValidationProblemDetails = raBudget.WebApi.Models.ValidationProblemDetails;
using ValidationException = raBudget.Core.Exceptions.ValidationException;

namespace WebApi
{
    public class Startup
    {
        #region Constructors

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        #endregion

        #region Properties

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

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

        #endregion

        #region Methods

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
            services.AddAutoMapper(typeof(AutoMapperProfile).GetTypeInfo().Assembly);

            // DataContext for DI
            services.AddScoped(typeof(IDataContext), typeof(DataContext));
            services.AddScoped(typeof(DataContext), typeof(DataContext));

            // Repositiories for DI
            services.AddScoped(typeof(IBudgetRepository<Budget>), typeof(BudgetRepository));
            services.AddScoped(typeof(IUserRepository<User>), typeof(UserRepository));

            // User identity provider for DI
            services.AddScoped<IAuthenticationProvider, AuthenticationProvider>();

            AssemblyScanner.FindValidatorsInAssembly(typeof(BaseResponse).Assembly)
                           .ForEach(result =>
                                    {
                                        //services.AddScoped(result.InterfaceType, result.ValidatorType);
                                        services.AddTransient(result.InterfaceType, result.ValidatorType);
                                    }
                                   );

            // Add MediatR
            services.AddMediatR(typeof(AddUserResponse).GetTypeInfo().Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "raBudget API", Version = "v1"}); });

            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<ForwardedHeadersOptions>(options =>
                                                        {
                                                            options.ForwardedHeaders =
                                                                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                                                        });
            // jwt authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                                  {
                                      options.Authority = Configuration["Authentication:Authority"];
                                      options.Audience = Configuration["Authentication:Audience"];
                                      if (IsDebug)
                                      {
                                          options.RequireHttpsMetadata = false;
                                          options.Events = new JwtBearerEvents
                                                           {
                                                               OnTokenValidated = async c =>
                                                                                  {
                                                                                      // Update authentication provider
                                                                                      var authProvider = c.HttpContext
                                                                                                          .RequestServices
                                                                                                          .GetRequiredService<IAuthenticationProvider>();

                                                                                      authProvider.FromAuthenticationResult(c.Principal);
                                                                                  },
#if DEBUG
                                                               OnAuthenticationFailed = c =>
                                                                                        {
                                                                                            ProblemDetails problem;
                                                                                            if (c.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                                                                            {
                                                                                                problem = new ProblemDetails()
                                                                                                              {
                                                                                                                  Title = "Token has expired",
                                                                                                                  Status = StatusCodes.Status401Unauthorized,
                                                                                                                  Detail = Environment.IsDevelopment() 
                                                                                                                               ? c.Exception.Message 
                                                                                                                               : null
                                                                                                              };
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                problem = new ExceptionProblemDetails(c.Exception);
                                                                                            }

                                                                                            c.NoResult();
                                                                                            c.Response.StatusCode = 401;
                                                                                            c.Response.ContentType = "application/json";
                                                                                            return c.Response.WriteAsync(JsonConvert.SerializeObject(problem));
                                                                                        }
#endif
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
            
            services.AddProblemDetails(ConfigureProblemDetails).AddMvcCore().AddJsonFormatters(x => x.NullValueHandling = NullValueHandling.Ignore);



            // DEPENDENCY INJECTION
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

                //app.UseDeveloperExceptionPage();
                app.UseProblemDetails();
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
        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            options.IncludeExceptionDetails = ctx => Environment.IsDevelopment();

            options.Map<NotImplementedException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status501NotImplemented));
            options.Map<HttpRequestException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status503ServiceUnavailable));
            options.Map<ValidationException>(ex => new ValidationProblemDetails(ex));

            options.Map<Exception>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status500InternalServerError));
        }

        #endregion
    }
}