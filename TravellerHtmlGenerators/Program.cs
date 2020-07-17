using System;
using System.IO;
using System.Text;
using TravellerSubsectorMap;

namespace TravellerHtmlGenerators
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "galaxy":
                            CreateGalaxyHTMLPages();
                            break;
                        case "table":
                            CreateUCTable();
                            break;
                        default:
                            break;
                    }
                }
            }

            CreateGalaxyHTMLPages();
        }

        private static void CreateGalaxyHTMLPages()
        {
            var galaxy = new Galaxy("Berlinnic");
            galaxy.GenerateGalaxy();
            var path = Directory.GetCurrentDirectory() + $"/{galaxy.Name}/";
            Directory.CreateDirectory(path);
            galaxy.WriteWholeGalaxyHTML(path);
        }

        private static void CreateUCTable()
        {
            var sb = new StringBuilder();
            var input = "";
            while (input?.ToLower() != "n")
            {
                var gen = UniversalisConfederationTableGenerator.CreateConfederationTableGenerator();
                sb.Append(gen.ToHtml() + "\n");
                Console.WriteLine("Would you like to add another? ([n] if no)");
                input = Console.ReadLine();
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/newText.html", sb.ToString());
        }
    }
}
