using AutoMapper;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadClientInformation
{
    public class LoadClientInformationMappingProfile : Profile
    {
        public LoadClientInformationMappingProfile()
        {
            //CreateMap<Question, QuestionModel>();
            /*etc...*/
            CreateMap<Appointment, LoadClientInformationResultAppointment>()
                .ForMember(dst => dst.AppointmentStartTime, x => x.MapFrom(src => src.StartTimeOfAppointment.ToDateTime().ToLocalTime()))
                ;
        }

    }
}
