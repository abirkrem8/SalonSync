// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SalonSync.Logic.Shared;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.DeleteAppointments;


// Generate fake appointments with exisiting clients and hair stylists at the hair salon for x days in advance. 

var env = "development";
var configuration = new ConfigurationBuilder()
 .AddJsonFile($"appsettings.json")
 .AddJsonFile($"appsettings.{env}.json").Build();

var projectId = configuration.GetValue<string>("FirebaseProjectId");
var firebaseJson = File.ReadAllText(configuration.GetValue<string>("FirebaseCredentials"));


IHost _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
{
    services.AddSingleton(_ => new FirestoreProvider(
    new FirestoreDbBuilder
    {
        ProjectId = projectId,
        JsonCredentials = firebaseJson // <-- service account json file
    }.Build()
));
    services.AddSingleton<IAppointmentDeletionService, AppointmentDeletionService>();
    services.AddTransient<AppointmentScheduleHandler>();

}).Build();



var service = _host.Services.GetRequiredService<IAppointmentDeletionService>();
int exitCode = service.Run(3,true);


Environment.Exit(exitCode);
