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
using WebApi.Enum;
using WebApi.Filters;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"));
            //var mysqlConnection = @"Server=localhost;Database=larabudget;User=root";
            //services.AddDbContext<DataContext>(options => options.UseMySql(mysqlConnection));

            services.AddDbContext<DataContext>(options => options.UseLazyLoadingProxies()
                                                                 .UseSqlServer(Configuration
                                                                                   ["Data:DefaultConnection:ConnectionString"]));

            services.AddMvc(options =>
                            {
                                options.Filters.Add(typeof(ValidateModelStateAttribute));
                            });
            services.AddAutoMapper();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
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
                                                                                       .GetRequiredService<UserService
                                                                                        >();
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
                                      x.RequireHttpsMetadata = false;
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

            // configure DI for application services
            services.AddScoped<UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // global cors policy
            app.UseCors(x => x
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .WithExposedHeaders("Token-Expired")
                            .WithOrigins("http://localhost:8080")
                       );

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}