
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.SqlServer;
using Google.Cloud.Firestore;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using AutoMapper;
using SalonSync.MVC.Logic;
using SalonSync.Logic.Load.LoadIndexScreen;
using SalonSync.Logic.Load.LoadStylistInformation;
using SalonSync.Logic.Load.LoadAppointmentScheduleForm;
using SalonSync.Logic.Load.LoadClientInformation;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("Program");
logger.LogInformation("Starting SalonSync Application");

logger.LogInformation($"Current Timezone is {System.TimeZone.CurrentTimeZone.StandardName}");

var builder = WebApplication.CreateBuilder(args);

string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
logger.LogInformation($"ASPNETCORE_ENVIRONMENT: {env}");

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory() + "/conf/")
    .AddJsonFile($"appsettings.json")
    .AddJsonFile($"appsettings.{env}.json");

logger.LogInformation("Setting up Firebase DB");
// Set up FireDB
var projectId = builder.Configuration.GetValue<string>("FirebaseProjectId");
string firebaseJson = "";
if (builder.Configuration.GetValue<bool>("FirebaseCredentials:UseEnvironmentVariables"))
{
    logger.LogInformation("Grabbing Firebase Credentials from the Environment Variables");
    string firebaseEnvironmentVariableName = builder.Configuration.GetValue<string>("FirebaseCredentials:FBCredentialsEnvironmentVariable");
    firebaseJson = Environment.GetEnvironmentVariable(firebaseEnvironmentVariableName);
}
else
{
    logger.LogInformation("Grabbing Firebase Credentials from the JSON file");
    firebaseJson = File.ReadAllText(builder.Configuration.GetValue<string>("FirebaseCredentials:FBCredFilePath"));
}
builder.Services.AddSingleton(_ => new FirestoreProvider(
    new FirestoreDbBuilder
    {
        ProjectId = projectId,
        JsonCredentials = firebaseJson // <-- service account json file
    }.Build()
));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
    mc.AddProfile(new AppointmentScheduleMappingProfile());
});

logger.LogInformation("Setting up Automapper");
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddMvc();

logger.LogInformation("Setting up Dependency Injection Handlers");
builder.Services.AddTransient<MappingProfile>();
builder.Services.AddTransient<AppointmentScheduleHandler>();
builder.Services.AddTransient<LoadIndexScreenHandler>();
builder.Services.AddTransient<LoadStylistInformationHandler>();
builder.Services.AddTransient<LoadAppointmentScheduleFormHandler>();
builder.Services.AddTransient<LoadClientInformationHandler>();

logger.LogInformation("Building application...");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Landing}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Information}/{action=Stylist}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Information}/{action=Client}/{id?}");

app.Run();
