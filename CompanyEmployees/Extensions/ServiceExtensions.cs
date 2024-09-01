using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using NLog.Fluent;
using System;
using CompanyEmployees.Presentation.Controllers;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Identity;
using Entities.Models;
using Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) => builder.AddMvcOptions(config => config.OutputFormatters.Add(new
        CsvOutputFormatter()));
        //enable links in our response only if it is explicitly asked for
        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();

                if (systemTextJsonOutputFormatter != null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+json");
                    systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.apiroot+json");
                }

                var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>().FirstOrDefault();
                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.apiroot+xml");
                }
            });

        }

        public static void ConfigureVersioning(this  IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;//adds the API version to the response header.
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                //opt.ApiVersionReader = new QueryStringApiVersionReader("api-version");

                //If we have a lot of versions of a single controller,
                //we can assign these versions in the configuration
                opt.Conventions.Controller<CompaniesController>().HasApiVersion(new ApiVersion(1, 0));
                opt.Conventions.Controller<CompaniesV2Controller>().HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }
        public static void ConfigureResponseCache(this IServiceCollection services)
        {
            services.AddResponseCaching();
            
        }
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddHttpCacheHeaders(expirationoptions =>
            {
                expirationoptions.MaxAge = 150;
                expirationoptions.CacheLocation =CacheLocation.Private; //can't be cached
            },
            validationoptions =>
            {
                validationoptions.MustRevalidate = true;
            });
        }
        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>()
            {
                //three requests are allowed in a five-minute period for any endpoint in our API.
                new RateLimitRule()
                {
                    Endpoint="*",
                    Limit=30,
                    Period="5m"
                }
            };
            //add the created rule.
            services.Configure<IpRateLimitOptions>(opt=>opt.GeneralRules=rateLimitRules);
            services.AddSingleton<IRateLimitCounterStore,MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy,AsyncKeyLockProcessingStrategy>();

        }
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 9;
                o.User.RequireUniqueEmail = true;


            }).AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
        }
        public static void ConfigureJWT(this IServiceCollection services,IConfiguration configuration)
        {
            var jwtConfiguration = new JwtConfiguration();
            //bind to the JwtSettings section directly and map configuration
            //values to respective properties inside the JwtConfiguration class
            configuration.Bind(jwtConfiguration.Section,jwtConfiguration);

            var secretKey = Environment.GetEnvironmentVariable("SECRETKEY");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true, //The receiver of the token is a valid recipient
                    ValidateIssuer = true, //The issuer is the actual server that created the token
                    ValidateLifetime = true,//The token has not expired
                    ValidateIssuerSigningKey = true,//The signing key is valid and is trusted by the server

                    ValidAudience = jwtConfiguration.ValidAudience,
                    ValidIssuer = jwtConfiguration.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
        public static void AddJWTConfiguration(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection("JWTSettings"));
        }

        /*public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("V1", new OpenApiInfo() { Title = "Code Maze API", Version = "V1" });
            });
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("V2", new OpenApiInfo { Title = "Code Maze API", Version = "V2" });
            });
        }*/
    }
}
