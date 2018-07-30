using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DAL;
using Capstone.Models;

namespace CapstoneCLI
{
    public class CLI
    {
        const string DatabaseConnection = @"Data Source=.\SQLEXPRESS;Initial Catalog=NationalParkReservationSystem;Integrated Security = True";
                
        public void RunCLI()
        {
            bool programOver = false;

            while (!programOver)
            {
                PrintHeader();
                PrintMenu();

                string input = CLIHelper.GetIntInRangeOrQ("Select a Menu Item for Further Details or Press (Q) to Quit", 1, 1, "Q", true);

                if (input == "1")
                {
                    GetParksCLI();
                }
                else if (input == "Q" || input == "q")
                {
                    programOver = true;
                }
            }
        }

        private void GetParksCLI()
        {
            bool getParksCLIOver = false;
            while (!getParksCLIOver)
            {
                Console.Clear();

                ParkDAL parkDal = new ParkDAL(DatabaseConnection);
                List<Park> parks = parkDal.GetParks();

                string snName = "Park Menu";
                Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
                Console.WriteLine(snName);

                string snNameDash = "---------";
                Console.SetCursorPosition((Console.WindowWidth - snNameDash.Length) / 2, Console.CursorTop);
                Console.WriteLine(snNameDash);
                Console.WriteLine();

                for (int i = 0; i < parks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {parks[i].Name}");
                }

                Console.WriteLine();
                string input = CLIHelper.GetIntInRangeOrQ("Select a Park for Further Details or Press (Q) to Quit", 1, parks.Count, "Q", true);

                if (input == "Q" || input == "q")
                {
                    Console.Clear();
                    getParksCLIOver = true;
                }
                else
                {
                    ParkInfoScreenCLI(parks[Convert.ToInt16(input.ToString()) - 1]);
                }
            }
        }

        private void ParkInfoScreenCLI(Park park)
        {
            Console.Clear();

            string snName = "Park Information Menu";
            Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
            Console.WriteLine(snName);

            string snNameDash = "---------------------";
            Console.SetCursorPosition((Console.WindowWidth - snNameDash.Length) / 2, Console.CursorTop);
            Console.WriteLine(snNameDash);
            Console.WriteLine();

            Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", "NAME", "LOCATION", "ESTABLISHED", "AREA", "ANNUAL VISITORS");
            Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", park.Name, park.Location, park.EstablishDate.ToShortDateString(), park.Area.ToString("N0"), park.Visitors.ToString("N0"));
            Console.WriteLine();
            Console.WriteLine("1) View Campgrounds");
            Console.WriteLine("2) Park Wide Campsite Search");
            Console.WriteLine("3) See All Upcoming Reservations for next 30 Days");

            Console.WriteLine();

            string input = CLIHelper.GetIntInRangeOrQ("Select a Option for Further Details or Press (R) to Return to Previous Menu", 1, 3, "R", true);

            if (input == "1")
            {
                Console.Clear();
                GetAllCampgroundsForParkCLI(park);
            }
            else if(input == "2")
            {
                Console.Clear();
                ParkWideSearchCLI(park);
            }
            else if(input == "3")
            {
                Console.Clear();
                SeeUpcomingReservationsForParkCLI(park);
            }

            Console.WriteLine($"Press any key to return to Park Selection Menu");
        }

