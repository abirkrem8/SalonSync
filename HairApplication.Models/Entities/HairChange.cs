using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairApplication.Models.Entities
{
    public class HairChange : EntityBase
    {
        public bool IsHaircut { get; set; } //else, hair color
        public HairDye HairDye { get; set; }
        public int CentimetersCut { get; set; }

    }

    public class HairDye
    {
        public HairDyeBrands Brand { get; set; }
        public string Color { get; set; }
        public int VolumeInmL { get;set; }
    }

    public enum HairDyeBrands
    {
        Revlon,
        Garnier,
        Amika
    }
}
