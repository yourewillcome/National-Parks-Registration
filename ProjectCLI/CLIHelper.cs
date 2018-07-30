using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneCLI
{
    public class CLIHelper
    {
        public static DateTime GetDateTime(string message)
        {
            string userInput = String.Empty;
            DateTime dateValue = DateTime.MinValue;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid date format. Please try again");
                }
                Console.WriteLine(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!DateTime.TryParse(userInput, out dateValue));

            return dateValue;
        }

        public static int GetInteger(string message)
        {
            string userInput = String.Empty;
            int intValue = 0;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input format. Please try again");
                }

                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!int.TryParse(userInput, out intValue));

            return intValue;

        }


        public static double GetDouble(string message)
        {
            string userInput = String.Empty;
            double doubleValue = 0.0;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input format. Please try again");
                }

                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!double.TryParse(userInput, out doubleValue));

            return doubleValue;

        }

        public static bool GetBool(string message)
        {
            string userInput = String.Empty;
            bool boolValue = false;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input format. Please try again");
                }

                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!bool.TryParse(userInput, out boolValue));

            return boolValue;
        }

        public static string GetString(string message)
        {
            string userInput = String.Empty;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input format. Please try again");
                }
                
                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (String.IsNullOrEmpty(userInput));

            return userInput;
        }

        /// <summary>
        /// Prompt user for an interger within a range or a quit character.  Determine whether you want the input to be a ReadKey or ReadLine.
        /// </summary>
        /// <param name="message">Prompt Message for user</param>
        /// <param name="minValue">Smallest accepted value for user input</param>
        /// 
        /// <param name="maxValue">Largest accepted value for user input</param>
        /// <param name="quitLetter">Character or String that will be accepted</param>
        /// <param name="isReadKey">True for a Console.Readkey (i.e. values are les than 10) False for Console.Readline</param>
        /// <returns>Returns a string that is either the int value as a string or the acceptable string/char</returns>
        public static string GetIntInRangeOrQ(string message, int minValue, int maxValue, string quitLetter, bool isReadKey)
        {
            string result = "";
            bool isDone = false;
            int numberOfAttempts = 0;
            string userInput;
            int userInputInt = 0;
            
            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Invalid input format. Please try again");
                    Console.WriteLine("");
                }

                Console.WriteLine(message + " ");

                if (isReadKey)
                {
                    userInput = Console.ReadKey().KeyChar.ToString();
                }
                else
                {
                    userInput = Console.ReadLine();
                }

                if(userInput.ToLower() == quitLetter.ToLower())
                {
                    result = quitLetter.ToLower();
                    isDone = true;
                }
                else
                {
                    bool isInteger = int.TryParse(userInput, out userInputInt);
                    if(isInteger && userInputInt >= minValue && userInputInt <= maxValue)
                    {
                        result = userInputInt.ToString();
                        isDone = true;
                    }
                }

                numberOfAttempts++;
            }
            while (!isDone);

            return result;
        }

        /// <summary>
        /// Prompt user for an interger included in a list of integers or a quit character.  Determine whether you want the input to be a ReadKey or ReadLine.
        /// </summary>
        /// <param name="message">Prompt Message for user</param>
        /// <param name="ints">List of integers that are allowed to be selected</param>
        /// <param name="quitLetter">Character or String that will be accepted</param>
        /// <param name="isReadKey">True for a Console.Readkey (i.e. values are les than 10) False for Console.Readline</param>
        /// <returns>Returns a string that is either the int value as a string or the acceptable string/char</returns>
        public static string GetIntInListOrQ(string message, List<int> ints, string quitLetter, bool isReadKey)
        {
            string result = "";
            bool isDone = false;
            int numberOfAttempts = 0;
            string userInput;
            int userInputInt = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Invalid input format. Please try again");
                    Console.WriteLine("");
                }

                Console.WriteLine(message + " ");

                if (isReadKey)
                {
                    userInput = Console.ReadKey().KeyChar.ToString();
                }
                else
                {
                    userInput = Console.ReadLine();
                }

                if (userInput.ToLower() == quitLetter.ToLower())
                {
                    result = quitLetter.ToLower();
                    isDone = true;
                }
                else
                {
                    bool isInteger = int.TryParse(userInput, out userInputInt);

                    if (isInteger && ints.Contains(userInputInt))
                    {
                        result = userInputInt.ToString();
                        isDone = true;
                    }
                }

                numberOfAttempts++;
            }
            while (!isDone);

            return result;
        }


        /// <summary>
        /// Get a bool value from a user by having them enter a custom true/false word (i.e. yes/no)
        /// </summary>
        /// <param name="message">User prompt</param>
        /// <param name="trueWord">User input that will return a "true" value</param>
        /// <param name="falseWord">User input that will return a "false" value</param>
        /// <returns></returns>
        public static bool GetBoolCustom(string message, string trueWord, string falseWord)
        {
            string userInput = String.Empty;
            bool boolValue = false;
            int numberOfAttempts = 0;
            bool goodInput = false;

            while (!goodInput)
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input format. Please try again");
                }

                Console.Write($"{message} <{trueWord}/{falseWord}>");
                Console.WriteLine();
                userInput = Console.ReadLine();
                if( userInput == trueWord || userInput == falseWord)
                {
                    goodInput = true;
                }

                numberOfAttempts++;
            }

            if (userInput == trueWord)
            {
                boolValue = true;
            }
                      

            return boolValue;
        }


    }
}