        private void GetAllCampgroundsForParkCLI(Park park)
        {
            CampgroundDAL campgroundDAL = new CampgroundDAL(DatabaseConnection);
            ReservationDAL reservationDAL = new ReservationDAL(DatabaseConnection);
            List<Campground> campgrounds = campgroundDAL.GetAllCampgroundsForPark(park);
            bool doneWithMenu = false;
            while (!doneWithMenu)
            {
                Console.Clear();
                //Displaying menu items
                string snName = "Park Campground Menu";
                Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
                Console.WriteLine(snName);

                string snNameDash = "--------------------";
                Console.SetCursorPosition((Console.WindowWidth - snNameDash.Length) / 2, Console.CursorTop);
                Console.WriteLine(snNameDash);
                Console.WriteLine();

                for (int i = 1; i <= campgrounds.Count; i++)
                {
                    if (i == 1)
                    {
                        Console.WriteLine("{0,-5}{1,-35}{2,-10}{3,-10}{4,-10}", " ", "NAME", "OPEN", "CLOSE", "DAILY FEE");
                    }
                    Console.WriteLine("{0,-5}{1,-35}{2,-10}{3,-10}{4,-10}", i + ")", campgrounds[i - 1].Name, campgrounds[i - 1].GetMonthString(true), campgrounds[i - 1].GetMonthString(false), campgrounds[i - 1].DailyFee.ToString("c"));
                }
                Console.WriteLine($"");

                //Selecting a campground
                string userCampground = CLIHelper.GetIntInRangeOrQ("Which campground would you like to make a reservation at? [or press (Q) to quit]", 1, campgrounds.Count(), "q", true);
                Console.WriteLine();

                

                if (userCampground.ToLower() == "q")
                {
                    doneWithMenu = true; 
                }
                else
                {
                    //Get Date and Time
                    Dictionary<bool, DateTime> startDateTrueEndDateFalse = GetStartAndEndDates();
                    DateTime userArrivalDate = startDateTrueEndDateFalse[true];
                    DateTime userDepartureDate = startDateTrueEndDateFalse[false];
                    bool datesGood = false;
                    while (!datesGood)
                    {
                        bool campgroundOpen = true;
                        try
                        {
                            reservationDAL.IsCampgroundOpen(campgrounds[int.Parse(userCampground) - 1], userArrivalDate, userDepartureDate);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            campgroundOpen = false;
                        }
                        if(campgroundOpen)
                        {
                            datesGood = true;
                        }
                    }


                    bool wantAdvancedSearch = CLIHelper.GetBoolCustom("Would you like to check additional site requirements with our advanced search?", "yes", "no");
                    if (wantAdvancedSearch)
                    {
                        AdvancedSearchGetSitesCLI(campgrounds[int.Parse(userCampground) - 1], userArrivalDate, userDepartureDate);
                    }
                    else if (!wantAdvancedSearch)
                    {
                        GetAllSitesForCampgroundCLI(campgrounds[int.Parse(userCampground) - 1], userArrivalDate, userDepartureDate);
                    }
                    doneWithMenu = true;
                }
            }
        }

        private Dictionary<bool, DateTime> GetStartAndEndDates()
        {
            Dictionary<bool, DateTime> startDateTrueEndDateFalse = new Dictionary<bool, DateTime>();
            bool isDone = false;

            while (!isDone)
            {
            Console.WriteLine();
            DateTime userArrivalDate = CLIHelper.GetDateTime("What is your arrival date? mm/dd/yyyy");
            Console.WriteLine();
            DateTime userDepartureDate = CLIHelper.GetDateTime("What is your departure date? mm/dd/yyyy");
            Console.WriteLine();
            ReservationDAL reservationDAL = new ReservationDAL(DatabaseConnection);

            bool hasNoExceptions = true;
            
                Reservation testReservation = new Reservation()
                {
                    Name = "Test",
                    ReservationId = 0,
                    SiteId = 0,
                    FromDate = userArrivalDate,
                    ToDate = userDepartureDate,
                    CreateDate = DateTime.Now
                };
                List<string> exceptionMessages = new List<string>();
                try
                {
                    reservationDAL.IsEndDateAfterStartDate(testReservation);
                }
                catch (Exception e)
                {
                    exceptionMessages.Add(e.Message);
                    hasNoExceptions = false;
                }
                try
                {
                    reservationDAL.IsReservationInTheFuture(testReservation);
                }
                catch (Exception e)
                {
                    exceptionMessages.Add(e.Message);
                    hasNoExceptions = false;
                }
                int counter = 0;
                foreach (string msg in exceptionMessages)
                {
                    counter++;
                    Console.WriteLine(msg);
                    if (counter == exceptionMessages.Count())
                    {
                        Console.WriteLine("Press Any Key To Try Again");
                        Console.ReadKey();
                    }
                }
                if (hasNoExceptions == true)
                {
                    startDateTrueEndDateFalse.Add(true, userArrivalDate);
                    startDateTrueEndDateFalse.Add(false, userDepartureDate);
                    isDone = true;
                }
            }
            return startDateTrueEndDateFalse;

        }

