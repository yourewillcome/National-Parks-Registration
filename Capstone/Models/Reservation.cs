﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Reservation
    {
        //Properties
        public int ReservationId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CampgroundName { get; set; }
        public int NumDays
        { get
            {
                return Convert.ToInt32(ToDate - FromDate);
            }
        }

    }
}
