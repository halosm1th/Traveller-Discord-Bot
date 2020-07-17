using System.IO;
using Traveller_subsector_generator;

namespace TravellerSubsectorMap
{
    class Quadrant
    {
        private MegaSector[,] megaSectors;
        private readonly int MEGA_X_SIZE = 8;
        private readonly int MEGA_Y_SIZE = 8;


        public string Name;
        public Quadrant(string name, int xSize, int ySize)
        {
            Name = name;
            megaSectors = new MegaSector[ySize,xSize];
        }

        public void GenerateQuadrants(string path)
        {
            for (int y = 0; y < megaSectors.GetLength(0); y++)
            {
                for (int x = 0; x < megaSectors.GetLength(1); x++)
                {
                    var mega = new MegaSector(Subsector.GenerateName(), MEGA_X_SIZE, MEGA_Y_SIZE);
                    megaSectors[y, x] = mega;
                    mega.GenerateMegaSector();

                }
            }
        }
    }
}