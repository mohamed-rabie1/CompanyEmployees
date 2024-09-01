using AspNetCoreRateLimit;
using CompanyEmployees;
using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utility;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using Repository;
using Service;
using Service.Contracts;
using Shared.DTOs;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

// Add services to the container.(IService Provider)


builder.Services.AddDbContext<RepositoryContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("X-Pagination");
        /*builder.WithOrigins("https://example.com")
        .WithMethods("Get", "POST")
        .WithHeaders("accept", "content-type");*/ // parameters in url
    });
});

builder.Services.Configure<IISOptions>(options =>
{
    
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; //enable our custom responses from the actions
});


builder.Services.AddSingleton<ILoggerManager,LoggerManager>();
builder.Services.AddScoped<IRepositoryManager,RepositoryManager>();
builder.Services.AddScoped<IServiceManager,ServiceManager>();
builder.Services.AddScoped<IDataShaper<EmployeeDTO>,DataShaper<EmployeeDTO>>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ValidationFilterAttribute>(); //action filter for specific controller or action method
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();
builder.Services.AddControllers(config =>
{
    //config.Filters.Add(new GlobalFilterExample());  global filter
    config.RespectBrowserAcceptHeader=true; // to accept all headers 
    config.ReturnHttpNotAcceptable=true; // if browser ont support type return 406
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter()); //for newtonsoft.Json
    config.CacheProfiles.Add("120SecondsDuration", new CacheProfile() { Duration = 120 });
}).AddXmlDataContractSerializerFormatters() // to support xml
.AddCustomCSVFormatter()
.AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);
//custom media type
builder.Services.AddCustomMediaTypes();
//versioning
builder.Services.ConfigureVersioning();
//Caching
builder.Services.ConfigureResponseCache();
//to enable Etag,last-modified headers
builder.Services.ConfigureHttpCacheHeaders();
//Rate Limitation
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication();
//identity
builder.Services.ConfigureIdentity();
//jwt
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJWTConfiguration(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.ConfigureSwagger();

var app = builder.Build();
// Configure the HTTP request pipeline.(middlewares)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var loggerManager = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(loggerManager);
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseStaticFiles();
app.UseIpRateLimiting();
app.UseCors("CorsPolicy");
app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseHttpsRedirection(); // app.UseRouting(); must be first 
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); // routing for API

app.UseSwagger();
//app.UseSwaggerUI(s =>
//{
//    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
//    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
//});
//Convention-based routing for MVC
//app.UseRouting();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
//support for JSON Patch using Newtonsoft.Json
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
{
    return new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();
}