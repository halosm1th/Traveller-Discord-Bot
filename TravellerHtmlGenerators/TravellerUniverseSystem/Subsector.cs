using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Traveller_subsector_generator
{
    class Subsector
    {
        public World[,] Systems;

        public long Population => (long) Systems.Cast<World>().Where(system => system.HasWorld).Sum(world => world.Popuation);
        public long WorldCount => (long) Systems.Cast<World>().Count( system => system.HasWorld);

        public string Name;
        private static List<string> names = File.ReadAllLines(Directory.GetCurrentDirectory() + "/placeName.txt").ToList();
       // private static List<string> usedNames = new List<string>();

        private static Random random = new Random();
        private Sector _sector;
        public Subsector(int x,int y, Sector sector)
        {
            Name = GenerateName() + $" subsector {x},{y}";
            Systems = new World[10, 8];
            _sector = sector;
        }

        public Subsector(List<string> worlds,string name, Sector sector)
        {
            Name = name;
            _sector = sector;
                Systems = new World[10,8];

            foreach (var worldText in worlds)
            {

                var world = new World(worldText);
                Systems[world.Y-1,world.X-1] = world;
            }

            for (int y = 0; y < Systems.GetLength(0); y++)
            {
                for (int x = 0; x < Systems.GetLength(1); x++)
                {
                    if (Systems[y, x] == null)
                    {
                        Systems[y, x] = new World(x, y,this);
                    }
                }
            }
        }

        public static string GenerateName()
        {

            var name = names[random.Next(0, names.Count)];

            //names.Remove(name);
            //usedNames.Add(name);
            return name;
        }


        private string GetMiddleText()
        {
            var sb = new StringBuilder();
            foreach (var system in Worlds())
            {
                var path = $"{system.Name}/{system.Name}".Replace(" ", "_");
                sb.Append($@"<li><a href=""{path}.html"">{system.Name}</a></li>");

            }

            return sb.ToString();
        }

        public string GetHTML()
        {
            var top = $@"<!DOCTYPE html>
<html>
<head>
    <title>{Name}</title>
    <link rel=""stylesheet"" href=""style.css"">
    <!-- This is a comment, by the way -->
</head>
<body>
<h1>{Name}</h1>
<p>the {Name} Subsector is in the <a href=""../{_sector.Name.Replace(" ","_")}.html"">{_sector.Name}</a> sector and contains the following {Systems.GetLength(0) * Systems.GetLength(1)} systems: 
    <ol>";
            var middle = GetMiddleText();
            var bottom = $@"

    </ol> 
</p>
<h2>Stats</h2>
Some stats about the {Name} Subsector:
<ul>
    <li>
        Population: {Population}
    </li>
    <li>
        Number of Planets: {WorldCount}
    </li>
</ul>
</body>
</html>";

            var sb = new StringBuilder();
            sb.Append(top);
            sb.Append(middle);
            sb.Append(bottom);

            return sb.ToString();
        }

        public void WriteWholeSubSectorHTML(string path)
        {
            Console.WriteLine();
            File.WriteAllText(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
            int current = 0;
            int total = Worlds().Count;
            foreach (var system in Worlds())
            {
                current++;
                var megaPath = $"{path}/{system.Name}".Replace(" ", "_");
                Directory.CreateDirectory(megaPath);
                system.WriteSystemHTML(megaPath);

                var percent = Math.Round((double)current / (double)total * 100, 0);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"\r[{Name}] SubSector: {percent}% ({current}/{total})");
            }
            Console.WriteLine();
        }

        public void GenerateSubsector(int worldOdds = 50)
        {
            
                for (int y = 0; y < Systems.GetLength(0); y++)
                {
                    for (int x = 0; x < Systems.GetLength(1); x++)
                    {
                        if (random.Next(0, 101) <= worldOdds)
                        {
                            Systems[y, x] = new World(x, y, GenerateName(),this);
                        }
                        else
                        {
                            Systems[y, x] = new World(x, y,this);
                        }
                    }
                }
        }
        public void WriteSubsector(StreamWriter sw)
        {
            foreach (var system in Systems)
            {
                if (system.HasWorld)
                {
                    sw.WriteLine(system);
                    sw.Flush();
                }
            }
        }

        public List<World> Worlds()
        {
            return SystemsAsList().Where(x => x.HasWorld).ToList();
        }

        public List<World> SystemsAsList()
        {
            var worlds = new List<World>();
            foreach (var system in Systems)
            {
                worlds.Add(system);
            }

            return worlds;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var worlds = SystemsAsList().Where(x => x.HasWorld).ToList();
            sb.Append($"Subsector {Name}\n");
            foreach (var world in worlds)
            {
                sb.Append(world.ToString() + "\n");
            }

            return sb.ToString();
        }

    }
}