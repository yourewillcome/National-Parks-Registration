using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using Capstone.DAL;
using Capstone.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capstone.Tests
{
    [TestClass]
    public class CampgroundDALTest
    {
        //SQL Member Variables
        private TransactionScope _tran;
        private string _connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=NationalParkReservationSystem;Integrated Security = True";

        Campground campground = new Campground();
        Park park = new Park();

        //Test Park Member Variables
        private int _parkId;
        private const int TestParkID = 4;
        private const string TestParkName = "Test Park";
        private const string TestParkLocation = "Ohio";
        private DateTime TestParkEstablishDate = DateTime.Today;
        private const int TestParkArea = 5500;
        private const int TestParkVisitors = 250;
        private const string TestParkDescription = "Wonderful place to test!";

        //Test Campground Member Variables
        private int _campgroundId;
        private const string TestCampgroundName = "Test Campground";
        private const int TestCampgroundOpenFrom = 1;
        private const int TestCampgroundOpenTo = 12;
        private const double TestCampgroundDailyFee = 250.00;

        [TestInitialize]
        public void Initialize()
        {
            _tran = new TransactionScope();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd;
                conn.Open();

                cmd = new SqlCommand("INSERT INTO park (name, location, establish_date, area, visitors, description) " +
                    $"VALUES ('{TestParkName}', '{TestParkLocation}', '{TestParkEstablishDate}', '{TestParkArea}', '{TestParkVisitors}', '{TestParkDescription}'); " +
                     "SELECT CAST(SCOPE_IDENTITY() as int);", conn);
                _parkId = (int)cmd.ExecuteScalar();

                park.ParkId = _parkId;

                cmd = new SqlCommand("INSERT INTO campground (park_id, name, open_from_mm, open_to_mm, daily_fee) " +
                                    $"VALUES ('{_parkId}', '{TestCampgroundName}', '{TestCampgroundOpenFrom}', '{TestCampgroundOpenTo}', '{TestCampgroundDailyFee}'); " +
                                     "SELECT CAST(SCOPE_IDENTITY() as int);", conn);
                _campgroundId = (int)cmd.ExecuteScalar();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _tran.Dispose();
        }

        [TestMethod]
        public void GetAllCampgroundsForPark()
        {
            //Arrange 
            CampgroundDAL campgroundDAL = new CampgroundDAL(_connectionString);

            //Act
            List<Campground> campgrounds = campgroundDAL.GetAllCampgroundsForPark(park);

            //Assert
            Assert.AreEqual(1, campgrounds.Count);
            Assert.AreEqual(_campgroundId, campgrounds[0].CampgroundId);
        }
    }
}