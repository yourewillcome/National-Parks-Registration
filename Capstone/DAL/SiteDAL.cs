using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
    public class SiteDAL
    {
        //Member Variables
        private string _connectionString;

        //Single Parameter Constructor

        /// <summary>
        /// Constructor!
        /// </summary>
        /// <param name="dbConnection">Connection String for SQL DataBase</param>
        public SiteDAL(string dbConnection)
        {
            _connectionString = dbConnection;
        }

        //Methods

        /// <summary>
        /// Create a list with all the parks in the database and their properties.
        /// </summary>
        /// <returns>List of all parks</returns>
        public List<Site> GetSites(Campground campground)
        {
            List<Site> result = new List<Site>();
            int campgroundID = campground.CampgroundId;

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlSite = "SELECT site_id, site.campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities, open_from_mm, open_to_mm, daily_fee " +
                                       "FROM site " +
                                       "JOIN campground ON site.campground_id = campground.campground_id " +
                                       $"WHERE site.campground_id = {campgroundID} " +
                                       "ORDER BY site_number ASC;";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlSite;
                cmd.Connection = conn;

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Site site = new Site();

                    site.SiteId = Convert.ToInt32(reader["site_id"]);
                    site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                    site.SiteNumber = Convert.ToInt32(reader["site_number"]);
                    site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                    site.Accessible = Convert.ToBoolean(reader["accessible"]);
                    site.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
                    site.Utilities = Convert.ToBoolean(reader["utilities"]);
                    //Properties from join
                    site.OpenFromMonth = Convert.ToInt32(reader["open_from_mm"]);
                    site.OpenToMonth = Convert.ToInt32(reader["open_to_mm"]);
                    site.DailyFee = Convert.ToDouble(reader["daily_fee"]);
                    result.Add(site);
                }
            }
            return result;
        }

        public List<Site> GetTopFiveAvailableSites(Campground campground, DateTime startDate, DateTime endDate)
        {
            List<Site> result = new List<Site>();
            int campgroundID = campground.CampgroundId;

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlSite = "SELECT site.site_id, site.campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities, open_from_mm, open_to_mm, daily_fee " +
                                 "FROM site " +
                                 "JOIN campground ON site.campground_id = campground.campground_id " +
                                 "JOIN reservation ON site.site_id = reservation.site_id " +
                                $"WHERE site.campground_id = {campgroundID} " +
                                $"AND NOT((@startDate >= reservation.from_date AND @endDate <= reservation.to_date ) OR " +
                                $"(@startDate >= reservation.from_date AND @startDate <= reservation.to_date) OR " +
                                $"(@endDate >= reservation.from_date AND @endDate <= reservation.to_date) OR " +
                                $"(@endDate <= reservation.from_date AND @startDate >= reservation.to_date) ) " +
                                $"ORDER BY site_number ASC;";


                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlSite;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Site site = new Site();

                    site.SiteId = Convert.ToInt32(reader["site_id"]);
                    site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                    site.SiteNumber = Convert.ToInt32(reader["site_number"]);
                    site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                    site.Accessible = Convert.ToBoolean(reader["accessible"]);
                    site.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
                    site.Utilities = Convert.ToBoolean(reader["utilities"]);
                    //Properties from join
                    site.OpenFromMonth = Convert.ToInt32(reader["open_from_mm"]);
                    site.OpenToMonth = Convert.ToInt32(reader["open_to_mm"]);
                    site.DailyFee = Convert.ToDouble(reader["daily_fee"]);
                    bool siteIdAlreadyAdded = false;
                    foreach(Site item in result)
                    {
                        if(item.SiteId == site.SiteId)
                        {
                            siteIdAlreadyAdded = true;
                        }
                    }
                    if(!siteIdAlreadyAdded)
                    {
                        result.Add(site);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a lits of sites in order of site ID based on advanced search parameters
        /// </summary>
        /// <param name="campground">Campground</param>
        /// <param name="startDate">First day of visit</param>
        /// <param name="endDate">Last day of visit</param>
        /// <param name="numberOfCampers">Number of campers during visit</param>
        /// <param name="needAccessible">true if site needs to be accessible</param>
        /// <param name="rvLength">length in feet of RV, enter 0 if no RV will be used</param>
        /// <param name="needUtilities">true if site needs to have utilities</param>
        /// <returns></returns>
        public List<Site> GetTopFiveAvailableSitesAdvancedSearch(Campground campground, DateTime startDate, DateTime endDate, int numberOfCampers, bool needAccessible, int rvLength, bool needUtilities)
        {
            List<Site> result = new List<Site>();
            int campgroundID = campground.CampgroundId;

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlSite = "SELECT site.site_id, site.campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities, open_from_mm, open_to_mm, daily_fee " +
                                 "FROM site " +
                                 "JOIN campground ON site.campground_id = campground.campground_id " +
                                 "JOIN reservation ON site.site_id = reservation.site_id " +
                                $"WHERE site.campground_id = {campgroundID} " +
                                $"AND NOT((@startDate >= reservation.from_date AND @endDate <= reservation.to_date ) OR " +
                                $"(@startDate >= reservation.from_date AND @startDate <= reservation.to_date) OR " +
                                $"(@endDate >= reservation.from_date AND @endDate <= reservation.to_date) OR " +
                                $"(@endDate <= reservation.from_date AND @startDate >= reservation.to_date) ) " +
                                $"AND max_rv_length >= @rvLength " +
                                $"AND max_occupancy >= @numberOfCampers ";
                                //$"ORDER BY site_number ASC;";
                
                if(needAccessible)
                {
                    sqlSite += "AND accessible = 1 ";
                }
                if(needUtilities)
                {
                    sqlSite += "AND utilities = 1 ";
                }
                sqlSite += "ORDER BY site_number ASC;";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlSite;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.Parameters.AddWithValue("@rvLength", rvLength);
                cmd.Parameters.AddWithValue("@numberOfCampers", numberOfCampers);

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Site site = new Site();

                    site.SiteId = Convert.ToInt32(reader["site_id"]);
                    site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                    site.SiteNumber = Convert.ToInt32(reader["site_number"]);
                    site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
                    site.Accessible = Convert.ToBoolean(reader["accessible"]);
                    site.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
                    site.Utilities = Convert.ToBoolean(reader["utilities"]);
                    //Properties from join
                    site.OpenFromMonth = Convert.ToInt32(reader["open_from_mm"]);
                    site.OpenToMonth = Convert.ToInt32(reader["open_to_mm"]);
                    site.DailyFee = Convert.ToDouble(reader["daily_fee"]);
                    bool siteIdAlreadyAdded = false;
                    foreach (Site item in result)
                    {
                        if (item.SiteId == site.SiteId)
                        {
                            siteIdAlreadyAdded = true;
                        }
                    }
                    if (!siteIdAlreadyAdded)
                    {
                        result.Add(site);
                    }
                }
            }
            return result;
        }

    }
}