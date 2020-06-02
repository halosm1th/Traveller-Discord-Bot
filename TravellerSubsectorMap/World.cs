using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Traveller_subsector_generator
{
    class World
    {
        public bool HasWorld = false;

        public int X;
        public int Y;
        public string Name;
        public int StarportQuality;
        public int WorldSize;
        public int WorldAtmosphere;
        public int WorldHydrographics;
        public int Popuation;
        public int GovernmentType;
        public int LawLevel;
        public int TechLevel;
        public bool MilitaryBase;
        public bool GasGiant;
        public bool OtherBase;

        public string UWP
        {
            get
            {
                string retString = "";

                var qal = StarportQuality.ToString("X");
                var size = WorldSize.ToString("X");
                var atmo = WorldAtmosphere.ToString("X");
                var hydo = WorldHydrographics.ToString("X");
                var pop = Popuation.ToString("X");
                var gov = GovernmentType.ToString("X");
                var law = LawLevel.ToString("X");
                var tech = TechLevel.ToString("X");

                retString= String.Format(new NumberFormatInfo(),
                    "{0:X}{1:X}{2:X}{3:X}{4:X}{5:X}{6:X}-{7:X}",
                    qal,size,atmo,hydo,pop,gov,law,tech);

                return retString.ToUpper();
            }
        }

        public string Stations
        {
            get
            {
                var military = MilitaryBase ? "+" : " ";
                var other = OtherBase ? "*" : " ";
                var gas = GasGiant ? "O" : " ";

                return $"{other} {gas} {military}";
            }
        }

        private static Random die = new Random();

        private string convert(string text)
        {
            if (text.Contains(":"))
            {
                return text;
            }

            //Barninsville 3 2 C00F481-A Y Y N
            var parts = text.Split(' ');
            string name = parts[0];
            var x = parts[1];
            var y = parts[2];
            var code = parts[3];
            var research = parts[4];
            var gas = parts[5];
            var military = parts[6];


            return $"{name} {x}:{y} {code} {research}{gas}{military}";
        }

        public World(string worldText)
        {

            worldText = convert(worldText);
            //Example:Longleaf 1:2 a36b3f4-e YNY
            var parts = worldText.Split(' ');
            if (parts.Length < 4)
            {
                throw new ArgumentException("not enough args");
            }

            var name = parts[0];
            var location = parts[1];
            var code = parts[2];
            var stations = parts[3];

            var loc = location.Split(':');

            Name = name;
            X = Convert.ToInt32(loc[0])-1;
            Y = Convert.ToInt32(loc[1])-1;

            if (stations[0] == 'Y') GasGiant = true;
            if (stations[1] == 'Y') MilitaryBase = true;
            if (stations[2] == 'Y') OtherBase = true;

            for (int i = 0; i < code.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        if (code[i] == 'x' || code[i] == 'X')
                        {
                            StarportQuality = 15;
                        }

                        else
                        {
                            StarportQuality = int.Parse(code[i].ToString(), NumberStyles.HexNumber);
                        }
                        break;
                    case 1:
                        WorldSize = int.Parse(code[i].ToString(), NumberStyles.HexNumber);
                        break;
                    case 2:
                        WorldAtmosphere= int.Parse(code[i].ToString(), NumberStyles.HexNumber);
                        break;
                    case 3:
                        WorldHydrographics = int.Parse(code[i].ToString(), NumberStyles.HexNumber);
                        break;
                    case 4:
                        Popuation = int.Parse(code[i].ToString(), NumberStyles.HexNumber);
                        break;
                    case 5:
                        GovernmentType = int.Parse(code[i].ToString(), NumberStyles.HexNumber);
                        break;
                    case 6:
                        LawLevel = int.Parse(code[i].ToString(), NumberStyles.HexNumber);
                        break;
                    case 8:
                        var letter = code[i].ToString();
                        TechLevel = int.Parse(letter, NumberStyles.HexNumber);
                        break;
                }

            }

            HasWorld = true;
        }

        public World(int x, int y)
        {
            X = x;
            Y = y;
            HasWorld = false;
        }

        public World(int x, int y, string name)
        {
            X = x;
            Y = y;
            Name = name;
            HasWorld = true;
            GenerateWorld();
        }

        public string WorldData()
        {
            var sb  = new StringBuilder();
            return
                string.Format(
                    "Universal World Profile.\n------------------------\nName: {0}\nUWP: {1}\n------------------------\nStarport: {2}\nWorld Size: {3}\nWorld atmosphere: {4}\nWorld hydrographics: {5}\nPopulation: {6}\nGovernment Type: {7}\nLaw Level: {8}\nTech Level: {9}\n------------------------\nTravel Warning: {10}\nTrade Codes:\n{11}\n",
                    Name, UWP, StarportDescrption(), WorldSizeDescription(), WorldAtmosphereDescrpition(),
                    WorldHydrographicsDescription(), PopulationDescription(), GovernmentTypeDescrption(),
                    LawLevelDescription(), TechLevelDescription(), TravelWarning(), GetTradeCodes())
                ;
        }


        private int Roll2D6(int modifier = 0, int bottom = 2, int top = 13)
        {
            return die.Next(bottom, top) - modifier;
        }

        private int CalculateStarport()
        {
            int modifier = 0;

            if (Popuation >= 8)
            {
                modifier += 1;
            }
            else if (Popuation >= 10)
            {
                modifier += 2;
            }
            else if (Popuation <= 4)
            {
                modifier -= 1;
            }
            else if (Popuation <= 2)
            {
                modifier -= 2;
            }

            var result = Roll2D6(modifier);
            if (result <= 2) return 15;
            else if (result == 3 || result == 4) return 14;
            else if (result == 5 || result == 4) return 13;
            else if (result == 7 || result == 4) return 12;
            else if (result == 9 || result == 4) return 11;
            else return 10;
        }

        private int GetTechModifiers()
        {
            int modifier;
            switch (StarportQuality)
            {
                case 10:
                    modifier = 6;
                    break;
                case 11:
                    modifier = 4;
                    break;
                case 12:
                    modifier = 2;
                    break;
                case 15:
                    modifier = -4;
                    break;
                default:
                    modifier = 0;
                    break;
            }

            switch (WorldSize)
            {
                case 0:
                case 1:
                    modifier = 2;
                    break;
                case 3:
                case 4:
                    modifier = 1;
                    break;
                default:
                    modifier = 0;
                    break;
            }

            switch (WorldAtmosphere)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    modifier = 1;
                    break;
                default:
                    modifier = 0;
                    break;
            }

            switch (WorldHydrographics)
            {
                case 0:
                case 9:
                    modifier = 1;
                    break;
                case 10:
                    modifier = 2;
                    break;
                default:
                    modifier = 0;
                    break;
            }

            switch (Popuation)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 8:
                    modifier = 1;
                    break;
                case 9:
                    modifier = 2;
                    break;
                case 10:
                    modifier = 4;
                    break;
                default:
                    modifier = 0;
                    break;
            }

            switch (GovernmentType)
            {
                case 0:
                case 5:
                case 7:
                    modifier = 1;
                    break;
                case 13:
                case 14:
                    modifier = -2;
                    break;
                default:
                    modifier = 0;
                    break;
            }

            return 0-modifier;
        }

        private void GenerateWorld()
        {
            WorldSize = Math.Max(0,Roll2D6(2));
            WorldAtmosphere = Math.Max(0, Roll2D6(WorldSize - 7));
            WorldHydrographics = Math.Max(0, WorldAtmosphere <= 0 ? 0 : Roll2D6(WorldAtmosphere - 7));

            Popuation = Math.Max(0, Roll2D6(2));
            GovernmentType = Math.Max(0, Roll2D6(Popuation - 7));
            LawLevel = Math.Max(0, Roll2D6(GovernmentType - 7));

            StarportQuality = CalculateStarport();
            TechLevel = Math.Max(0, Roll2D6(GetTechModifiers(), 1, 7));

            GasGiant = Roll2D6()  <=  10;
            MilitaryBase = Roll2D6() >= 8;
            OtherBase = Roll2D6() >= 8;
        }

        private string StarportDescrption()
        {
            switch (StarportQuality)
            {
                case 10:
                    return
                        "Excellent Starport. 1D x Cr1000 to Berth. | Refined Fuel. | Shipyard(all) Repair Facilities. Check sheet for bases";
                case 11:
                    return
                        "Good Starport. 1D x Cr500 to Berth. | Refined Fuel. | Shipyard(Spacecraft) Repair Facilities. Check sheet for bases";
                case 12:
                    return
                        "Routine Starport. 1D x Cr100 to Berth. | Unrefined Fuel. | Shipyard(Small Craft) Repair Facilities. Check sheet for bases";
                case 13:
                    return
                        "Poor Starport. 1D x Cr10 to Berth. | Unrefined Fuel. | Limited Repair Facilities. Check sheet for bases";
                case 14:
                    return "Frontier Starport. 0Cr to Berth. | No Fuel. | No Repair Facilities. No bases.";
                default:
                    return "No Starport. No berthing Cost. | No facilities. | No bases";
            }
        }

        private string WorldSizeDescription()
        {
            switch (WorldSize)
            {
                case 0:
                    return "Size of Less then 1000KM. | Examples Asteroid, Orbital Complex. | Negligible Gravity";
                case 1:
                    return "Size of Roughly 1,600KM. | Example Triton. | 0.05 Gravity";
                case 2:
                    return "Size of Roughly 3,200KM. | Examples Luna, Europa. | 0.15 Gravity";
                case 3:
                    return "Size of Roughly 4,800KM. | Examples Mercury, Ganymede. | 0.25 Gravity";
                case 4:
                    return "Size of Roughly 6,400KM. | No listed example. | 0.35 Gravity";
                case 5:
                    return "Size of Roughly 8,000KM. | Example Mars. | 0.45 Gravity";
                case 6:
                    return "Size of Roughly 9,600KM. | No Listed examples. | 0.7 Gravity";
                case 7:
                    return "Size of Roughly 11,200KM. | Example Earth. 0.9 | Gravity";
                case 8:
                    return "Size of Roughly 12,800KM. | No Listed examples. | 1.0 Gravity";
                case 9:
                    return "Size of Roughly 14,400KM. | No Listed examples. | 1.25 Gravity";
                case 10:
                    return "Size of Roughly 16,000KM. | No Listed examples. | 1.4 Gravity";
                default:
                    return "Error";
            }
        }

        private string WorldAtmosphereDescrpition()
        {
            switch (WorldAtmosphere)
            {
                case 0:
                    return "Compostion: None. | Example Moon. | Pressure 0.00. | Survival Gear Required Vacc Suit.";
                case 1:
                    return
                        "Compostion: Trace. | Example Mars. | Pressure 0.01 to 0.09. | Survival Gear Required Vacc Suit.";
                case 2:
                    return
                        "Compostion: Very Thin, Tainted. | Example None. | Pressure 0.1 to 0.42. | Survival Gear Required Respirator, Filter/";
                case 3:
                    return
                        "Compostion: Very Thin. | Example None. | Pressure 0.1 to 0.42. | Survival Gear Required Respirator.";
                case 4:
                    return
                        "Compostion: Thin, Tainted. | Example None. | Pressure 0.43 to 0.7. | Survival Gear Required Filter.";
                case 5:
                    return "Compostion: Thin. | Example None. | Pressure 0.43 to 0.7. | Survival Gear Required None.";
                case 6:
                    return
                        "Compostion: Standard. | Example Earth. | Pressure 0.71-1.49. | Survival Gear Required None.";
                case 7:
                    return
                        "Compostion: Standard, Tainted. | Example None. | Pressure 0.71-1.49. | Survival Gear Required Filter.";
                case 8:
                    return
                        "Compostion: Dense, Tainted. | Example None. | Pressure 1.5-2.49. | Survival Gear Required None.";
                case 9:
                    return
                        "Compostion: Dense, Tainted. | Example None. | Pressure 1.5-2.49. | Survival Gear Required Filter.";
                case 10:
                    return
                        "Compostion: Exotic. | Example None. | Pressure Varies. | Survival Gear Required Air Supply..";
                case 11:
                    return
                        "Compostion: Corrosive. | Example Venus. Pressure Varies. | Survival Gear Required Vacc Suit.";
                case 12:
                    return
                        "Compostion: Insidious. | Example None. Pressure Varies. | Survival Gear Required Vacc Suit.";
                case 13:
                    return "Compostion: Very Dense. | Example None. | Pressure 2.5+. | Survival Gear Required None";
                case 14:
                    return "Compostion: Low. | Example None. | Pressure 0.5 or less. | Survival Gear Required None.";
                case 15:
                    return "Compostion: Unusual. | Example None. | Pressure Varies. | Survival Gear Required Varies";
                default:
                    return "Error";
            }
        }

        private string WorldHydrographicsDescription()
        {
            switch (WorldHydrographics)
            {
                case 0:
                    return "0-5%. | Desert World";
                case 1:
                    return "5-15%. | Dry World";
                case 2:
                    return "16-25%. | A few small seas.";
                case 3:
                    return "25-35%. | Small seas and ocean worlds.";
                case 4:
                    return "36-45%. | Wet World";
                case 5:
                    return "46-55%. | Large Oceans.";
                case 6:
                    return "56-65%. | Fucking Huge Oceans.";
                case 7:
                    return "66-75%. | Earth-Like world.";
                case 8:
                    return "76-85%. | Water World";
                case 9:
                    return "86-95%. | Only a few small Islands and archipelagos.";
                case 10:
                    return "96-100%. | Almost Entirely Water";
            }

            return "Error";
        }

        private string PopulationDescription()
        {
            var r = new Random();
            var size = "";
            size += r.Next(1, 10);
            for (int i = 0; i < Popuation; i++)
            {
                size += "0";
            }

            return size;
        }

        private string GovernmentTypeDescrption()
        {
            switch (GovernmentType)
            {
                case 0:
                    return
                        "None. | No Government stucture, in many cases family bonds predominate. | Examples: Family, clan, anarchy. | No contraband";
                case 1:
                    return
                        "Company/Corporation. | Ruling functions are assumed by a copmany managerial elite, and most citzenry are company employees or dependants. | Examples: Corporate outpost, asteroid mine, feudal domain. | Contraband: Weapons, Drugs, Travellers.";
                case 2:
                    return
                        "Participating Democracy. | Ruling function are reach by the advice and consent of the citzenry directly. | Collective, tribval council, commlinked consensus. | Common contraband includes drugs.";
                case 3:
                    return
                        "Self-Perpetuating Oligarchy. | Ruling functions are performed by a restricted minority, with little or no input from the mass of citizenry. | Plutocracy, hereditary ruling caste. | Trachnology, weapons, travellers.";
                case 4:
                    return
                        "Representative Democracy. | Ruling functions are performed by elected representatives. | Republic, democracy. | Drugs, Weapons, Psiconics.";
                case 5:
                    return
                        "Feudal Technocracy. | Ruling functions are performed by specific individuals for persons who agree to be ruled by them. Relationships are based on the performance of technical activities which are mutually beneficial. | Those with accesss to higher technology tend to have higher social status. | Technology, weapons, computers.";
                case 6:
                    return
                        "Captive Government. | Ruling functions are performed by an imposed leadership answerable to an outside group. | A colony or conquored area. | Contraband includes weapons, technology, travellers.";
                case 7:
                    return
                        "Balkanisation. | No central authority exists; rival governments complete for control. Law level refers to the government nearest the starport. | Multiple governments, civil war. | Various contraband.";
                case 8:
                    return
                        "Civil Service Bureaucracy. | Ruling functions are performed by government agencies employing individuals selected for their expertise. | Examples: Technocracy, Communism. | Contraband includes drugs and weapons.";
                case 9:
                    return
                        "Impersonal Bureaucracy. | Ruling functions are performed by agencies which have become insulated from the governed citizens. | Entrenched castes of bureacrats. Decaying empire. | Contraband includes Technology, weapons, drugs, travellers, psiconics.";
                case 10:
                    return
                        "Charismatic Dictator. | Ruling functions are performed by agencies directed by a single leader who enjoys the overwhelming confidence of the citizens. | Revolutionary leader, messiah, emperor. | No contraband";
                case 11:
                    return
                        "Non-Charismatic Leader. | A previous charismatic dictator has been replaced by a leader through normal channels. | Military dictatorship, hereditary kingship. | Contraband: weapons, technology, computers.";
                case 12:
                    return
                        "Charismatic Oligarchy. | Ruling functions are performed by a select group of members of an organisation or class which enjoys the overwhelming confidence of the citizenry. | Examples: Junta, revolution council. | Contraband: weapons";
                case 13:
                    return
                        "Religious Dicatorship. | Ruling functions are performed by a religious organisation without regard to the specific individual needs of the citizenry. | Examples: Cult, transcendent philosophy, psionic gorup mind. | Contraband varies";
                case 14:
                    return
                        "Religious Autocracy. | Government by a single religious leader having absolute power over the citizenry. | Example: Messiah. | Various contraband";
                case 15:
                    return
                        "Totalitarian Oligarchy. | Government by an all-powerful minority which maintains absolute control through widespread coercion and oppresion. | Examples: World church, ruthless corporation. | Varies";
                default:
                    return "No idea.";
            }
        }

        private string LawLevelDescription()
        {
            switch (LawLevel)
            {
                case 0:
                    return "Weapons Banned: No restrictions - Heavy armour and a handy weapon are commended...";
                case 1:
                    return
                        "Weapons Banned: Poison gas, explosives, undetectable weapons, WMD. | Armour Banned: Battle dress.";
                case 2:
                    return "Weapons Banned: Portable energy and laser weapons. | Armour Banned: Combat armour.";
                case 3:
                    return "Weapons Banned: Military Weapons. | Armour Banned: Flak.";
                case 4:
                    return "Weapons Banned: Light assault weapons and submachine guns. | Armour Banned: Cloth.";
                case 5:
                    return "Weapons Banned: Personal concealable weapons. | Armour Banned: Mesh.";
                case 6:
                    return
                        "Weapons Banned: All firearms except shotguns & stunners; carrying weapons discouraged. | Armour Banned: No New Items.";
                case 7:
                    return "Weapons Banned: Shotguns. | Armour Banned: No New Items.";
                case 8:
                    return "Weapons Banned: All bladed weapons, stunners. | Armour Banned: All visible armour.";
                default:
                    return "Weapons Banned: All Weapons. | Armour Banned: All armour.";
            }
        }

        private string TechLevelDescription()
        {
            switch (TechLevel)
            {
                case 0:
                    return
                        "(Primitive): No technology. TL 0 species have only discovered the simplest tools and principles, and are on a par with Earth’s Stone Age.";
                case 1:
                    return
                        "(Primitive): Roughly on a par with Bronze or Iron age technology. TL 1 science is mostly superstition, but they can manufacture weapons and work metals.";
                case 2:
                    return
                        "(Primitive): Renaissance technology. TL 2 brings with it a greater understanding of chemistry, physics, biology and astronomy as well as the scientific method. ";
                case 3:
                    return
                        "(Primitive): The advances of TL 2 are now applied, bringing the germ of industrial revolution and steam power. Primitive firearms now dominate the battlefield. This is roughly comparable to the early 19th century.";
                case 4:
                    return
                        "(Industrial): The transition to industrial revolution is complete, bringing plastics, radio and other such inventions. Roughly comparable to the late 19th/early 20th century.";
                case 5:
                    return
                        "(Industrial): TL 5 brings widespread electrification, tele-communications and internal combustion. At the high end of the TL, atomics and primitive computing appear. Roughly on a par with the mid–20th century.";
                case 6:
                    return
                        "(Industrial): TL 6 brings the development of fission power and more advanced computing. Advances in materials technology and rocketry bring about the dawn of the space age. ";
                case 7:
                    return
                        "(Pre-Stellar): A pre-stellar society can reach orbit reliably and has telecommunications satellites. Computers become common. At the time of writing, humanity is currently somewhere between TL 7 and TL 8.";
                case 8:
                    return
                        "(Pre-Stellar): At TL 8, it is possible to reach other worlds in the same system, although terraforming or full colonisation are not within the culture’s capacity. Permanent space habitats become possible. Fusion power becomes commercially viable.";
                case 9:
                    return
                        "(Pre-Stellar): The defining element of TL 9 is the development of gravity manipulation, which makes space travel vastly safer and faster. This research leads to development of the jump drive, which occurs near the end of this Tech Level. TL 9 cultures can colonise other worlds, although travelling to a colony is often a one-way trip.";
                case 10:
                    return
                        "(Early Stellar): With the advent of commonly available jump drives, nearby systems are opened up. Orbital habitats and factories become common. Interstellar travel and trade lead to an economic boom. Colonies become much more viable.";
                case 11:
                    return
                        "(Early Stellar): The first true artificial intelligences become possible, as computers are able to model synaptic networks. Grav-supported structures reach to the heavens. Jump 2 travel becomes possible, allowing easier travel beyond the one jump stellar mains. ";
                case 12:
                    return
                        "(Average Stellar): Weather control revolutionises terraforming and agriculture. Man-portable plasma weapons and carrier-mounted fusion guns make the battlefield untenable for unarmoured combatants. Jump 3 travel is developed. ";
                case 13:
                    return
                        "(Average Stellar): The battle dress appears on the battlefield in response to the new weapons. Cloning of body parts becomes easy. Advances in hull design and thruster plates means that spacecraft can easily go underwater. Jump 4 travel.";
                case 14:
                    return
                        "(Average Stellar): Fusion weapons become man-portable. Flying cities appear. Jump 5 travel.";
                case 15:
                    return
                        " (High Stellar): Black globe generators suggest a new direction for defensive technologies, while the development of synthetic anagathics means that the human lifespan is now vastly increased. Jump 6 travel.";
                default:
                    return "I didn't code this far";
            }
        }
        private string GetTradeCodes()
        {
            var tradeCodes = new List<Func<string>>()
            {
                () =>
                {
                    return ((WorldAtmosphere > 3 && WorldAtmosphere < 9)
                        && (WorldHydrographics > 3 && WorldHydrographics < 9)
                        && (Popuation > 4 && Popuation < 8))? "(Ag)riculture: Dedicated to farming and food production. Often, they are divided into vast semi-feudal estates." : "";
                },
                () =>
                { return
                    (WorldAtmosphere == 0 && WorldAtmosphere == 0 && WorldHydrographics == 0)? "(As)teroids: Usually mining colonies, but can also be orbital factories or colonies." : ""; },
                () => { return (Popuation == 0 && GovernmentType == 0 && LawLevel == 0)? "(Ba)rren: Uncolonised and empty." : ""; },
                () => { return (WorldAtmosphere >= 2 && WorldHydrographics == 0)? "(De)sert: Dry and barely habitable" : ""; },
                () => { return (WorldAtmosphere >= 10 && WorldHydrographics >= 1)? "(Fl)uid Oceans: Worlds where the surface liquid is something other than water, and so are incompatible with Earth-derived life" : ""; },
                () => { return (
                    (WorldSize > 5 && WorldSize < 9)
                    && (WorldAtmosphere == 5 || WorldAtmosphere == 6 || WorldAtmosphere == 8)
                    && (WorldHydrographics >=5 && WorldHydrographics <= 7))? "(Ga)rden: Worlds that are like earth" : ""; },
                () => { return (Popuation >= 9)? "(Hi)gh population: A population in the billions" : ""; },
                () => { return (TechLevel >= 12)? "(Ht)High Tech: Among the most technologically advanced in Charted Space" : ""; },
                () => { return (
                    (WorldAtmosphere == 0 || WorldAtmosphere == 1)
                    && WorldHydrographics > 1)? "(Ie)Ice-Capped: Worlds that have most of their surface liquid frozen in polar ice caps, and are cold and dry." : ""; },
                () => { return (Popuation >= 9 &&
                                ((WorldAtmosphere >= 0 && WorldAtmosphere <=2)
                                    || WorldAtmosphere == 4 || WorldAtmosphere == 7 || WorldAtmosphere == 9))? "(In)dustrial: Dominated by factories and cities." : ""; },
                () => { return (Popuation <=3)? "(Lo)w population: A population of only a few thousand or less." : ""; },
                () => { return (TechLevel <=5)? "(Lt)Low tech: Pre-industrial and cannot produce advanced goods." : ""; },
                () => { return ((WorldAtmosphere >= 0 && WorldAtmosphere <=3)
                        && (WorldHydrographics >= 0 && WorldHydrographics <= 3)
                        && Popuation >= 6)? "(Na) Non-Agricultural: Too dry or barren to support their populations using conventional food production." : ""; },
                () => { return (Popuation <=6 && Popuation >= 0)? "(Ni) Non-Industrial: Too low in population to maintain an extensive industrial base. " : ""; },
                () => { return (
                    (WorldAtmosphere >=2 && WorldAtmosphere <=5)
                    && (WorldHydrographics >= 0 && WorldHydrographics <= 3))? "(Po)or: Lacking resources, viable land or sufficient population to be anything other than marginal colonies." : ""; },
                () => { return (
                        (WorldAtmosphere == 6 || WorldAtmosphere == 8)
                        && (Popuation >= 6 && Popuation <=8)
                        && (GovernmentType >= 4 && GovernmentType <= 9))? "(Ri)ch: Blessed with a stable government and viable." : ""; },
                () => { return (WorldAtmosphere == 0)? "(Va)cuum: No atmosphere." : ""; },
                () => { return (WorldHydrographics >= 10)? "(Wa)ter World: Almost entirely water-ocean across their surface." : ""; },
            };

            var tradeCode = new StringBuilder();
            foreach (var trade in tradeCodes)
            {
                var result = trade();
                if (result != "") tradeCode.Append(trade() + "\n");
            }

            return tradeCode.ToString();
        }

        private bool TravelWarning()
        {
            return WorldAtmosphere > 10
                   || (GovernmentType == 0 || GovernmentType == 7 || GovernmentType == 10)
                   || (LawLevel == 0 || LawLevel >= 9);
        }

        public override string ToString()
        {
            var gas = GasGiant ? 'Y' : 'N';
            var mil = MilitaryBase ? 'Y' : 'N';
            var oth = OtherBase ? 'Y' : 'N';
            var sq = StarportQuality.ToString("x");
            var ws = WorldSize.ToString("x");
            return $"{Name} {X+1} {Y+1}: {UWP} " +
                   $"Gas: {gas} Military: {mil} Other: {oth}";
        }
    }
}
