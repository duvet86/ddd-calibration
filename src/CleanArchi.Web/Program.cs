using System.Net.Mime;
using System.Text;
using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CleanArchi.Core;
using CleanArchi.Core.Interfaces;
using CleanArchi.Infrastructure;
using CleanArchi.Infrastructure.Data;
using CleanArchi.Infrastructure.Identity;
using CleanArchi.Web.HealthChecks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NuGet.Protocol;
using IdentityConstants = CleanArchi.Infrastructure.Identity.IdentityConstants;

var builder = WebApplication.CreateBuilder(args);

// Start Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Start App DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity DbContext
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

var signingKey = Encoding.ASCII.GetBytes(IdentityConstants.JWT_SECRET_KEY);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = redirectContext =>
            {
                redirectContext.HttpContext.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.AllowedForNewUsers = false;
});

builder.Services.AddScoped<ITokenClaimsService, IdentityTokenClaimService>();

// Add memory cache services
builder.Services.AddMemoryCache();

//builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();
builder.Services
    .AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api_health_check", tags: new[] { "apiHealthCheck" })
    .AddCheck<HomePageHealthCheck>("home_page_health_check", tags: new[] { "homePageHealthCheck" });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
});

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
    config.Services = new List<ServiceDescriptor>(builder.Services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
});

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultCoreModule());
    containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure

var app = builder.Build();

app.UseHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            var result = new
            {
                status = report.Status.ToString(),
                errors = report.Entries.Select(e => new
                {
                    key = e.Key,
                    value = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                })
            }.ToJson();
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }
    });

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseShowAllServicesMiddleware();
}
else
{
    //app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseRouting();

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
    endpoints.MapHealthChecks("home_page_health_check", new HealthCheckOptions { Predicate = check => check.Tags.Contains("homePageHealthCheck") });
    endpoints.MapHealthChecks("api_health_check", new HealthCheckOptions { Predicate = check => check.Tags.Contains("apiHealthCheck") });
});

app.Logger.LogInformation("Seeding Database...");

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var appContext = services.GetRequiredService<AppDbContext>();
        appContext.Database.EnsureCreated();

        var identityContext = services.GetRequiredService<IdentityDbContext>();
        identityContext.Database.EnsureCreated();

        //SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the DB.");

        throw;
    }
}

app.Logger.LogInformation("Starting app...");
app.Run();
