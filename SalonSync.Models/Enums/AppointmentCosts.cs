using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Enums
{
    public static class AppointmentCosts
    {
        private static Dictionary<AppointmentType, int> AppointmentCostList = new Dictionary<AppointmentType, int>()
        {
            {AppointmentType.HairCut, 60},
            {AppointmentType.NewClientConsultation, 40},
            {AppointmentType.HighLight,95},
            {AppointmentType.FullColor, 145},
            {AppointmentType.Extensions, 275}
        };

        public static int GetCost(AppointmentType appointmentType)
        {
            return AppointmentCostList[appointmentType];
        }
    }
}
