using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Traveller_subsector_generator
{
    class Sector
    {
        private Subsector[,] subsectors;
        public string Name;

        public Sector()
        {
            subsectors = new Subsector[4,4];
            Name = Subsector.GenerateName();
        }

        public Sector(string[] text,string name)
        {
            subsectors = new Subsector[4,4];
            Name = name;
            var subsectorsData = new List<string>();
            var x = 0;
            var y = 0;
            var subsectorName = "";
            foreach (var line in text)
            {
                if (line == "")
                {
                    subsectors[y, x] = new Subsector(subsectorsData, subsectorName);
                    x++;
                    if (x >= 4)
                    {
                        x = 0;
                        y++;
                    }

                    subsectorsData = new List<string>();
                }else if (line.Contains("Subsector"))
                {
                    subsectorName = line.Remove(0, "Subsector ".Length);
                }
                else
                {
                    subsectorsData.Add(line);
                }
            }

            subsectors[y,x] = new Subsector(subsectorsData,subsectorName);
        }

        public void GenerateSubsectors(int worldChance = 50)
        {
            for (int y = 0; y < subsectors.GetLength(0); y++)
            {
                for (int x = 0; x < subsectors.GetLength(1); x++)
                {
                    subsectors[y,x] = new Subsector(x,y);
                    subsectors[y,x].GenerateSubsector(worldChance);
                }
            }
        }

        public void WriteSector(string path)
        {
            Directory.SetCurrentDirectory(path);
            foreach (var subsector in subsectors)
            {
                var sub= subsector.GenerateSubSectorImage();
                sub.Save(subsector.Name + ".jpg");
               // Console.ForegroundColor = ConsoleColor.Red;
               // Console.WriteLine($"Writing subsector {subsector.Name}");
              //  Console.ForegroundColor = ConsoleColor.Green;
                File.WriteAllText(path + $"\\{subsector.Name}.txt",subsector.ToString());
            }
        }

    }
}