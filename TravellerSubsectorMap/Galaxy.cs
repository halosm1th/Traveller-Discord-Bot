using System;
using System.IO;

namespace TravellerSubsectorMap
{
    class Galaxy
    {
        private Quadrant[,] quadrants;
        public string Name;
        public readonly int QUADRANT_SIZE = 3;

        public Galaxy(string name)
        {
            Name = name;
            quadrants = new Quadrant[2,2];
        }

        private string QuadrantName(int x, int y)
        {
            if (y == 1)
            {
                if (x == 0)
                {
                    return "Alpha Quadrant";
                }
                else
                {
                    return "Beta Quadrant";
                }
            }
            else
            {
                if (x == 0)
                {
                    return "Gamma Quadrant";
                }

                else
                {
                    return "Delta Quadrant";
                }
            }
        }

        public void GenerateAndWriteGalaxy(string path)
        {
            for (int y = 0; y < quadrants.GetLength(0); y++)
            {
                for (int x = 0; x < quadrants.GetLength(1); x++)
                {
                    var qua = new Quadrant(QuadrantName(x, y), QUADRANT_SIZE, QUADRANT_SIZE);

                    var quaPath = path + $"\\{qua.Name}\\";
                    Directory.CreateDirectory(quaPath);
                    //Console.WriteLine($"Writing quadrant {qua.Name} to {quaPath}");
                    qua.GenerateAndWriteQuadrants(quaPath);
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Clear();
            Console.WriteLine("Done...");
            Console.ReadLine();
        }
    }
}