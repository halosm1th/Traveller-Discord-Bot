using System;
using System.IO;
using System.Linq;
using System.Text;
using Traveller_subsector_generator;

namespace TravellerSubsectorMap
{
    class Quadrant
    {
        private MegaSector[,] megaSectors;
        private readonly int MEGA_X_SIZE = 8;
        private readonly int MEGA_Y_SIZE = 8;
        public long Population
        {
            get
            {
                if (_pop <= 0)
                {
                    _pop = megaSectors.Cast<MegaSector>().Sum(quadrant => quadrant.Population);
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
                    _worldCount = megaSectors.Cast<MegaSector>().Sum(quadrant => quadrant.WorldCount);
                }

                return _worldCount;
            }
        }

        private long _pop = -1;
        private long _worldCount = -1;


        public string Name;
        public Galaxy Galaxy;
        public Quadrant(string name, int xSize, int ySize,Galaxy galaxy)
        {
            Name = name;
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
                    var mega = new MegaSector(Subsector.GenerateName(), MEGA_X_SIZE, MEGA_Y_SIZE,this);
                    megaSectors[y, x] = mega;
                    mega.GenerateMegaSector();

                    var percent = Math.Round((double)current / (double)total * 100, 0);
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
            var topText = $@"<!DOCTYPE html><html><head><title>{Name}</title><link rel=""stylesheet"" href=""style.css""></ head >
                <body><h1>{Name}</h1><p> the {Name} Quadrant contains the following MegaSectors: <ul> ";
            
            var bottomText = $@"</ul></p><h2>Stats</h2>Some stats about the {Name} Quadrant: <ul><li>Population: {Population}
                </li><li> Number of Planets: {WorldCount}</li></ul></body></html>";

            sb.Append(topText);
            var middleText = GenerateMiddleText();
            sb.Append(middleText);

            sb.Append(bottomText);
            return sb.ToString();
        }

        public void WriteWholeQuadrantHTML(string path)
        {
            Console.WriteLine();
            int current = 0;
            int total = megaSectors.GetLength(0) * megaSectors.GetLength(1);
            File.WriteAllText(path+$"/{Name.Replace(" ", "_")}.html",GetHTML());
            foreach (var mega in megaSectors)
            {
                var megaPath = $"{path}/{mega.Name.Replace(" ", "_")}";
                Directory.CreateDirectory(megaPath);
                mega.WriteWholeMegasectorHTML(megaPath);

                current++;
                var percent = Math.Round((double)current / (double)total * 100, 0);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write($"\r[{Name}] Quadrant: {percent}% ({current}/{total})");
            }
            Console.WriteLine();

        }
    }
}