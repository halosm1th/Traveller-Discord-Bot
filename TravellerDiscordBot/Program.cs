using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Traveller_subsector_generator;
using Exception = System.Exception;

namespace TravellerDiscordBot
{
    class Program
    {
        private DiscordSocketClient _client;

        private readonly Random _random = new Random();

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            var token = File.ReadAllLines("M:\\Code\\token.txt")[0];
            await _client.LoginAsync(TokenType.Bot,token);
                //todo put this in enviroment variable
                await _client.StartAsync();

            _client.MessageReceived += MessageReceived;

            await Task.Delay(-1);
        }


        private async Task<string> GetData(List<string> words)
        {
            var message = new StringBuilder();
            
            for (int i = 1; i < words.Count; i++)
            {
                if (i > 1)
                {
                    message.Append("#"+words[i]);
                }
                else
                {
                    message.Append(words[i]);
                }
            }

            return message.ToString();
        }

        enum RollParts
        {
            TimesToRoll,SidesOnDice,NumberOfDice,ModifiesUp,ModifiersDown,ModifiersMul,ModifiersDiv
        }

        private async Task RollDice(SocketMessage message)
        {

            var data = GetData(message.Content.ToLower().Split(" ").ToList()).Result;

            var result = await RollResult(data);

            var outputResult = result.Item1;
            var totals = result.Item2;
            var outTotal = "";
            foreach (var v in totals)
            {
                outTotal += $" {v}";
            }
            await message.Channel.SendMessageAsync($"{message.Author.Username} rolls: `[{outputResult}]={outTotal}`");
        }

        private async Task<(StringBuilder, List<int>)> RollResult(string data)
        {
            var numberOfDiceToRoll = 1;
            var numberOfSidesOnDice = 6;
            var numberOfTimesToRollDice = 1;
            var modifiers = 0;

            var specialCharacters =
                new Dictionary<RollParts, char>()
                {
                    {RollParts.NumberOfDice, 'd'},
                    {RollParts.TimesToRoll, ':'},
                    {RollParts.SidesOnDice, '#'},
                    {RollParts.ModifiesUp, '+'},
                    {RollParts.ModifiersDown, '-'},
                    {RollParts.ModifiersMul, '*'},
                    {RollParts.ModifiersDiv, '/'},
                };

            var workingResult = "";
            for (int i = 0; i < data.Length; i++)
            {
                var current = data[i];
                if (current == specialCharacters[RollParts.TimesToRoll])
                {
                    numberOfTimesToRollDice = GetValidNumber(ref workingResult);
                }
                else if (current == specialCharacters[RollParts.NumberOfDice])
                {
                    numberOfDiceToRoll = GetValidNumber(ref workingResult);
                }
                else if (current == specialCharacters[RollParts.SidesOnDice])
                {
                    if(workingResult != "") numberOfSidesOnDice = GetValidNumber(ref workingResult);
                }
                else if (current == specialCharacters[RollParts.ModifiesUp])
                {
                    modifiers += GetValidNumber(ref workingResult);
                }
                else if (current == specialCharacters[RollParts.ModifiersDown])
                {
                    modifiers -= GetValidNumber(ref workingResult);
                }
                else if (current == specialCharacters[RollParts.ModifiersMul])
                {
                    modifiers *= GetValidNumber(ref workingResult);
                }
                else if (current == specialCharacters[RollParts.ModifiersDown])
                {
                    modifiers /= GetValidNumber(ref workingResult);
                }
                else
                {
                    workingResult += current;
                }
            }

            var outputResult = await GetDiceOutput(numberOfTimesToRollDice, numberOfDiceToRoll, numberOfSidesOnDice, modifiers);
            await Log($"from: {data} to: {outputResult.Item1}");
            return outputResult;
        }

        private static int GetValidNumber(ref string workingResult)
        {
            int numberOfDiceToRoll;
            var valid = int.TryParse(workingResult, out numberOfDiceToRoll);
            if (!valid)
            {
                throw new ArgumentException($"{workingResult} is not a valid number!");
            }

            workingResult = "";
            return numberOfDiceToRoll;
        }

