using AutoMapper;
using HairApplication.Logic.LoadAppointmentScheduleForm;
using SalonSync.Logic.AppointmentConfirmation;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Load.LoadIndexScreen;
using SalonSync.Logic.Load.LoadStylistInformation;
using SalonSync.MVC.Models;
using System.Globalization;
using System.Text.Json;

namespace SalonSync.MVC.Logic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoadIndexScreenResult, IndexViewModel>()
               ;

            // After new appointment form is filled out, we need to verify the client in the DB
            // and display it to the user on a confirmation screen
            CreateMap<AppointmentEntryViewModel, AppointmentConfirmationItem>()
                ;

            // Taking the data and displaying it to the user
            CreateMap<AppointmentConfirmationResult, AppointmentConfirmationViewModel>()
                .ForMember(dest => dest.DateTimeOfAppointment, opt => opt.MapFrom(src => src.DateTimeOfAppointment.ToString("MM/dd/yyyy hh:mm tt")))

               ;

            // After receiving confirmation that they want to schedule an appointment,
            // Put the appointment in the DB
            CreateMap<AppointmentConfirmationViewModel, AppointmentScheduleItem>()
                .ForMember(dest => dest.HairStylist, opt => opt.MapFrom(src => src.HairStylistId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ClientFirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ClientLastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ClientPhoneNumber))
                .ForMember(dest => dest.IsNewClient, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ClientId)))
                .ForMember(dest => dest.DateTimeOfApppointment, opt => opt.MapFrom(src => DateTime.ParseExact(src.DateTimeOfAppointment, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture)))
                ;

            CreateMap<LoadStylistInformationResult, StylistDetailViewModel>();
            CreateMap<LoadStylistInformationResultAppointment, StylistDetailViewModelAppointment>();
            /*etc...*/

            CreateMap<LoadAppointmentScheduleFormResult, AppointmentEntryViewModel>();
        }
    }
}