        private void ParkWideSearchCLI(Park park)
        {
            Console.WriteLine("UNDER CONSTRUCTION: Parkwide Search feature coming soon!");

            CampgroundDAL campgroundDAL = new CampgroundDAL(DatabaseConnection);
            SiteDAL siteDAL = new SiteDAL(DatabaseConnection);
            ReservationDAL reservationDAL = new ReservationDAL(DatabaseConnection);

            //Get start and end dates for reservation
            Dictionary<bool, DateTime> startDateTrueEndDateFalse = GetStartAndEndDates();
            DateTime userArrivalDate = startDateTrueEndDateFalse[true];
            DateTime userDepartureDate = startDateTrueEndDateFalse[false];

            //Get all campgrounds for park
            List<Campground> campgrounds = campgroundDAL.GetAllCampgroundsForPark(park);
            Dictionary<int, Campground> campgroundDictionary = new Dictionary<int, Campground>();
            foreach(Campground item in campgrounds)
            {
                campgroundDictionary.Add(item.CampgroundId, item);
            }


            //Shit getting real BELOW!
            bool doneWithMenu = false;
            while (!doneWithMenu)
            {
                Console.Clear();

                string snName = "Available Park Sites Menu";
                Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
                Console.WriteLine(snName);

                string snNameDash = "--------------------------------------";
                Console.SetCursorPosition((Console.WindowWidth - snNameDash.Length) / 2, Console.CursorTop);
                Console.WriteLine(snNameDash);
                Console.WriteLine();
                Console.WriteLine($"Reservation Start Date: {userArrivalDate.ToShortDateString()}");
                Console.WriteLine($"Reservation End Date: {userDepartureDate.ToShortDateString()}");

                Dictionary<int, Site> allSitesInPark = new Dictionary<int, Site>();
                int campgroundDisplayLoopCounter = 0;
                foreach (Campground campground in campgrounds)
                {
                    //Write Header For Each Campground
                    Console.WriteLine();
                    if(campgroundDisplayLoopCounter ==0)
                    {
                        Console.WriteLine("---------------------------------------------------------------------------");
                    }
                    Console.WriteLine(campground.Name.ToUpper() + " CAMPGROUND");

                    //Check if Campground is Closed
                    bool campgroundOpen = true;
                    try
                    {
                        reservationDAL.IsCampgroundOpen(campground, userArrivalDate, userDepartureDate);
                    }
                    catch (Exception e)
                    {
                        campgroundOpen = false;
                        Console.WriteLine(e.Message);
                        Console.WriteLine();

                    }


                    List<Site> sites = siteDAL.GetTopFiveAvailableSites(campground, userArrivalDate, userDepartureDate);
                    if (sites.Count() > 0 && campgroundOpen)
                    {
                        Console.WriteLine("{0,-15}{1,-15}{2,-15}", "OPEN", "CLOSE", "DAILY FEE");
                        Console.WriteLine("{0,-15}{1,-15}{2,-15}", campground.GetMonthString(true), campground.GetMonthString(false), campground.DailyFee);
                        Console.WriteLine();

                        Console.WriteLine($"Reservation Total Fee: " + campground.CostOfStayForSite(userArrivalDate, userDepartureDate).ToString("c"));
                        Console.WriteLine();
                    }

                    if (sites.Count() > 5 && campgroundOpen)
                    {
                        for (int i = 1; i <= 5; i++)
                        {
                            if (i == 1)
                            {
                                Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", "SITE NO.", "MAX OCCUP.", "ACCESSIBLE", "RV LENGTH", "UTILITY");
                            }
                            Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", sites[i - 1].SiteId, sites[i - 1].MaxOccupancy, sites[i - 1].Accessible, sites[i - 1].MaxRVLength, sites[i - 1].Utilities);
                            allSitesInPark.Add(sites[i - 1].SiteId, sites[i - 1]);
                        }
                    }
                    else if (sites.Count() < 6 && sites.Count() > 0 && campgroundOpen )
                    {
                        for (int i = 1; i <= sites.Count(); i++)
                        {
                            if (i == 1)
                            {
                                Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", "SITE NO.", "MAX OCCUP.", "ACCESSIBLE", "RV LENGTH", "UTILITY");
                            }
                            Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", sites[i - 1].SiteId, sites[i - 1].MaxOccupancy, sites[i - 1].Accessible, sites[i - 1].MaxRVLength, sites[i - 1].Utilities);
                            allSitesInPark.Add(sites[i - 1].SiteId, sites[i - 1]);
                        }
                    }
                    else if (sites.Count() == 0 && campgroundOpen )
                    {
                        Console.WriteLine("There are no sites available at your selected campground on your selected dates :(");
                    }
                    Console.WriteLine("---------------------------------------------------------------------------");

                    campgroundDisplayLoopCounter++;

                }

                //Get user input

                List<int> siteIDs = new List<int>();
                foreach(KeyValuePair<int,Site> item in allSitesInPark)
                {
                    siteIDs.Add(item.Key);
                }
                string userSelection = CLIHelper.GetIntInListOrQ("Please enter a site ID to make a reservation or enter R to return to the Park Information Menu", siteIDs, "r", false);
                if (userSelection.ToLower() == "r")
                {
                    doneWithMenu = true;
                }
                else
                {
                    int selectedSiteID = int.Parse(userSelection);
                    Site selectedSite = allSitesInPark[selectedSiteID];
                    Campground selectedCampground = campgroundDictionary[selectedSite.CampgroundId];
                    

                    MakeReservationCLI(selectedCampground, selectedSiteID, userArrivalDate, userDepartureDate);
                    doneWithMenu = true;
                }
            }
        }

