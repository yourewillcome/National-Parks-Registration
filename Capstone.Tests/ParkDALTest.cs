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
    public class ParkDALTest
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
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _tran.Dispose();
        }

        [TestMethod]
        public void GetParks()
        {
            //Arrange 
            ParkDAL parkDAL = new ParkDAL(_connectionString);

            //Act
            List<Park> parks = parkDAL.GetParks();

            //Assert
            Assert.AreEqual(4, parks.Count);
            Assert.AreEqual(_parkId, parks[3].ParkId);
        }
    }
}
