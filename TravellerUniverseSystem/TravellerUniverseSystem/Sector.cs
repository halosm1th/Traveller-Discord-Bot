using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellerUniverse
{
    class Sector
    {
        private Subsector[,] subsectors;
        public string Name;
        private SuperSector _super;

        public long WorldCount => subsectors.Cast<Subsector>().Sum(sub => sub.WorldCount);

        public Sector(SuperSector supersector)
        {
            subsectors = new Subsector[4,4];
            Name = Subsector.GenerateName();
            _super = supersector;
        }

        private string GetMiddleText()
        {
            var sb = new StringBuilder();
            foreach (var sub in subsectors)
            {
                var path = $"{sub.Name}/{sub.Name}".Replace(" ", "_");
                sb.Append($@"<li><a href=""{path}.html"">{sub.Name}</a></li>");

            }

            return sb.ToString();
        }

        public string GetHTML()
        {
            var top = $@"<!DOCTYPE html>
<html>
<head>
    <title>{Name}</title>
    <link rel=""stylesheet"" href=""{Galaxy.StyleLocation}"">
    <!-- This is a comment, by the way -->
</head>
<body>
<h1>{Name}</h1>
<p>the {Name} Sector is in the <a href=""../{_super.Name.Replace(" ", "_")}.html"">{_super.Name} Supersector</a> and contains the following {subsectors.GetLength(0) * subsectors.GetLength(1)} Subsectors: 
    <ol>";
            var middle = GetMiddleText();
            var bottom = $@"

    </ol> 
</p>
<h2>Stats</h2>
Some stats about the {Name} Sector:
<ul>
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

        public void WriteWholeSectorHTML(string path)
        {
            File.WriteAllText(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
            foreach (var sub in subsectors)
            {   
                var megaPath = $"{path}/{sub.Name}".Replace(" ", "_");
                Directory.CreateDirectory(megaPath);
                sub.WriteWholeSubSectorHTML(megaPath);
            }
        }
        public async Task WriteWholeSectorHTMLAsync(string path)
        {
            Console.WriteLine();
          var file= File.WriteAllTextAsync(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
            foreach (var sub in subsectors)
            {
                var megaPath = $"{path}/{sub.Name}".Replace(" ", "_");
                Directory.CreateDirectory(megaPath);
                await sub.WriteWholeSubSectorHTMLAsync(megaPath);
            }

            await file;
            Console.WriteLine();
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
                    subsectors[y, x] = new Subsector(subsectorsData, subsectorName,this);
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

            subsectors[y,x] = new Subsector(subsectorsData,subsectorName,this);
        }

        public void GenerateSubsectors(int worldChance = 50)
        {
            for (int y = 0; y < subsectors.GetLength(0); y++)
            {
                for (int x = 0; x < subsectors.GetLength(1); x++)
                {
                    subsectors[y,x] = new Subsector(x,y,this);
                    subsectors[y,x].GenerateSubsector(worldChance);

                }
            }
        }

    }
}