        private void SeeUpcomingReservationsForParkCLI(Park park)
        {
            //Title Display
            Console.WriteLine("UNDER CONSTRUCTION: Upcoming Reservation feature coming soon!");
            Console.Clear();

            string snName = "Upcoming Park Reservations Menu";
            Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
            Console.WriteLine(snName);

            string snNameDash = "--------------------------------------";
            Console.SetCursorPosition((Console.WindowWidth - snNameDash.Length) / 2, Console.CursorTop);
            Console.WriteLine(snNameDash);
            Console.WriteLine();

            //Set up DALs and get list of Reservations
            ReservationDAL reservationDAL = new ReservationDAL(DatabaseConnection);
            List<Reservation> upcomingReservations = reservationDAL.GetReservationsForNext30DaysInPark(park);
            CampgroundDAL campgroundDAL = new CampgroundDAL(DatabaseConnection);
            List<Campground> allCampgroundsForPark = campgroundDAL.GetAllCampgroundsForPark(park);
            SiteDAL siteDAL = new SiteDAL(DatabaseConnection);
            List<Site> allSites = new List<Site>();
            

            //Display Reservations
            if(upcomingReservations.Count() == 0)
            {
                Console.WriteLine($"There are no reservations at {park.Name} National Park in the next 30 days.");
            }
            else
            {
                Console.WriteLine($"Reservations at {park.Name} National Park beginning within the next 30 days:");
                Console.WriteLine();
                int upcomingReservationDisplayCounter = 0;
                foreach (Reservation item in upcomingReservations)
                {
                    if (upcomingReservationDisplayCounter == 0)
                    {
                        Console.WriteLine("{0,-35}{1,-20}{2,-10}{3,-15}{4,-10}", "RESERVATION NAME", "CAMPGROUND", "SITE", "START DATE", "END DATE");
                    }
                    Console.WriteLine("{0,-35}{1,-20}{2,-10}{3,-15}{4,-10}", item.Name, item.CampgroundName, item.SiteId.ToString(), item.FromDate.ToShortDateString(), item.ToDate.ToShortDateString());
                    upcomingReservationDisplayCounter++;
                }
            }
            Console.WriteLine();

            Console.WriteLine("Press any key to return to Park Menu");
            Console.ReadKey();
        }

