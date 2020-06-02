using System.Collections.Generic;
using System.Text;

namespace TravellerMapGraphs
{
    class Room
    {
        private static int currentID = 1;

        public string Name;
        //Size in meters
        public int XSizeM;
        public int YSizeM;
        public string Description;
        public int ID;
        public List<string> Items;
        public List<Room> ConnectingRooms;

        public Room(string name, int xSize, int ySize, string desc)
        {
            Name = name;
            XSizeM = xSize;
            YSizeM = ySize;
            Description = desc;
            ID = currentID;
            currentID++;
            Items = new List<string>();
            ConnectingRooms = new List<Room>();
        }

        public Room(string name, int xSize, int ySize, string desc, List<string> items)
        {
            Name = name;
            XSizeM = xSize;
            YSizeM = ySize;
            Description = desc;
            ID = currentID;
            currentID++;
            Items = items;
            ConnectingRooms = new List<Room>();
        }

        public void ConnectRoom(Room room)
        {
            if (!ConnectingRooms.Contains(room))
            {
                ConnectingRooms.Add(room);
                room.ConnectRoom(this);
            }
        }

        public void AddItem(string item)
        {
            Items.Add(item);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in Items)
            {
                sb.Append(item + " ");
            }

            return $"{Name}:{ID}\n" +
                   $"  Size: {XSizeM}Mx{YSizeM}M\n" +
                   $"  Description: {Description}\n" +
                   $"  Items: {sb.ToString()}\n";
        }
    }
}