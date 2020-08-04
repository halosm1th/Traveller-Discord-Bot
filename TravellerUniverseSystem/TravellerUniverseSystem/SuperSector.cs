﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TravellerUniverse
{
    class SuperSector
    {
        public string Name;


        public long WorldCount => sectors.Cast<Sector>().Sum(sector => sector.WorldCount);

        Sector[,] sectors;
        private MegaSector _mega;
         
        public SuperSector(string name,int x, int y ,int xSize, int ySize,MegaSector mega)
        {
            sectors = new Sector[ySize,xSize];
            Name = name + $" {x},{y}";
            _mega = mega;
        }

        private string GetMiddleText()
        {
            var sb = new StringBuilder();
            foreach (var sector in sectors)
            {
                var path = $"{sector.Name}/{sector.Name}".Replace(" ", "_");
                sb.Append($@"<li><a href=""{path}.html"">{sector.Name}</a></li>");

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
<p>the {Name} SuperSector is inside the <a href=""../{_mega.Name.Replace(" ", "_")}.html"">{_mega.Name} Megasector</a> and contains the following {sectors.GetLength(0) * sectors.GetLength(1)} Sectors: 
    <ol>";
            var middle = GetMiddleText();
            var bottom = $@"

    </ol> 
</p>
<h2>Stats</h2>
Some stats about the {Name} SuperSector:
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

        public void WriteWholeSupersectorHTML(string path)
        {
            File.WriteAllText(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
            foreach (var sector in sectors)
            {
                var megaPath = $"{path}/{sector.Name}".Replace(" ", "_");
                Directory.CreateDirectory(megaPath);
                sector.WriteWholeSectorHTML(megaPath);
            }

        }

        public async Task WriteWholeSupersectorHTMLAsync(string path)
        {
            Console.WriteLine();
            int current = 0;
            var total = sectors.GetLength(0) * sectors.GetLength(1);
            await File.WriteAllTextAsync(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
            foreach (var sector in sectors)
            {
                var megaPath = $"{path}/{sector.Name}".Replace(" ", "_");
                Directory.CreateDirectory(megaPath);
                await sector.WriteWholeSectorHTMLAsync(megaPath);

                current++;
                var percent = Math.Round((double)current / (double)total * 100, 0);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\r[{Name}] SuperSector: {percent}% ({current}/{total})");
            }

        }

        public void GenerateSuperSector()
        {
            for (int y = 0; y < sectors.GetLength(0); y++)
            {
                for (int x = 0; x <sectors.GetLength(1); x++)
                {
                    var s = new Sector(this);

                    s.GenerateSubsectors(40);
                    sectors[y, x] = s;

                }
            }
        }
    }
}