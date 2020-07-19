using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TravellerSubsectorMap;

namespace Traveller_subsector_generator
{
    class MegaSector
    {
        public string Name;


        public long WorldCount
        {
            get
            {
                if (_worldCount <= 0)
                {
                    _worldCount = supersectors.Cast<SuperSector>().Sum(quadrant => quadrant.WorldCount);
                }

                return _worldCount;
            }
        }

        private long _worldCount = 0;
        private Quadrant _quadrant;

        private SuperSector[,] supersectors;
        private int _XSize;
        private int _YSize;


        public MegaSector(string name, int xSize, int ySize,Quadrant quadrant)
        {
            supersectors = new SuperSector[ySize, xSize];
            Name = name;
            _XSize = xSize;
            _YSize = ySize;
            _quadrant = quadrant;
        }

        private string GenerateMidHTML()
        {
            var sb = new StringBuilder();
            foreach (var super in supersectors)
            {
                var path = $"{super.Name}/{super.Name}".Replace(" ", "_");
                sb.Append($@"<li><a href=""{path}.html"">{super.Name}</a></li>");
            }

            return sb.ToString();
        }

        public string GetHTML()
        {
            var sb = new StringBuilder();

            var topHTML = $@"
<!DOCTYPE html>
<html>
    <head>
        <title>{Name}</title>
        <link rel=""stylesheet"" href=""{Galaxy.StyleLocation}"">
    <!-- This is a comment, by the way -->
    </head>
    <body>
    <h1>{Name}</h1>
    <p>the {Name} megasector is inside the <a href=""../{_quadrant.Name.Replace(" ", "_")}.html"">{_quadrant.Name} Quadrant</a> and contains the following {supersectors.GetLength(0) * supersectors.GetLength(1)} SuperSectors: 
    <ol>";
            var midHtml = GenerateMidHTML();
            var bottomHTML = $@"    
    </ol> 
    </p>
    <h2>Stats</h2>
    <p>Some stats about the {Name} MegaSector: <p>
    <ul>
        <li>
            Number of Planets: {WorldCount}
        </li>
    </ul>
    </body>
</html>";
            sb.Append(topHTML);
            sb.Append(midHtml);
            sb.Append(bottomHTML);
            return sb.ToString();
        }

        public void WriteWholeMegasectorHTML(string path)
        {
            int current = 0;
            int total = supersectors.GetLength(0) * supersectors.GetLength(1);
            File.WriteAllText(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
            foreach (var super in supersectors)
            {
                var megaPath = $"{path}/{super.Name}".Replace(" ", "_");
                Directory.CreateDirectory(megaPath);
                super.WriteWholeSupersectorHTML(megaPath);

                current++;
                var percent = Math.Round((double)current / (double)total * 100, 0);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"\r[{Name}] megaSector: {percent}% ({current}/{total})");
            }
        }

        public async Task WriteWholeMegasectorHTMLAsync(string path)
        {

            WriteWholeMegasectorHTML(path);
            /*1
                        Console.WriteLine();
                        int current = 0;
                        int total = supersectors.GetLength(0) * supersectors.GetLength(1);
                        await File.WriteAllTextAsync(path + $"/{Name.Replace(" ", "_")}.html", GetHTML());
                        foreach (var super in supersectors)
                        {
                            var megaPath = $"{path}/{super.Name}".Replace(" ", "_");
                            Directory.CreateDirectory(megaPath);
                            await super.WriteWholeSupersectorHTMLAsync(megaPath);

                            current++;
                            var percent = Math.Round((double)current / (double)total * 100, 0);
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine($"\r[{Name}] megaSector: {percent}% ({current}/{total})");
                        }*/
        }

        public void GenerateMegaSector()
        {
            int current = 0;
            int total = supersectors.GetLength(0) * supersectors.GetLength(1);
            for (int y = 0; y < _YSize; y++)
            {
                for (int x = 0; x < _XSize; x++)
                {
                    current++;
                    var s = new SuperSector(Subsector.GenerateName(), x, y, 6, 6,this);
                    s.GenerateSuperSector();
                    supersectors[y, x] = s;

                    var percent = Math.Round((double)current / (double)total * 100, 0);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"\r[{Name}] megaSector: {percent}% ({current}/{total})");
                }
            }

        }
    }
}