        private void AdvancedSearchGetSitesCLI(Campground campground, DateTime userArrivalDate, DateTime userDepartureDate)
        {
            //int numberOfCampers, bool needAccessible, int rvLength, bool needUtilities
            int numberOfCampers = CLIHelper.GetInteger("Please enter the number of peoples");
            bool needAccessible = CLIHelper.GetBoolCustom("Will you need your site to have handicap access?", "yes", "no");
            bool usingRV = CLIHelper.GetBoolCustom("Will you be using an RV?", "yes", "no");
            int rvLength = 0;
            if (usingRV)
            {
                rvLength = CLIHelper.GetInteger("Please enter the length of your RV (ft)");
            }
            bool needUtilities = CLIHelper.GetBoolCustom("Will you need your site to have utility access?", "yes", "no");

            SiteDAL siteDAL = new SiteDAL(DatabaseConnection);

            List<Site> sites = siteDAL.GetTopFiveAvailableSitesAdvancedSearch(campground, userArrivalDate, userDepartureDate, numberOfCampers, needAccessible, rvLength, needUtilities);
            bool finishedWithMenu = false;

            while (!finishedWithMenu)
            {
                string input = DisplaySitesAndGetUserInput(sites, campground, userArrivalDate, userDepartureDate);

                Console.WriteLine();
                //Using user input
                if (input.ToLower() == "r")
                {
                    finishedWithMenu = true;
                }
                else if (input.ToLower() != "r" && input != "zzz")
                {
                    MakeReservationCLI(campground, int.Parse(input), userArrivalDate, userDepartureDate);
                }
                finishedWithMenu = true;
            }
        }



        private void GetAllSitesForCampgroundCLI(Campground campground, DateTime userArrivalDate, DateTime userDepartureDate)
        {
            SiteDAL siteDAL = new SiteDAL(DatabaseConnection);

            List<Site> sites = siteDAL.GetTopFiveAvailableSites(campground, userArrivalDate, userDepartureDate);
            bool finishedWithMenu = false;

            while (!finishedWithMenu)
            {
                string input = DisplaySitesAndGetUserInput(sites, campground, userArrivalDate, userDepartureDate);

                Console.WriteLine();
                //Using user input
                if (input.ToLower() == "r")
                {
                    finishedWithMenu = true;
                }
                else if (input.ToLower() != "r" && input != "zzz")
                {
                    MakeReservationCLI(campground, int.Parse(input), userArrivalDate, userDepartureDate);
                }
                finishedWithMenu = true;
            }
        }

