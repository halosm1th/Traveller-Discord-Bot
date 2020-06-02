using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;

namespace TravellerMapGraphs
{
    static class Program
    {
        private static int _startingRooms = 1;
        static void Main(string[] args)
        {
            string filename = "file.txt";
            
            
            //TODO remove true
            if (args.Length == 0)
            {
                ConsoleMode();
            }
            else
            {
                try
                {
                    if (args.Length > 1)
                    {
                        filename = args[0];
                        if (args.Length > 1)
                        {
                            _startingRooms = Int32.TryParse(args[1], out _startingRooms) ? _startingRooms : 1;
                        }
                    }

                    Console.WriteLine(_startingRooms);

                    Console.WriteLine($"Loading {filename}");
                    var m = ParseFile(filename,_startingRooms);
                    Console.WriteLine($"Saving to {Directory.GetCurrentDirectory() + "/" + filename}");
                    m.Save();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error! It is: " + e);
                    Console.WriteLine("Useage: {programName} PATH [NUMBER OF STARTING POINTS]");
                }
            }

        }

        enum Commands
        {
            NewRoom,
            ConnectRooms,
            RemoveConnection,
            ListRooms,
            ListConnections,
            EditRoom,
            AddItem,
            ListItems,
            EditItem,
            EditDescription,
            EditSize,
            LoadFile,
            SaveFile,
            ClearScreen,
            Help,
            Exit,
            Error
        }

        static Commands parseCommand(string command)
        {
            command = command.ToLower();
            if (command == "new" || command == "nr" || command == "newroom")
            {
                return Commands.NewRoom;
            }
            else if (command == "connect" || command == "cr" || command == "connectrooms")
            {
                return Commands.ConnectRooms;
            }
            else if (command == "remove" || command == "r" || command == "removeconnection")
            {

                return Commands.RemoveConnection;
            }
            else if (command == "list" || command == "lr" || command == "listr" || command == "listrooms")
            {

                return Commands.ListRooms;
            }
            else if (command == "listc" || command == "lc" || command == "listconnections")
            {

                return Commands.ListConnections;
            }
            else if (command == "editroom" || command == "er")
            {

                return Commands.EditRoom;
            }
            else if (command == "ai" || command == "additem")
            {

                return Commands.AddItem;
            }
            else if (command == "li" || command == "listitem")
            {

                return Commands.ListItems;
            }
            else if (command == "ei" || command == "edititem")
            {

                return Commands.EditItem;
            }
            else if (command == "ed" || command == "editdescription")
            {

                return Commands.EditDescription;
            }
            else if (command == "es" || command == "editsize")
            {

                return Commands.EditSize;
            }
            else if (command == "?" || command == "help" || command == "h")
            {
                return Commands.Help;
            }
            else if (command == "exit" || command == "e" || command == "quit" || command == "q")
            {

                return Commands.Exit;
            }
            else if (command == "loadfile" || command == "lf")
            {
                return Commands.LoadFile;
            }
            else if (command == "savefile" || command == "sf")
            {
                return Commands.SaveFile;
            }
            else if (command == "cls" || command == "clear")
            {
                return Commands.ClearScreen;
            }
            else
            {
                return Commands.Error;
            }
        }

        static int GetNumber(string prompt)
        {
            Console.WriteLine("Room xSize: ");
            var num = Console.ReadLine();
            var number = 0;
            var convert = Int32.TryParse(num, out number);
            if (!convert)
            {
                number = GetNumber(prompt);
            }

            return number;
        }

        static void NewRoom(Map map)
        {
            Console.Write("Room Name: ");
            var name = Console.ReadLine();
            var xSize = GetNumber("XSize");
            var ySize = GetNumber("ySize");
            Console.Write("Room description: ");
            var description = Console.ReadLine();

            Console.WriteLine("Add Item(y/n)?");
            var hasItems = Console.ReadLine();
            var items = new List<string>();
            if (!(hasItems.ToLower() == "y"))
            {

                while (hasItems == "y")
                {
                    Console.Write("item>");
                    var item = Console.ReadLine();

                    items.Add(item);
                    Console.WriteLine("Add another(y)?");
                    hasItems = Console.ReadLine();
                }
            }

            var room = new Room(name, xSize, ySize, description, items);
            map.AddRoom(room);

        }

        static void ConnectRoom(Map map)
        {
            Console.Write("First room name> ");
            var firstRoom = Console.ReadLine();
            Console.Write("Second Room Name> ");
            var secondRoom = Console.ReadLine();

            if (map.ConnectRoom(firstRoom, secondRoom) == ConnectionTypes.Failure)
            {
                ConnectRoom(map);
            }
        }

