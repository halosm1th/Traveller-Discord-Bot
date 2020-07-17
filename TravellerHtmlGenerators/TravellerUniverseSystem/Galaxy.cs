using System;
using System.IO;
using System.Linq;

namespace TravellerSubsectorMap
{
    class Galaxy
    {
        private Quadrant[,] quadrants;
        public string Name;
        public readonly int QUADRANT_SIZE = 3;

        public long Population
        {
            get
            {
                if (_pop <= 0)
                {
                    _pop = quadrants.Cast<Quadrant>().Sum(quadrant => quadrant.Population);
                }

                return _pop;
            }
        }

        public long WorldCount
        {
            get
            {
                if (_worldCount <= 0)
                {
                    _worldCount = quadrants.Cast<Quadrant>().Sum(quadrant => quadrant.WorldCount);
                }

                return _worldCount;
            }
        } 

        private long _pop = -1;
        private long _worldCount = -1;

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

        public void GenerateGalaxy()
        {
            int current = 0;
            int total = quadrants.GetLength(0) + quadrants.GetLength(1);
            for (int y = 0; y < quadrants.GetLength(0); y++)
            {
                for (int x = 0; x < quadrants.GetLength(1); x++)
                {
                    current++;
                    var qua = new Quadrant(QuadrantName(x, y), QUADRANT_SIZE, QUADRANT_SIZE,this);
                    quadrants[y, x] = qua;
                    qua.GenerateQuadrants();

                    var percent = Math.Round((double)current / (double)total * 100, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{Name}] Galaxy: {percent}% ({current}/{total})");
                }
            }
        }

        public string GetHTML()
        {
            var template = File.ReadAllText(Directory.GetCurrentDirectory() + "/galaxyTemplate.html");

            var qua1Name = quadrants[0, 0].Name.Replace(" ","_");
            var qua2Name = quadrants[0, 1].Name.Replace(" ", "_");
            var qua3Name = quadrants[1, 0].Name.Replace(" ", "_");
            var qua4Name = quadrants[1, 1].Name.Replace(" ", "_");
            
            
            return String.Format(template, 
                Name,qua1Name,qua2Name,qua3Name,qua4Name,Population,WorldCount);
        }

        /// <summary>
        /// Write the HTML page for this galaxy
        /// </summary>
        /// <param name="Path">The location on disk to write this html page.</param>
        public void WriteGalaxyHTML(string Path)
        {
            File.WriteAllText(Path,GetHTML());
        }

        /// <summary>
        /// Write the whole galaxies html pages (ie quadrent down).
        /// </summary>
        /// <param name="path"></param>
        public void WriteWholeGalaxyHTML(string path)
        {
            Console.WriteLine();
            int current = 0;
            int total = quadrants.GetLength(0) * quadrants.GetLength(1);
            File.WriteAllText(path + $"/{Name}.html",GetHTML());
            foreach (var quadrent in quadrants)
            {
                var quadrantPath = $"{path}/{quadrent.Name}".Replace(" ","_");
                Directory.CreateDirectory(quadrantPath);
                quadrent.WriteWholeQuadrantHTML(quadrantPath);

                current++;
                var percent = Math.Round((double)current / (double)total * 100, 0);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write($"\r[{Name}] Galaxy: {percent}% ({current}/{total})");

            }
        }
    }
}