        public string DisplaySitesAndGetUserInput(List<Site> sites, Campground campground, DateTime userArrivalDate, DateTime userDepartureDate)
        {
            Console.Clear();
            
            string snName = "Search for Campground Reservation Menu";
            Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
            Console.WriteLine(snName);

            string snNameDash = "--------------------------------------";
            Console.SetCursorPosition((Console.WindowWidth - snNameDash.Length) / 2, Console.CursorTop);
            Console.WriteLine(snNameDash);
            Console.WriteLine();

            Console.WriteLine("{0,-25}{1,-15}{2,-15}{3,-15}", "CAMPGROUND", "OPEN", "CLOSE", "DAILY FEE");
            Console.WriteLine("{0,-25}{1,-15}{2,-15}{3,-15}", campground.Name, campground.GetMonthString(true), campground.GetMonthString(false), campground.DailyFee);
            Console.WriteLine();

            Console.WriteLine($"Reservation Start Date: {userArrivalDate.ToShortDateString()}");
            Console.WriteLine($"Reservation End Date: {userDepartureDate.ToShortDateString()}");
            Console.WriteLine($"Reservation Total Fee: " + campground.CostOfStayForSite(userArrivalDate, userDepartureDate).ToString("c"));
            Console.WriteLine();


            string input = "zzz";

            if (sites.Count() > 5)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (i == 1)
                    {
                        Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", "SITE NO.", "MAX OCCUP.", "ACCESSIBLE", "RV LENGTH", "UTILITY");
                    }
                    Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", sites[i - 1].SiteId, sites[i - 1].MaxOccupancy, sites[i - 1].Accessible, sites[i - 1].MaxRVLength, sites[i - 1].Utilities);
                }
                Console.WriteLine();
                input = CLIHelper.GetIntInRangeOrQ("Select a Site Number (+ enter) to Make a Reserveration or Press (R + enter) to Return to Previous Menu", sites[0].SiteId, sites[sites.Count() - 1].SiteId, "R", false);
            }
            else if (sites.Count() < 6 && sites.Count() > 0)
            {
                for (int i = 1; i <= sites.Count(); i++)
                {
                    if (i == 1)
                    {
                        Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", "SITE NO.", "MAX OCCUP.", "ACCESSIBLE", "RV LENGTH", "UTILITY");
                    }
                    Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", sites[i - 1].SiteId, sites[i - 1].MaxOccupancy, sites[i - 1].Accessible, sites[i - 1].MaxRVLength, sites[i - 1].Utilities);
                }
                Console.WriteLine();
                input = CLIHelper.GetIntInRangeOrQ("Select a Site Number (+ enter) to Make a Reserveration or Press (R + enter) to Return to Previous Menu", sites[0].SiteId, sites[sites.Count() - 1].SiteId, "R", false);
            }
            else if (sites.Count() == 0)
            {
                
                Console.WriteLine("Sorry There are no sites available at your selected campground on your selected dates :(");
                Console.WriteLine("Press Any Key to return to the Park Menu");
                Console.ReadKey();
            }
            return input;
        }

        public void DisplayTopFiveSites(List<Site> sites)
        {

        }


        private void MakeReservationCLI(Campground campground, int siteID, DateTime userArrivalDate, DateTime userDepartureDate)
        {

            string reservationName = CLIHelper.GetString("What name should the reservation be made under?");
            Site chosenSite = new Site();
            ReservationDAL reservationDAL = new ReservationDAL(DatabaseConnection);
            Reservation finalReservation = new Reservation()
            {
                SiteId = siteID,
                Name = reservationName,
                FromDate = userArrivalDate,
                ToDate = userDepartureDate,
                CreateDate = DateTime.Now
            };
            bool noExceptions = true;
            try
            {
                reservationDAL.MakeReservation(finalReservation);
            }
            catch (Exception e)
            {
                noExceptions = false;
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Any Key to Return to Park Menu");
                Console.ReadKey();
            }

            if (noExceptions)
            {
                SiteDAL siteDAL2 = new SiteDAL(DatabaseConnection);
                List<Site> allSites = siteDAL2.GetSites(campground);
                Dictionary<int, Site> siteDictionaryForCampground = new Dictionary<int, Site>();
                foreach (Site item in allSites)
                {
                    siteDictionaryForCampground.Add(item.SiteId, item);
                }
                chosenSite = siteDictionaryForCampground[finalReservation.SiteId];
                List<Reservation> allReservations = reservationDAL.GetCurrentReservations(chosenSite);

                Console.WriteLine($"Reservation Booked! Your Confirmation Number is {allReservations[0].ReservationId}");
                Console.WriteLine();
                Console.WriteLine("Press any Key to return to menu");
                Console.ReadKey();
            }
        }

        private void PrintHeader()
        {
            string snName = "--- NATIONAL PARK RESERVATION SYSTEM ---";
            Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
            Console.WriteLine(snName);

            string artLine01 = @"                          __,--'\                  ";
            string artLine02 = @"                    __,--'    :. \.                ";
            string artLine03 = @"               _,--'              \`.              ";
            string artLine04 = @"              /|\       `          \ `.            ";
            string artLine05 = @"             / | \        `:        \  `/          ";
            string artLine06 = @"            / '|  \        `:.       \             ";
            string artLine07 = @"           / , |   \                  \            ";
            string artLine08 = @"          /    |:   \              `:. \           ";
            string artLine09 = @"         /| '  |     \ :.           _,-'`.         ";
            string artLine10 = @"       \' |,  / \   ` \ `:.     _,-'_|    `/       ";
            string artLine11 = @"          '._;   \ .   \   `_,-'_,-'               ";
            string artLine12 = @"        \'    `- .\_   |\,-'_,-'                   ";
            string artLine13 = @"                    `--|_,`'                       ";
            string artLine14 = @"                            `/                     ";

            Console.WriteLine();
            Console.SetCursorPosition((Console.WindowWidth - artLine01.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine01);
            Console.SetCursorPosition((Console.WindowWidth - artLine02.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine02);
            Console.SetCursorPosition((Console.WindowWidth - artLine03.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine03);
            Console.SetCursorPosition((Console.WindowWidth - artLine04.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine04);
            Console.SetCursorPosition((Console.WindowWidth - artLine05.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine05);
            Console.SetCursorPosition((Console.WindowWidth - artLine06.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine06);
            Console.SetCursorPosition((Console.WindowWidth - artLine07.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine07);
            Console.SetCursorPosition((Console.WindowWidth - artLine08.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine08);
            Console.SetCursorPosition((Console.WindowWidth - artLine09.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine09);
            Console.SetCursorPosition((Console.WindowWidth - artLine10.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine10);
            Console.SetCursorPosition((Console.WindowWidth - artLine11.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine11);
            Console.SetCursorPosition((Console.WindowWidth - artLine12.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine12);
            Console.SetCursorPosition((Console.WindowWidth - artLine13.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine13);
            Console.SetCursorPosition((Console.WindowWidth - artLine14.Length) / 2, Console.CursorTop);
            Console.WriteLine(artLine14);
            Console.WriteLine();

            //Console.WriteLine(@"                          __,--'\");
            //Console.WriteLine(@"                    __,--'    :. \.");
            //Console.WriteLine(@"               _,--'              \`.");
            //Console.WriteLine(@"              /|\       `          \ `.");
            //Console.WriteLine(@"             / | \        `:        \  `/");
            //Console.WriteLine(@"            / '|  \        `:.       \");
            //Console.WriteLine(@"           / , |   \                  \");
            //Console.WriteLine(@"          /    |:   \              `:. \");
            //Console.WriteLine(@"         /| '  |     \ :.           _,-'`.");
            //Console.WriteLine(@"       \' |,  / \   ` \ `:.     _,-'_|    `/");
            //Console.WriteLine(@"          '._;   \ .   \   `_,-'_,-'");
            //Console.WriteLine(@"        \'    `- .\_   |\,-'_,-'");
            //Console.WriteLine(@"                    `--|_,`'");
            //Console.WriteLine(@"                            `/");
        }

        private void PrintMenu()
        {
            string snName = "Main Menu";
            Console.SetCursorPosition((Console.WindowWidth - snName.Length) / 2, Console.CursorTop);
            Console.WriteLine(snName);

            string snNameDash = "---------";
            Console.SetCursorPosition((Console.WindowWidth - snNameDash.Length) / 2, Console.CursorTop);
            Console.WriteLine(snNameDash);
            Console.WriteLine();

            Console.WriteLine("Please Select an Item:");
            Console.WriteLine();
            Console.WriteLine("1 - Show all parks");
            Console.WriteLine();
        }

    }
}