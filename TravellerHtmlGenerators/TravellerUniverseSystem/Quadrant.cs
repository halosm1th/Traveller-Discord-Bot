using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traveller_subsector_generator;

namespace TravellerSubsectorMap
{
    class Quadrant
    {
        private MegaSector[,] megaSectors;
        private readonly int MEGA_X_SIZE = 8;
        private readonly int MEGA_Y_SIZE = 8;


        public long WorldCount
        {
            get
            {
                if (_worldCount <= 0)
                {
                    _worldCount = megaSectors.Cast<MegaSector>().Sum(quadrant => quadrant.WorldCount);
                }

                return _worldCount;
            }
        }

        private long _worldCount = -1;


        public string Name;
        public Galaxy Galaxy;
        private bool _AlphaQuadrantAndBerlinnic = false;
        public Quadrant(string name, int xSize, int ySize,Galaxy galaxy, bool alphaQuadrant =false)
        {
            Name = name;
            _AlphaQuadrantAndBerlinnic = alphaQuadrant;
            megaSectors = new MegaSector[ySize,xSize];
            Galaxy = galaxy;
        }

        public void GenerateQuadrants()
        {
            int current = 0;
            int total = megaSectors.GetLength(0) * megaSectors.GetLength(1)-1;
            for (int y = 0; y < megaSectors.GetLength(0); y++)
            {
                for (int x = 0; x < megaSectors.GetLength(1); x++)
                {
                    current++;
                    if (_AlphaQuadrantAndBerlinnic)
                    {

                    }

                    else
                    {
                        var mega = new MegaSector(Subsector.GenerateName(), MEGA_X_SIZE, MEGA_Y_SIZE, this);
                        megaSectors[y, x] = mega;
                        mega.GenerateMegaSector();
                    }

                    var percent = Math.Round((double) current / (double) total * 100, 0);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\r[{Name}] quadrant: {percent}% ({current}/{total})");
                    
                }
            }
        }

        private string GenerateMiddleText()
        {
            var sb = new StringBuilder();
            foreach (var mega in megaSectors)
            {
                var path = $"{mega.Name}/{mega.Name}".Replace(" ", "_");
                sb.Append($@"<li><a href=""{path}.html"">{mega.Name}</a></li>");
            }

            return sb.ToString();
        }

        public string GetHTML()
        {
            var sb = new StringBuilder();
            var topText = $@"<!DOCTYPE html><html><head><title>{Name}</title><link rel=""stylesheet"" href=""{Galaxy.StyleLocation}""></ head >
                <body><h1>{Name}</h1><p> the {Name} Quadrant is in the <a href=""../{Galaxy.Name.Replace(" ", "_")}.html"">{Galaxy.Name} Galaxy</a>contains the following MegaSectors: <ul> ";
            
            var bottomText = $@"</ul></p><h2>Stats</h2>Some stats about the {Name} Quadrant: <ul>
<li> Number of Planets: {WorldCount}</li></ul></body></html>";

            sb.Append(topText);
            var middleText = GenerateMiddleText();
            sb.Append(middleText);

            sb.Append(bottomText);
            return sb.ToString();
        }

        public void WriteWholeQuadrantHTML(string path)
        {
            int current = 0;
            int total = megaSectors.GetLength(0) * megaSectors.GetLength(1) -1;
            File.WriteAllText(path+$"/{Name.Replace(" ", "_")}.html",GetHTML());
            foreach (var mega in megaSectors)
            {
                var megaPath = $"{path}/{mega.Name.Replace(" ", "_")}";
                Directory.CreateDirectory(megaPath);
                mega.WriteWholeMegasectorHTML(megaPath);

                current++;
                var percent = Math.Round((double)current / (double)total * 100, 0);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"\r[{Name}] Quadrant: {percent}% ({current}/{total})");
            }

        }

        public async Task WriteWholeQuadrantHTMLAsync(string path)
        {
            Console.WriteLine();
            int current = 0;
            int total = megaSectors.GetLength(0) * megaSectors.GetLength(1) -1;
            var write = File.WriteAllTextAsync(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
            var megaTasks = new List<Task>();
            foreach (var mega in megaSectors)
            {
                var megaPath = $"{path}/{mega.Name.Replace(" ", "_")}";
                Directory.CreateDirectory(megaPath);
                var megaRun =Task.Run( () => mega.WriteWholeMegasectorHTML(megaPath));
                megaTasks.Add(megaRun);

            }

            current = megaTasks.Count(x => x.IsCompleted);
            while (current < total)
            {
                current = megaTasks.Count(x => x.IsCompleted);
                var percent = Math.Round((double)current / (double)total * 100, 0);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"\r[{Name}] Quadrant: {percent}% ({current}/{total})");
                await Task.Delay(1000 * total - current);
            }

            await write;
            Console.WriteLine();

        }
    }
}