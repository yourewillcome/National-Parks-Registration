using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ReservationDAL
    {
        //Member Variables
        private string _connectionString;

        //Single Parameter Constructor

        /// <summary>
        /// Constructor!
        /// </summary>
        /// <param name="dbConnection">Connection String for SQL DataBase</param>
        public ReservationDAL(string dbConnection)
        {
            _connectionString = dbConnection;
        }

        public List<Reservation> GetCurrentReservations(Site site)
        {
            List<Reservation> result = new List<Reservation>();
            int siteID = site.SiteId;

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlReservation = "SELECT reservation_id, site_id, name, from_date, to_date, create_date " +
                                        "FROM reservation " +
                                       $"WHERE site_id = {siteID} " +
                                        "ORDER BY reservation_id DESC;";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlReservation;
                cmd.Connection = conn;

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Reservation reservation = new Reservation();

                    reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
                    reservation.SiteId = Convert.ToInt32(reader["site_id"]);
                    reservation.Name = Convert.ToString(reader["name"]);
                    reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
                    reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
                    reservation.CreateDate = Convert.ToDateTime(reader["create_date"]);

                    result.Add(reservation);
                }
            }
            return result;
        }

        public void CheckReservation(Reservation newReservation)
        {
            //bool isAvailable = true;
            List<Reservation> existingReservations = new List<Reservation>();

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlReservation = "SELECT reservation_id, site_id, name, from_date, to_date, create_date " +
                                        "FROM reservation " +
                                       $"WHERE site_id = {newReservation.SiteId} " +
                                        "ORDER BY from_date DESC;";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlReservation;
                cmd.Connection = conn;

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Reservation existingReservation = new Reservation();

                    existingReservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
                    existingReservation.SiteId = Convert.ToInt32(reader["site_id"]);
                    existingReservation.Name = Convert.ToString(reader["name"]);
                    existingReservation.FromDate = Convert.ToDateTime(reader["from_date"]);
                    existingReservation.ToDate = Convert.ToDateTime(reader["to_date"]);
                    existingReservation.CreateDate = Convert.ToDateTime(reader["create_date"]);

                    existingReservations.Add(existingReservation);
                }
            }

            foreach (Reservation item in existingReservations)
            {
                if (newReservation.FromDate >= item.FromDate && newReservation.ToDate <= item.ToDate ||
                    newReservation.FromDate >= item.FromDate && newReservation.FromDate <= item.ToDate ||
                    newReservation.ToDate >= item.FromDate && newReservation.ToDate <= item.ToDate ||
                    newReservation.FromDate <= item.FromDate && newReservation.ToDate >= item.ToDate)
                {
                    //isAvailable = false;
                    throw new Exception("Sorry, that date has been booked! Please Try a different date.");
                }
            }

            //return isAvailable;
        }
    
        public void IsCampgroundOpen(Campground campground, DateTime userArrivalDate, DateTime userDepartureDate)
        {
            //bool result = true;
            if(userArrivalDate.Month < campground.OpenFromMonth || userArrivalDate.Month > campground.OpenToMonth)
            {
                throw new Exception("Campground is closed during start or end date.");
            }
            if (userDepartureDate.Month < campground.OpenFromMonth || userDepartureDate.Month > campground.OpenToMonth)
            {
                throw new Exception("Campground is closed during start or end date.");
            }

            //return result;
        }

        public void IsReservationInTheFuture(Reservation reservation)
        {
            DateTime nowTime = DateTime.Now;
            if (reservation.FromDate < nowTime)
            {
                throw new Exception("Start date is the past. Stop living in the past!");
            }
            if (reservation.ToDate < nowTime)
            {
                throw new Exception("End date is the past. Stop living in the past!");
            }
        }

        public void IsEndDateAfterStartDate(Reservation reservation)
        {
            if(reservation.FromDate > reservation.ToDate)
            {
                throw new Exception("End date is before start date.  Get out of here Time Wizard!");
            }
        }

        public void MakeReservation(Reservation reservation)
        {
            //bool wasSuccessful = true;

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlReservation = "INSERT INTO reservation (site_id, name, from_date, to_date, create_date)" +
                                        $"VALUES(@site_id, @name, @from_date, @to_date, @create_date)";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlReservation;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@site_id", reservation.SiteId);
                cmd.Parameters.AddWithValue("@name", reservation.Name);
                cmd.Parameters.AddWithValue("@from_date", reservation.FromDate);
                cmd.Parameters.AddWithValue("@to_date", reservation.ToDate);
                cmd.Parameters.AddWithValue("@create_date", reservation.CreateDate);

                //Send command to database
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    //wasSuccessful = false;
                    throw new Exception("DATABASE ERROR: Reservation could not be made.");
                }
            }
            //return wasSuccessful;
        }

        public List<Reservation> GetReservationsForNext30DaysInPark(Park park)
        {
            List<Reservation> result = new List<Reservation>();
            DateTime currentDate = DateTime.Now;
            TimeSpan thirtyDays = new TimeSpan(30, 0, 0, 0);
            DateTime thirtyDaysFromNow = currentDate + thirtyDays;

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlReservation = "SELECT reservation_id, reservation.site_id, reservation.name, from_date, to_date, create_date, campground.name AS campgroundName " +
                                        "FROM reservation " +
                                        "JOIN site ON reservation.site_id = site.site_id " +
                                        "JOIN campground ON site.campground_id = campground.campground_id " +
                                        "JOIN park ON park.park_id = campground.park_id " +
                                       $"WHERE park.park_id = @park_id " +
                                       $"AND (@currentDate <= reservation.from_date AND @thirtyDate >= reservation.from_date ) " +
                                        "ORDER BY from_date ASC;";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlReservation;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@park_id", park.ParkId);
                cmd.Parameters.AddWithValue("@currentDate", currentDate);
                cmd.Parameters.AddWithValue("@thirtyDate", thirtyDaysFromNow);

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Reservation reservation = new Reservation();

                    reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
                    reservation.SiteId = Convert.ToInt32(reader["site_id"]);
                    reservation.Name = Convert.ToString(reader["name"]);
                    reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
                    reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
                    reservation.CreateDate = Convert.ToDateTime(reader["create_date"]);
                    reservation.CampgroundName = Convert.ToString(reader["campgroundName"]);

                    result.Add(reservation);
                }
            }


            return result;
        }
    }
}