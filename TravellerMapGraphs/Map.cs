using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.Msagl.Drawing;
using Color = Microsoft.Msagl.Drawing.Color;

namespace TravellerMapGraphs
{
    enum ConnectionTypes
    {
        Success, Failure, AlreadyCompleted
    }
    class Map
    {
        public string Name;
        
        public List<Room> FirstRooms = new List<Room>();
        public int StartingRooms;
        public int RoomCount => map.Count;
        //Room name, room
        Dictionary<string, Room> map = new Dictionary<string,Room>();

        Graph mapGraph = new Graph();

        public Map(string name, int startingRooms = 1)
        {
            Name = name;
            StartingRooms = startingRooms;
        }

        public void AddRoom(Room room)
        {
            if (FirstRooms.Count < StartingRooms)
            {
                FirstRooms.Add(room);
            }
            map.Add(room.Name,room);
        }

        public ConnectionTypes ConnectRoom(string startRoom, string connectingRoom)
        {
            try
            {
                var beginRoom = map[startRoom];
                var endRoom = map[connectingRoom];
                if (endRoom != null && beginRoom != null)
                {
                    return ConnectRoom(beginRoom, endRoom);
                }
            }
            catch (KeyNotFoundException)
            {
            }

            return ConnectionTypes.Failure;
        }


        public ConnectionTypes ConnectRoom(Room startRoom, string connectingRoom)
        {
            try
            {
                var room = map[connectingRoom];
                if (room != null)
                {
                    return ConnectRoom(startRoom, room);
                }
            }
            catch (KeyNotFoundException e)
            {

            }
            return ConnectionTypes.Failure;
        }

        public ConnectionTypes ConnectRoom(Room startRoom, Room endRoom)
        {
            if (mapGraph.Edges.Count(
                edge => (edge.Source == startRoom.Name && edge.Target == endRoom.Name) ||
                (edge.Source == endRoom.Name && edge.Target == startRoom.Name)) > 1 )
            {
                return ConnectionTypes.AlreadyCompleted;
            }

            try
            {
                startRoom.ConnectRoom(endRoom);
                mapGraph.AddEdge(startRoom.Name, endRoom.Name);
                mapGraph.AddEdge(endRoom.Name, startRoom.Name);
                return ConnectionTypes.Success;
            }
            catch (Exception)
            {
                return ConnectionTypes.Failure;
            }
        }

        public List<Room> GetAllRooms()
        {
            return map.Values.ToList();
        }

        public Room GetRoom(string name)
        {
            var room = map[name];
            if (room != null)
            {
                return room;
            }

            throw new KeyNotFoundException("Room not found!");
        }

        public void Save()
        {
            SaveImage();
            SaveText();
        }

        private void SaveText()
        {

            string path = Name.Contains("\\") ? Name : Directory.GetCurrentDirectory() + $"/{Name}";
            path += ".txt";
            using (var sw =
                new StreamWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                foreach (var room in map)
                {
                    sw.WriteLine(room.ToString());
                    sw.WriteLine();
                    sw.Flush();
                }
            }
        }

        private void SaveImage()
        {
            foreach (var firstRoom in FirstRooms)
            {
                mapGraph.FindNode(firstRoom.Name).Attr.FillColor = Color.Green;
            }

            var renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(mapGraph);
            renderer.CalculateLayout();
            int width = 200 * RoomCount;
            Bitmap bitmap = new Bitmap(width, (int) (mapGraph.Height *
                                                     (width / mapGraph.Width)), PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);
            bitmap.Save(Name + ".png");
        }
    }
}