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
    public class SiteDALTest
    {
        //SQL Member Variables
        private TransactionScope _tran;
        private string _connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=NationalParkReservationSystem;Integrated Security = True";

        Campground campground = new Campground();
        Park park = new Park();
        Site site = new Site();

        //Test Site Member Variables
        private int _siteId;
        private const int TestSiteID = 4;
        private const int TestCampgroundId = 3;
        private const int TestSiteNo = 48;
        private const int MaxOccup = 6;
        private const bool IsAccessible = true;
        private const int MaxRVLength = 40;
        private const bool HasUtilities = true;

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
                campground.CampgroundId = _campgroundId;

                cmd = new SqlCommand("INSERT INTO site (campground_id, site_number, max_occupancy, accessible, max_rv_length, utilities) " +
                                    $"VALUES ('{_campgroundId}', '{TestSiteNo}', '{MaxOccup}', '{IsAccessible}', '{MaxRVLength}', '{HasUtilities}'); " +
                                     "SELECT CAST(SCOPE_IDENTITY() as int);", conn);

                _siteId = (int)cmd.ExecuteScalar();
                site.SiteId = _siteId;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _tran.Dispose();
        }

        [TestMethod]
        public void GetSites()
        {
            //Arrange 
            SiteDAL siteDAL = new SiteDAL(_connectionString);

            //Act
            List<Site> sites = siteDAL.GetSites(campground);

            //Assert
            Assert.AreEqual(1, sites.Count);
            Assert.AreEqual(_siteId, sites[0].SiteId);
        }
    }
}
