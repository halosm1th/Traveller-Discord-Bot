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
        public string Name;
        private static List<string> names = File.ReadAllLines(Directory.GetCurrentDirectory() + "/placeName.txt").ToList();
       // private static List<string> usedNames = new List<string>();

        private static Random random = new Random();

        public Subsector(int x,int y)
        {
            Name = GenerateName() + $" subsector {x},{y}";
            Systems = new World[10, 8];
        }

        public Subsector(List<string> worlds,string name)
        {
            Name = name;
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
                        Systems[y, x] = new World(x, y);
                    }
                }
            }
        }

        public static string GenerateName()
        {
            if (names.Count == 1)
            {
                //names.AddRange(usedNames);
                //usedNames = new List<string>();
            } 

            var name = names[random.Next(0, names.Count)];

            //names.Remove(name);
            //usedNames.Add(name);
            return name;
        }

        public void GenerateSubsector(int worldOdds = 50)
        {
            
                for (int y = 0; y < Systems.GetLength(0); y++)
                {
                    for (int x = 0; x < Systems.GetLength(1); x++)
                    {
                        if (random.Next(0, 101) <= worldOdds)
                        {
                            Systems[y, x] = new World(x, y, GenerateName());
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Systems[y, x]);
                            if (Systems[y, x].Name == "Scicily")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("THE BARON HAS BEEN BORN");
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                        }
                        else
                        {
                            Systems[y, x] = new World(x, y);
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