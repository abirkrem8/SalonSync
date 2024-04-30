using AutoMapper;
using SalonSync.Logic.AddAppointmentNotes;
using SalonSync.Logic.AppointmentSchedule;
using SalonSync.Logic.Load.LoadAppointmentScheduleForm;
using SalonSync.Logic.Load.LoadClientInformation;
using SalonSync.Logic.Load.LoadClientList;
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
            #region HomeController
            CreateMap<LoadIndexScreenResult, IndexViewModel>();

            CreateMap<LoadClientListResult, ClientListViewModel>();
            CreateMap<LoadClientListResultItem, ClientListViewModelItem>();
            #endregion


            #region InformationController
            CreateMap<LoadStylistInformationResult, StylistDetailViewModel>();
            CreateMap<LoadStylistInformationResultAppointment, StylistDetailViewModelAppointment>();


            CreateMap<LoadClientInformationResultAppointment, ClientInformationViewModelAppointment>()
                .ForMember(dest => dest.AppointmentNotes, opt => opt.MapFrom(src => src.AppointmentNotes));
            CreateMap<LoadClientInformationResult, ClientInformationViewModel>();


            CreateMap<AddAppointmentNoteModel, AddAppointmentNotesItem>();
            #endregion


            #region AppointmentController
            CreateMap<LoadAppointmentScheduleFormResult, AppointmentScheduleViewModel>();

            CreateMap<AppointmentScheduleViewModel, AppointmentScheduleItem>()
                .ForMember(dest => dest.HairStylistId, opt => opt.MapFrom(src => src.SelectedStylist.Split('|', StringSplitOptions.None)[0]))
                .ForMember(dest => dest.TimeOfAppointment, opt => opt.MapFrom(src => DateTime.Parse(src.TimeOfAppointment)))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ClientFirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ClientLastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ClientPhoneNumber))
                ;
            #endregion
        }
    }
}
