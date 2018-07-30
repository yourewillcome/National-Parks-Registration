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
    public class ParkDAL
    {
        //Member Variables
        private string _connectionString;

        //Single Parameter Constructor

        /// <summary>
        /// Constructor!
        /// </summary>
        /// <param name="dbConnection">Connection String for SQL DataBase</param>
        public ParkDAL(string dbConnection)
        {
            _connectionString = dbConnection;
        }

        //Methods

        /// <summary>
        /// Create a list with all the parks in the database and their properties.
        /// </summary>
        /// <returns>List of all parks</returns>
        public List<Park> GetParks()
        {
            List<Park> result = new List<Park>();

            //Connect to Database
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                //Create sql statement
                const string sqlPark = "SELECT park_id, name, location, establish_date, area, visitors, description " +
                                       "FROM park " +
                                       "ORDER BY name ASC;";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sqlPark;
                cmd.Connection = conn;

                //Send command to database
                SqlDataReader reader = cmd.ExecuteReader();

                //Pull data off of result set
                while (reader.Read())
                {
                    Park park = new Park();

                    park.ParkId = Convert.ToInt32(reader["park_id"]);
                    park.Name = Convert.ToString(reader["name"]);
                    park.Location = Convert.ToString(reader["location"]);
                    park.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
                    park.Area = Convert.ToInt32(reader["area"]);
                    park.Visitors = Convert.ToInt32(reader["visitors"]);
                    park.Description = Convert.ToString(reader["description"]);

                    result.Add(park);
                }
            }
                return result;
        }
    }
}
