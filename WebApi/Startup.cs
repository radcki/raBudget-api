using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
using raBudget.Core.Handlers.UserHandlers.RegisterUser;
using raBudget.Core.Infrastructure;
using raBudget.Core.Infrastructure.AutoMapper;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Enum;
using raBudget.EfPersistence.Contexts;
using raBudget.EfPersistence.RepositoryImplementations;
using raBudget.WebApi.Handlers;
using raBudget.WebApi.Providers;
using Serilog;
using WebApi.Filters;
using WebApi.Hubs;
using WebApi.Providers;
using ILogger = Serilog.ILogger;
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
        
        #endregion

        #region Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            switch (Configuration["Data:ServerType"])
            {
                case "MySql":
                    services.AddDbContext<IDataContext, DataContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")));
                    break;

                case "SqlServer":
                    services.AddDbContext<IDataContext, DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));
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

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile).GetTypeInfo().Assembly);
            var config = new MapperConfiguration(cfg => {
                                                     cfg.AddProfile(new AutoMapperProfile());
                                                 });
            services.AddSingleton(config);

            // DataContext for DI
            services.AddTransient(typeof(IDataContext), typeof(DataContext));
            services.AddTransient(typeof(DataContext), typeof(DataContext));

            // Repositiories for DI
            services.AddScoped(typeof(IBudgetRepository), typeof(BudgetRepository));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

            // User identity provider for DI
            services.AddScoped<IAuthenticationProvider, AuthenticationProvider>();

            // FluentValidator
            AssemblyScanner.FindValidatorsInAssembly(typeof(BaseResponse).Assembly)
                           .ForEach(result =>
                                    {
                                        services.AddTransient(result.InterfaceType, result.ValidatorType);
                                    }
                                   );

            // Add MediatR
            services.AddMediatR(typeof(CheckInUserHandler).GetTypeInfo().Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            // Swagger
            services.AddSwaggerGen(c =>
                                   {
                                       c.SwaggerDoc("v1", new OpenApiInfo {Title = "raBudget API", Version = "v1"});

                                       var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                                       c.IncludeXmlComments(xmlPath);
                                   });

            // MVC
            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(options =>
                                    {
                                        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                                    }); ;

            // Headers
            services.Configure<ForwardedHeadersOptions>(options =>
                                                        {
                                                            options.ForwardedHeaders =
                                                                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                                                        });
            // JWT authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(ConfigureJwtBearer);

            // Authorization
            services.AddAuthorization(options =>
                                      {
                                          options.AddPolicy("admin",
                                                            policy => policy.RequireClaim(ClaimTypes.Role,
                                                                                          eRole.Admin.ToString()));
                                      }
                                     );
            services.AddScoped<IAuthorizationHandler, UserRegisteredHandler>(); // automatic registration of authenticated

            // Exception handling
            services.AddProblemDetails(ConfigureProblemDetails)
                    .AddMvcCore()
                    .AddJsonFormatters(x => x.NullValueHandling = NullValueHandling.Ignore);
            
            // SignalR
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddScoped<BudgetsNotifier>();
            services.AddScoped<TransactionsNotifier>();
            services.AddSignalR();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSerilogRequestLogging();

            /* FOR NGINX REVERSE PROXY */
            app.UseForwardedHeaders();
            app.UsePathBase("/api");

            /* CORS */
            if (Environment.IsDevelopment())
            {
                app.UseCors(x => x
                                .WithOrigins("http://localhost:8080")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .WithExposedHeaders("Token-Expired")
                           );

                //app.UseDeveloperExceptionPage();
                app.UseProblemDetails(); // Write exceptions into json response
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
            app.UseSwaggerUI(c =>
                             {
                                 c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "raBudget API V1");
                             });

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

        private void ConfigureJwtBearer(JwtBearerOptions options)
        {
            options.Authority = Configuration["Authentication:Authority"];
            options.Audience = Configuration["Authentication:Audience"];

            options.RequireHttpsMetadata = Environment.IsProduction();
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async c =>
                {
                    // Update authentication provider with authentication result
                    var authProvider = c.HttpContext
                                        .RequestServices
                                        .GetRequiredService<IAuthenticationProvider>();

                    authProvider.FromAuthenticationResult(c.Principal);
                },

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
            };

            options.SaveToken = true;
            options.Validate();
        }
        #endregion
    }
}