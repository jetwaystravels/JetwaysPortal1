using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OnionConsumeWebAPI.ApiService;
using OnionConsumeWebAPI.Comman;

using OnionConsumeWebAPI.ErrorHandling;
using System.Security.Claims;
//using OnionConsumeWebAPI.Models.DbSettings;

var builder = WebApplication.CreateBuilder(args);
// Add configuration files
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					 .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Configure MongoDB settings
//builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
//builder.Services.AddSingleton<IMongoClient>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//    if (string.IsNullOrEmpty(settings.ConnectionString))
//        throw new InvalidOperationException("MongoDB connection string is not set.");

//    return new MongoClient(settings.ConnectionString);
//});
//builder.Services.AddScoped(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//    var client = sp.GetRequiredService<IMongoClient>();
//    return client.GetDatabase(settings.DatabaseName);
//});
//builder.Services.AddScoped<MongoDbService>();
builder.Services.AddScoped<CredentialService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.BuildServiceProvider();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//builder.Environment.EnvironmentName = "Production";
builder.Services.AddSession(Option =>
{
	Option.IdleTimeout = TimeSpan.FromMinutes(15);
	Option.Cookie.HttpOnly = true;
	Option.Cookie.IsEssential = true;

});

builder.Services.AddDistributedRedisCache(option =>
{
	option.Configuration = "localhost:6379";
});


//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("CustomClaimPolicy", policy =>
//        policy.RequireClaim(ClaimTypes.NameIdentifier, "CustomClaimValue"));
//});

//builder.Services.AddAuthentication(options =>
//{
//	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

//});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(10);
                options.LoginPath = "/FlightSearchIndex/Index";
                //o.Cookie.Name = options.CookieName;
                //o.Cookie.Domain = options.CookieDomain;
                //o.SlidingExpiration = true;
                //o.ExpireTimeSpan = options.CookieLifetime;
                //o.TicketDataFormat = ticketFormat;
                //o.CookieManager = new CustomChunkingCookieManager();
            });

builder.Services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
////if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//{

//    app.UseExceptionHandler("/Home/Error");
//}
app.UseStaticFiles();
//app.UseMiddleware<ExceptionHandling>();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<RedirectToLogin>();
app.MapControllerRoute(
name: "default",
pattern: "{controller=FlightSearchIndex}/{action=Index}/{id?}");

app.Run();
