using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Site
    {
        //Properties
        public int SiteId { get; set; }
        public int CampgroundId { get; set; }
        public int SiteNumber { get; set; }
        public int MaxOccupancy { get; set; }
        public bool Accessible { get; set; }
        public int MaxRVLength { get; set; }
        public bool Utilities { get; set; }

        public int OpenFromMonth { get; set; }
        public int OpenToMonth { get; set; }
        public double DailyFee { get; set; }


    }
}
