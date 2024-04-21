// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SalonSync.Logic.Shared;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.GenerateData;
using SalonSync.Logic.AddAppointmentNotes;
using SalonSync.Logic.GetAvailableAppointments;
using CommandLine;
using HairApplication.Logic.CreateNewClient;

class Program
{
     public static void Main(string[] args)
    {
        // Generate fake appointments with exisiting clients and hair stylists at the hair salon for x days in advance. 
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Program");
        logger.LogInformation("Starting SalonSync Data Generator");

        logger.LogInformation($"Current Timezone is {System.TimeZone.CurrentTimeZone.StandardName}");

        string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        logger.LogInformation($"ASPNETCORE_ENVIRONMENT: {env}");

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
            services.AddSingleton<IAppointmentScheduleService, AppointmentScheduleService>();
            services.AddTransient<AppointmentScheduleHandler>();
            services.AddTransient<GetAvailableAppointmentsHandler>();
            services.AddTransient<AddAppointmentNotesHandler>();
            services.AddTransient<CreateNewClientHandler>();

        }).Build();



        var service = _host.Services.GetRequiredService<IAppointmentScheduleService>();

        int exitCode = Parser.Default.ParseArguments<CommandLineOptions>(args)
            .MapResult(async (CommandLineOptions opts) =>
            {
                try
                {
                    // We have the parsed arguments, so let's just pass them down
                    return service.Run(opts);
                }
                catch
                {
                    Console.WriteLine("Error!");
                    return -3; // Unhandled error
                }
            },
            errs => Task.FromResult(-1)).Result; // Invalid arguments



        Environment.Exit(exitCode);

    }
}