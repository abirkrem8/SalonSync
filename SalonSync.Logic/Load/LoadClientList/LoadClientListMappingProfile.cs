using AutoMapper;
using SalonSync.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Logic.Load.LoadClientList
{
    public class LoadClientListMappingProfile : Profile
    {
        public LoadClientListMappingProfile()
        {
            //CreateMap<Question, QuestionModel>();
            /*etc...*/

            CreateMap<Client, LoadClientListResultItem>();
        }

    }
}