        private async Task<(StringBuilder,List<int>)> GetDiceOutput(int numberOfTimesToRollDice, int numberOfDiceToRoll,
            int numberOfSidesOnDice,int modifiers)
        {
            var totals = new List<int>();
            var total = 0;
            var results = new List<string>();
            for (int i = 0; i < numberOfTimesToRollDice; i++)
            {
                var output = "(";
                for (int j = 0; j < numberOfDiceToRoll; j++)
                {
                    var comma = j > 0 ? "," : "";
                    var result = _random.Next(1, numberOfSidesOnDice + 1);
                    total += result;
                    output += $"{comma}{result}";
                }

                var totalAfter = total + modifiers;
                output += $")={total}[+{modifiers}]={totalAfter}";
                totals.Add(totalAfter);
                total = 0;

                results.Add(output);
            }

            var outputResult = new StringBuilder();
            for (var i =0; i <  results.Count; i++)
            {
                outputResult.Append(results[i]);
                if(i != results.Count-1) outputResult.Append("\n");
            }

            return (outputResult,totals);
        }

        private async Task<string> DecodeWorldProfile(string worldName, string WorldCode)
        {
            World w = new World(worldName,WorldCode);
            return w.WorldData();
        }

        private async Task DecodeUWP(SocketMessage message)
        {
            var parts = message.Content.Split(" ");
            var profile = await DecodeWorldProfile(parts[1], parts[2]);
            await message.Channel.SendMessageAsync(profile);
        }

        private async Task DecodeUPP(SocketMessage message)
        {

            var data = message.Content.Split(":");
            var stats = new int[6];
            
            var name = data[0].Remove(0,4);
            var statsStart = data[1];

            for (int i = 0; i < statsStart.Length; i++)
            {
                if (statsStart[i] != '\0')
                {
                    var num = 0;
                    var letter = statsStart[i];
                    Console.WriteLine(letter.ToString());
                    if (int.TryParse(letter.ToString(), NumberStyles.AllowHexSpecifier, null, out num))
                    {
                        stats[i] = num;
                    }

                    else
                    {
                        await message.Channel.SendMessageAsync($"That is not a valid UPP. [Error with: {stats[i]}]" +
                                                               " UPP should be formatted as so: " +
                                                               "`NAME:ABCDEF`" +
                                                               " where ABCDEF are the hex numbers for the persons stats");
                        return;
                    }
                }
            }

            var mods = await CalcMods(stats);
            var messageOut = $"{name}- Str: {stats[0]}({mods[0]}), Dex: {stats[1]}({mods[1]}), End: {stats[2]}({mods[2]})," +
                             $"Int: {stats[3]}({mods[3]}), Edu: {stats[4]}({mods[4]}), Soc: {stats[5]}({mods[5]})";
            await message.Channel.SendMessageAsync(messageOut);
        }

        private async Task<int[]> CalcMods(int[] stats)
        {
            var results = new List<int>();
            foreach (var s in stats)
            {
                results.Add(CalculateMod(s));
            }

            return results.ToArray();
        }

        private int CalculateMod(int number)
        {
            if (number < 0)
            {
                return -4;
            }

            switch (number)
            {
                case 0: return -3;
                case 1:
                case 2:
                    return -2;
                case 3:
                case 4:
                case 5:
                    return -1;
                case 6:
                case 7:
                case 8:
                    return 0;
                case 9:
                case 10:
                case 11:
                    return 1;
                case 12:
                case 13:
                case 14:
                    return 2;
                default: return 3;
            }
        }

        private async Task MessageReceived(SocketMessage message)
        {
            try
            {
                if (message.Content.ToLower().Contains("!r") || message.Content.ToLower().Contains("!roll"))
                {
                    await RollDice(message);
                }
                else if (message.Content.ToLower().StartsWith("!uwp"))
                {
                    await DecodeUWP(message);
                }
                else if (message.Content.ToLower().StartsWith("!upp"))
                {
                    await DecodeUPP(message);
                }
                else if (message.Content.ToLower().StartsWith("!help"))
                {
                    await message.Channel.SendMessageAsync(
                        $"\nValid Commands are: r | uwp | upp" +
                        $"\n(r)oll [NumberOfTimesToRoll](NumberOfDice)D[Amount#] [postfix modifiers] | Roll some dice" +
                        $"\nexample: !r 5:2D 1+3-" +
                        $"\n(U)niversal World Profile (UWP) | Decode a universal world profile" +
                        $"\nexample: !uwp Acadie  C868563-9" +
                        $"\n(U)niversal Person Profile (UPP) | Decode a universal person profile" +
                        $"\nexample: !upp Ninno:8A8944");
                }
            }
            catch (Exception e)
            {
                await message.Channel.SendMessageAsync(e.Message);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task Log(string msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}
