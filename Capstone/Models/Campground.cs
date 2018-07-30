using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Campground
    {
        //Member Variables
        private readonly Dictionary<int, string> _months = new Dictionary<int, string>()
        {
            {1, "January"},
            {2, "February"},
            {3, "March"},
            {4, "April"},
            {5, "May"},
            {6, "June"},
            {7, "July"},
            {8, "August"},
            {9, "September"},
            {10, "October"},
            {11, "November"},
            {12, "December"},
        };

        //properties
        public int CampgroundId { get; set; }
        public int ParkId { get; set; }
        public string Name { get; set; }
        public int OpenFromMonth { get; set; }
        public int OpenToMonth { get; set; }
        public double DailyFee { get; set; }


        //Methods

        public string GetMonthString(bool isStartingMonth)
        {
            string result = "";
            int month = 0;
            if(isStartingMonth)
            {
                month = OpenFromMonth;
            }
            else
            {
                month = OpenToMonth;
            }
            result = _months[month];
            return result;
        }

        public double CostOfStayForSite(DateTime startDate, DateTime endDate)
        {
            TimeSpan numberOfDays = endDate-startDate;
            double numberOfDaysDub = numberOfDays.TotalDays;
            double result = numberOfDaysDub * DailyFee;
            return result;
        }

    }
}
