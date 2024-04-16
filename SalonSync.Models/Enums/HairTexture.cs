using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalonSync.Models.Enums
{
    public enum HairTexture
    {
        [Display(Name = "Unknown")]
        Unknown,
        [Display(Name = "Straight")]
        Straight,
        [Display(Name = "Wavy")]
        Wavy,
        [Display(Name = "Curly")]
        Curly
    }
}
