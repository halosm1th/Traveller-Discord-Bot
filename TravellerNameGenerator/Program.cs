using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellerNameGenerator
{
    class Program
    {
        private static List<string> firstNames => File.ReadAllLines(Directory.GetCurrentDirectory() + "/firstName.txt").ToList();
        private static List<string> lastNames => File.ReadAllLines(Directory.GetCurrentDirectory() + "/lastName.txt").ToList();
        private static List<string> middleNames => File.ReadAllLines(Directory.GetCurrentDirectory() + "/middleName.txt").ToList();
        private static List<string> placeNames => File.ReadAllLines(Directory.GetCurrentDirectory() + "/placeName.txt").ToList();
        private static readonly int MAX_PLACE_PREFIX = 5;
        
        static void Main(string[] args)
        {
            Random r = new Random();
            Console.WriteLine("Press e to exit, 1 for person name, 2 for place name");

            string strin = "";
            bool placeName = false;
            while ((strin = Console.ReadLine().ToLower()) != "n")
            {
                if (strin == "2") placeName = true;
                else if (strin == "1") placeName = false;

                if (placeName)
                {
                    var name = new StringBuilder();
                    for (int i = 0; i < r.Next(1,MAX_PLACE_PREFIX+1); i++)
                    {
                        name.Append(placeNames[r.Next(0, placeNames.Count)] + " ");
                    }

                    Console.WriteLine($"{name}");
                }
                else
                {
                    var fn = firstNames[r.Next(0, firstNames.Count)];
                    var mn = middleNames[r.Next(0, middleNames.Count)];
                    var ln = lastNames[r.Next(0, lastNames.Count)];
                    Console.WriteLine($"{fn} {mn} {ln}");
                }
            }
        }
    }
}
