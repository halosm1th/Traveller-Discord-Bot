using System;
using System.IO;
using System.Threading.Tasks;

namespace Traveller_subsector_generator
{
    class MegaSector
    {
        public string Name;
        private SuperSector[,] supersectors;

        public MegaSector(string name, int xSize, int ySize)
        {
            supersectors = new SuperSector[ySize, xSize];
            Name = name;
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    var s = new SuperSector(Subsector.GenerateName(),x,y,6,6);
                    supersectors[y, x] = s;
                }
            }
        }

        public void GenerateMegaSector(string path)
        {
            foreach (var supers in supersectors)
            {
                supers.GenerateSuperSector();
            }
        }
    }
}