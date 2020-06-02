using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Traveller_subsector_generator
{
    class Subsector
    {
        public World[,] Systems;
        public string Name;
        private static readonly int HEIGHT = 140;
        private static readonly float WIDTH = HexWidth(HEIGHT);
        private static List<string> names = File.ReadAllLines(Directory.GetCurrentDirectory() + "/placeName.txt").ToList();
       // private static List<string> usedNames = new List<string>();

        private static Random random = new Random();

        public Subsector(int x,int y)
        {
            Name = GenerateName() + $" subsector {x},{y}";
            Systems = new World[10, 8];
        }

        public Subsector(List<string> worlds,string name)
        {
            Name = name;
            Systems = new World[10,8];

            foreach (var worldText in worlds)
            {

                var world = new World(worldText);
                Systems[world.Y-1,world.X-1] = world;
            }

            for (int y = 0; y < Systems.GetLength(0); y++)
            {
                for (int x = 0; x < Systems.GetLength(1); x++)
                {
                    if (Systems[y, x] == null)
                    {
                        Systems[y, x] = new World(x, y);
                    }
                }
            }
        }

        public static string GenerateName()
        {
            if (names.Count == 1)
            {
                //names.AddRange(usedNames);
                //usedNames = new List<string>();
            } 

            var name = names[random.Next(0, names.Count)];

            //names.Remove(name);
            //usedNames.Add(name);
            return name;
        }

        public void GenerateSubsector(int worldOdds = 50)
        {
            
                for (int y = 0; y < Systems.GetLength(0); y++)
                {
                    for (int x = 0; x < Systems.GetLength(1); x++)
                    {
                        if (random.Next(0, 101) <= worldOdds)
                        {
                            Systems[y, x] = new World(x, y, GenerateName());
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Systems[y, x]);
                            if (Systems[y, x].Name == "Scicily")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("THE BARON HAS BEEN BORN");
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                        }
                        else
                        {
                            Systems[y, x] = new World(x, y);
                        }
                    }
                }
        }
        public void WriteSubsector(StreamWriter sw)
        {
            foreach (var system in Systems)
            {
                if (system.HasWorld)
                {
                    sw.WriteLine(system);
                    sw.Flush();
                }
            }
        }

        public List<World> Worlds()
        {
            return SystemsAsList().Where(x => x.HasWorld).ToList();
        }

        public List<World> SystemsAsList()
        {
            var worlds = new List<World>();
            foreach (var system in Systems)
            {
                worlds.Add(system);
            }

            return worlds;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var worlds = SystemsAsList().Where(x => x.HasWorld).ToList();
            sb.Append($"Subsector {Name}\n");
            foreach (var world in worlds)
            {
                sb.Append(world.ToString() + "\n");
            }

            return sb.ToString();
        }


        //The following region relies on the dotnet drawing library, which is not available in dotnet core. I need to find a replacement for this library soon.
        //As the code here will not always be compatible with the code elsewhere. Which is a damn shame
        #region Dotnet Framework stuff

        private static readonly int xSize = 1050;
        private static readonly int ySize = 1500;
        private static readonly Image GRID = CreateGrid();

        public Bitmap GenerateSubSectorImage()
        {
            var subsector = new Bitmap(GRID);

            DrawWorlds(subsector);

            return subsector;
        }

        private void DrawWorlds(Bitmap subsector)
        {
            var fontSize = 18;

            var graphics = Graphics.FromImage(subsector);
            var brush = new SolidBrush(Color.Black);
            var Font = new Font("Ariel", fontSize);

            for (int row = 0; row < Systems.GetLength(0); row++)
            {
                for (int col = 0; col < Systems.GetLength(1); col++)
                {
                    //Get HexCoords

                    DrawLocationText(row, col, fontSize, HEIGHT, WIDTH, Font, graphics, brush);

                    if (Systems[row, col].HasWorld)
                    {
                        DrawSystemName(row, col, fontSize, HEIGHT, WIDTH, Font, graphics, brush);
                        DrawUniversalWorldProfile(HEIGHT, row, col, WIDTH, Font, graphics, brush, Systems[row, col]);


                        //Get Text Coords
                        var y = (float)(HEIGHT / 2) + (row * HEIGHT);
                        if (col % 2 == 1) y += HEIGHT / 2;
                        y += fontSize * 1.5f;

                        var text = $"{Systems[row, col].Stations}".ToUpper();

                        var x = col * (WIDTH * 0.75f);
                        x += (WIDTH / 2) - (text.Length * Font.SizeInPoints) / 2.0f;
                        x += Font.Size;

                        graphics.DrawString(text, Font, brush, x, y);
                    }
                }
            }
        }

        private void DrawSystemName(int row, int col, int fontSize, int height, float width, Font Font, Graphics graphics,
            SolidBrush brush)
        {
            var text = $"{Systems[row, col].Name}";

            if (text.Length > 10)
            {
                text = text.Substring(0, 10);
            }

            var y = (fontSize * 2) + (row * height);
            if (col % 2 == 1) y += height / 2;

            var x = col * (width * 0.75f);
            x += (width / 2) - (text.Length * Font.Size) / 2.8f;

            graphics.DrawString(text, Font, brush, x, y);
        }

        private static void DrawLocationText(int row, int col, int fontSize, int height, float width, Font Font,
            Graphics graphics, SolidBrush brush)
        {
            var text = $"{col + 1} {row + 1}";

            var y = (fontSize) + (row * height);
            if (col % 2 == 1) y += height / 2;

            var x = col * (width * 0.75f);
            x += (width / 2) - (text.Length * Font.SizeInPoints) / 2.0f;

            graphics.DrawString(text, Font, brush, x, y - fontSize);
        }

        private static void DrawUniversalWorldProfile(int height, int row, int col, float width, Font Font, Graphics graphics,
            SolidBrush brush, World world)
        {
            //Get Text Coords
            var y = (height / 2) + (row * height);
            if (col % 2 == 1) y += height / 2;

            var text = $"{world.UWP}".ToUpper();

            var x = col * (width * 0.75f);
            x += (width / 2) - (text.Length * Font.SizeInPoints) / 2.0f;
            x += Font.Size;

            graphics.DrawString(text, Font, brush, x, y);
        }

        private static PointF[] HexToPoints(float height, int row, int Col)
        {
            float width = HexWidth(height);
            float y = height / 2;
            float x = 0;

            y += row * height;
            if (Col % 2 == 1) y += height / 2;

            x += Col * (width * 0.75f);

            return new PointF[]
            {
                new PointF(x,y),
                new PointF(x + width * 0.25f,y - height/2),
                new PointF(x + width * 0.75f, y - height / 2),
                new PointF(x + width, y),
                new PointF(x + width * 0.75f, y + height / 2),
                new PointF(x + width * 0.25f, y + height / 2),
            };
        }

        private static float HexWidth(float height)
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }

        private static Image CreateGrid()
        {
            var grid =new Bitmap(xSize, ySize);
            var graphics = Graphics.FromImage(grid);
            var brush = new SolidBrush(Color.White);
            graphics.FillRectangle(brush,new Rectangle(0,0,xSize,ySize));
            var pen = new Pen(Color.Black);

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var points = HexToPoints(HEIGHT, row, col);
                    graphics.DrawPolygon(pen, points);
                }
            }


            return grid as Image;
        }

        private Bitmap CreateSubsectorBitmap()
        {

            var subsector = new Bitmap(GRID);

            return subsector;
        }

        #endregion
    }
}