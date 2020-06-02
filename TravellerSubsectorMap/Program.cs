using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Traveller_subsector_generator;

namespace TravellerSubsectorMap
{
    class Program
    {
        //System = 1 parsec
        //Subsector = 8 * 10 parsecs
        //Sector 4 * 4 Subsectors
        //Supersector 6 * 6 sectors
        //MegaSector 8 * 8 supsersectors
        //Quadrant 3 * 3 Megasectors
        //Galaxy 2 * 2 Quadrants (4)
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (args.Length < 1 && false)
            {
                Console.WriteLine("Begining Generation of Galaxy");
                var galaxy = new Galaxy(Subsector.GenerateName());

                Console.WriteLine("Creating directory");
                var path = Directory.GetCurrentDirectory() + $"/{galaxy.Name}_Galaxy";
                Directory.CreateDirectory(path);

                Console.WriteLine($"writing {galaxy.Name} to {path}");
                galaxy.GenerateAndWriteGalaxy(path);
                Console.Clear();
                Console.WriteLine("Done.");
                Console.Title = "Done";
                Console.ReadKey();
                Console.ReadLine();
                Console.ReadKey();
            }
            else
            {
                JustASubPlease();
            }
        }

        private static void JustASubPlease()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter path to Sector:");
                    var path = "Bremerton"; // Console.ReadLine();
                    var text = File.ReadAllLines(Directory.GetCurrentDirectory() + $"/{path}.txt");
                    var sector = new Sector(text, path);

                    path = Directory.GetCurrentDirectory() + $"/{sector.Name}/{sector.Name}";
                    Directory.CreateDirectory(path);
                    sector.WriteSector(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


                Console.WriteLine("Stopping bruv?");
                if (Console.ReadKey().KeyChar == 'y') return;
            }
        }
    }
}
