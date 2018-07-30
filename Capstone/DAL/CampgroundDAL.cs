using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;

using Capstone.Models;
using System.Data.SqlClient;

namespace Capstone.DAL
{
    public class CampgroundDAL
    {
        //Member Variables
        private string _connectionString;

        //Single Parameter Constructor

        /// <summary>
        /// CampgroundDAL Constructor!
        /// </summary>
        /// <param name="dbConnection">Connection String for SQL DataBase</param>
        public CampgroundDAL(string dbConnection)
        {
            _connectionString = dbConnection;
        }

        //Methods
        public List<Campground> GetAllCampgroundsForPark(Park park)
        {
            List<Campground> result = new List<Campground>();
            int parkID = park.ParkId;

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                string sqlCampground = "SELECT campground_id, park_id, name, open_from_mm, open_to_mm, daily_fee " +
                                       "FROM campground " +
                                       $"WHERE park_id = {parkID.ToString()} " +
                                       "ORDER BY name ASC;";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlCampground;
                cmd.Connection = conn;

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Campground campground = new Campground();

                    campground.CampgroundId = Convert.ToInt32(reader["campground_id"]);
                    campground.ParkId = Convert.ToInt32(reader["park_id"]);
                    campground.Name = Convert.ToString(reader["name"]);
                    campground.OpenFromMonth = Convert.ToInt32(reader["open_from_mm"]);
                    campground.OpenToMonth = Convert.ToInt32(reader["open_to_mm"]);
                    campground.DailyFee = Convert.ToInt32(reader["daily_fee"]);

                    result.Add(campground);
                }
            }
            return result;
        }
                
    }
}
