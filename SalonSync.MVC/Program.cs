
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.SqlServer;
using Google.Cloud.Firestore;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Shared;
using AutoMapper;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.MVC.Logic;
using SalonSync.Logic.AppointmentConfirmation;
using SalonSync.Logic.Load.LoadIndexScreen;
using SalonSync.Logic.Load.LoadStylistInformation;
using HairApplication.Logic.LoadAppointmentScheduleForm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Set up FireDB
var projectId = builder.Configuration.GetValue<string>("FirebaseProjectId");
var firebaseJson = File.ReadAllText(builder.Configuration.GetValue<string>("FirebaseCredentials"));
builder.Services.AddSingleton(_ => new FirestoreProvider(
    new FirestoreDbBuilder
    {
        ProjectId = projectId,
        JsonCredentials = firebaseJson // <-- service account json file
    }.Build()
));

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
    mc.AddProfile(new AppointmentScheduleMappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddMvc();

builder.Services.AddTransient<MappingProfile>();
builder.Services.AddTransient<AppointmentScheduleHandler>();
builder.Services.AddTransient<AppointmentConfirmationHandler>();
builder.Services.AddTransient<LoadIndexScreenHandler>();
builder.Services.AddTransient<LoadStylistInformationHandler>();
builder.Services.AddTransient<LoadAppointmentScheduleFormHandler>();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Information}/{action=Stylist}/{id?}");

app.Run();