        static void ConsoleMode()
        {
            bool running = true;
            string command = "";
            Room currentRoom = null;

            Console.Write("Map Name= ");
            var mapName = Console.ReadLine();
            var map = new Map("");

            while (running)
            {
                var commandResult = parseCommand(command);
                switch (commandResult)
                {
                    case Commands.NewRoom:
                        NewRoom(map);
                        break;
                    case Commands.ConnectRooms:
                        ConnectRoom(map);
                        break;
                    case Commands.RemoveConnection:
                        break;
                    case Commands.ListRooms:
                        Console.WriteLine(mapName);
                        foreach (var room in map.GetAllRooms())
                        {
                            Console.Write($"{room}");
                        }
                        break;
                    case Commands.ListConnections:
                        break;
                    case Commands.EditItem:
                        break;
                    case Commands.EditDescription:
                        break;
                    case Commands.EditSize:
                        break;
                    case Commands.LoadFile:
                        Console.Write("filename to load: ");
                        var loadMapName = Console.ReadLine();
                        map = ParseFile(loadMapName);
                        mapName = map.Name;
                        break;
                    case Commands.SaveFile:
                        map.Save();
                        break;
                    case Commands.ClearScreen:
                        Console.Clear();
                        break;
                    case Commands.Help:
                        Console.WriteLine("NewRoom" +
                                          "\nConnectRooms" +
                                          "\nRemoveConnection" +
                                          "\nListRooms" +
                                          "\nListConnections" +
                                          "\nEditRoom" +
                                          "\nAddItem" +
                                          "\nListItems" +
                                          "\nEditItem" +
                                          "\nEditDescription" +
                                          "\nEditSize" +
                                          "\nLoadFile" +
                                          "\nSaveFile" +
                                          "\nClearScreen" +
                                          "\nHelp" +
                                          "\nExit" +
                                          "\nError");
                        break;
                    case Commands.Exit:
                        running = false;
                        break;
                    case Commands.Error:
                        break;

                }

                Console.ForegroundColor = ConsoleColor.Gray;

                var printText = currentRoom == null ? mapName : currentRoom.Name; 
                Console.Write($"[${printText}]:> ");
                command = Console.ReadLine();
            }

            map.Save();
        }

        enum TokenType
        {
            Name,
            XSize,
            YSize,
            Description,
            Item,
            Connection
        }


        static Dictionary<TokenType, ListOrString> TokenizeRoom(string line)
        {
            //NAME xSIZE ySIZE "DESCRPTION" ["ITEMS" "ITEMS"] [connections]

            int index = 0;
            bool inString = false;
            bool inList = false;
            bool doneItems = false;

            TokenType currentTokenType = TokenType.Name;
            var tokens= new Dictionary<TokenType, ListOrString>();
            string currentToken = "";

            for (index = 0; index < line.Length; index++)
            {
                var token = line[index];
                if (token == '\"')
                {
                    //Invert if we are in a string or not.
                    inString = !inString;
                }else if (token == '[')
                {
                    inList = true;
                }else if (token == ']')
                {
                    inList = false;
                }else if (token == ' ' && !inString)
                {
                    if (inList)
                    {
                        ListToken(tokens, currentTokenType, currentToken);
                    }
                    else
                    {
                        if (currentToken != "")
                        {
                            tokens[currentTokenType] = new ListOrString(currentToken);
                            currentTokenType++;
                        }
                    }

                    currentToken = "";
                }
                else
                {
                    currentToken += token;
                }
            }

            ListToken(tokens, currentTokenType, currentToken);

            return tokens;
        }

        private static void ListToken(Dictionary<TokenType, ListOrString> tokens, TokenType currentTokenType, string currentToken)
        {
            if (tokens.ContainsKey(currentTokenType))
            {
                tokens[currentTokenType].TextList.Add(currentToken);
            }
            else
            {
                tokens[currentTokenType] = new ListOrString(currentToken);
            }
        }

        //Room that you failed to connect to. the list contains the failed connections
        static Dictionary<string, List<string>> FailedConnections = new Dictionary<string, List<string>>();

        static Map ParseFile(string mapName, int startingPoints = 1)
        {
            var roomListText = LoadFile(mapName);
            Map map = new Map(mapName,startingPoints);

            foreach (var roomText in roomListText)
            {
                var tokens = TokenizeRoom(roomText);
                var roomName = tokens[TokenType.Name].Text;
                var roomDesc = tokens[TokenType.Description].Text;
                var roomX = Convert.ToInt32(tokens[TokenType.XSize].Text);
                var roomY = Convert.ToInt32(tokens[TokenType.YSize].Text);
                var roomItems = tokens[TokenType.Item].TextList;
                var roomConnections = tokens[TokenType.Connection].TextList;

                var loggingItems = tokens[TokenType.Item];
                var loggingConnectionss = tokens[TokenType.Connection];
                Console.WriteLine($"{roomName} {roomX}x{roomY} {roomDesc} [{loggingItems}] [{loggingConnectionss}]");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Creating room");
                var room = new Room(roomName, roomX,roomY,roomDesc);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Finished room");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("adding Items");

                foreach (var item in roomItems)
                {
                    room.AddItem(item);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($" Adding: {item}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Finshed adding items");

                Console.ForegroundColor = ConsoleColor.Yellow; 
                Console.WriteLine("ConnectingRooms");

                map.AddRoom(room);
                foreach (var connection in roomConnections)
                {
                    ConnectRooms(room, connection, map);
                }

                try
                {
                    foreach (var failedConnection in FailedConnections[room.Name])
                    {
                        ConnectRooms(room, failedConnection, map);
                    }
                }
                catch (Exception)
                {

                }
            }

            return map;
        }

        private static void ConnectRooms(Room room, string connection, Map map)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" Attempting to connect {room.Name} with {connection}");
            Console.ForegroundColor = ConsoleColor.Gray;

            var connectionType = map.ConnectRoom(room, connection);

            if (connectionType == ConnectionTypes.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Connection succesful");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (connectionType == ConnectionTypes.AlreadyCompleted)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Connection already exists.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Connection failed: No room with name {connection}.");
                Console.ForegroundColor = ConsoleColor.Gray;

                if (FailedConnections.ContainsKey(connection))
                {
                    FailedConnections[connection].Add(room.Name);
                }
                else
                {
                    var failed = new List<string>() { room.Name };
                    FailedConnections[connection] = failed;
                }
            }

        }

        static List<string> LoadFile(string mapName)
        {
            string path = mapName.Contains("\\")? mapName : Directory.GetCurrentDirectory() + $"/{mapName}";
            Console.WriteLine(path);
            var text = File.ReadAllLines(path).ToList();
            return text.Where(line => line.Length > 1).ToList();

        }
    }
}
