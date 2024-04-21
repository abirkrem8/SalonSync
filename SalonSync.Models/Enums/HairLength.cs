using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Enums
{
    public enum HairLength
    {
        [Display(Name = "Unknown")]
        Unknown,
        [Display(Name = "Short")]
        Short,
        [Display(Name = "Medium")]
        Medium,
        [Display(Name = "Long")]
        Long
    }
}
