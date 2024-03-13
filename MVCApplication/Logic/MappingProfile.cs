using AutoMapper;
using HairApplication.Logic.AppointmentSchedule;
using HairApplication.MVC.Models;

namespace HairApplication.MVC.Logic
{
    public class MappingProfile : Profile 
    {
        public MappingProfile()
        {
            CreateMap<AppointmentEntryViewModel, AppointmentScheduleItem>()
                .ForMember(dest => dest.FirstName,  opt => opt.MapFrom(src => src.ClientFirstName))
                .ForMember(dest => dest.LastName,  opt => opt.MapFrom(src => src.ClientLastName))
                .ForMember(dest => dest.PhoneNumber,  opt => opt.MapFrom(src => src.ClientPhoneNumber))
                .ForMember(dest => dest.DateTimeOfApppointment,  opt => opt.MapFrom(src => 
                    new DateTime(src.DateOfAppointment.Year,src.DateOfAppointment.Month, 
                    src.DateOfAppointment.Day, src.TimeOfAppointment.Hour, src.TimeOfAppointment.Minute, 
                    src.TimeOfAppointment.Second)))
                .ForMember(dest => dest.HairStylist,  opt => opt.MapFrom(src => src.SelectedStylist))
                ;
            /*etc...*/
        }
    }
}
