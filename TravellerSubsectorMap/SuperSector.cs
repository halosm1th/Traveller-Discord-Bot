using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Traveller_subsector_generator
{
    class SuperSector
    {
        public string Name;
        Sector[,] sectors;

        public SuperSector(string name,int x, int y ,int xSize, int ySize)
        {
            sectors = new Sector[ySize,xSize];
            Name = name + $" {x},{y}";
        }

        public void GenerateSuperSector()
        {
            for (int y = 0; y < sectors.GetLength(0); y++)
            {
                for (int x = 0; x <sectors.GetLength(1); x++)
                {
                    var s = new Sector();

                    s.GenerateSubsectors(40);
                    sectors[y, x] = s;
                }
            }
        }

        public void WriteSuperSector(string path)
        {
            foreach (var sector in sectors)
            {
                var secPath = path + $"\\{sector.Name}_Sector";

                Directory.CreateDirectory(secPath);
                Console.WriteLine($"Writing {sector.Name} To: {secPath}");
                sector.WriteSector(secPath);
            }
        }
    }
}