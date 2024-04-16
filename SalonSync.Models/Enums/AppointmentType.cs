using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Enums
{
    public enum AppointmentType
    {
        [Display(Name = "New Client Consultation")]
        NewClientConsultation,
        [Display(Name = "Hair Cut")]
        HairCut,
        [Display(Name = "Highlight")]
        HighLight,
        [Display(Name = "Full Color")]
        FullColor,
        [Display(Name = "Extensions")]
        Extensions
    }

    
}
