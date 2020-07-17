using System;
using System.Collections.Generic;
using System.Text;

namespace TravellerHtmlGenerators
{
    class UniversalisConfederationTableGenerator
    {
        public string Name { get; }
        public string htmlReference { get; }

        public string MilitaryTotal { get; }
        public string NavyTotal { get; }
        public string ArmyTotal { get; }


        public string EconomicTotal { get; }
        public string TradeVol { get; }
        public string UCCL { get; }

        public string ExternalTotal { get; }
        public string TradeWithOthers { get; }
        public string UCPOwer { get; }

        public string InteralTotal { get; }
        public string CitizenControl { get; }
        public string ExtraCitizenController { get; }
        
        public string TechLevel { get; }
        public string AverageTechleve { get; }
        public string HighestTechLevel { get; }

        public UniversalisConfederationTableGenerator(string name, string htmlReference, string militaryTotal, string navyTotal, string armyTotal, string economicTotal, string tradeVol, string uccl, string externalTotal, string tradeWithOthers, string ucpOwer, string interalTotal, string citizenControl, string extraCitizenController, string techLevel, string averageTechleve, string highestTechLevel)
        {
            Name = name;
            this.htmlReference = htmlReference;
            MilitaryTotal = militaryTotal;
            NavyTotal = navyTotal;
            ArmyTotal = armyTotal;
            EconomicTotal = economicTotal;
            TradeVol = tradeVol;
            UCCL = uccl;
            ExternalTotal = externalTotal;
            TradeWithOthers = tradeWithOthers;
            UCPOwer = ucpOwer;
            InteralTotal = interalTotal;
            CitizenControl = citizenControl;
            ExtraCitizenController = extraCitizenController;
            TechLevel = techLevel;
            AverageTechleve = averageTechleve;
            HighestTechLevel = highestTechLevel;
        }


        private static string Ask(string question)
        {
            Console.WriteLine(question);
            return Console.ReadLine();
        }
        public static UniversalisConfederationTableGenerator CreateConfederationTableGenerator()
        {
            var name = Ask("Nation name");
            var refr = Ask("html reference?");
            var militaryTotal = Ask("nation military total?");
            var army = Ask("Army power?");
            var navy = Ask("Navy power?");
            var economic = Ask("Economic total?");
            var tradvol = Ask("Trade Volitility Index?");
            var uccl = Ask("UCCL?");
            var external = Ask("External Total?");
            var extrade = Ask("External trade?");
            var ucpower = Ask("how much power in the uc?");
            var interalpower = Ask("how much interal power?");
            var cit = Ask("How much control over citizens?");
            var ece = Ask("How much control over comps?");
            var tl = Ask("Tech level?");
            var avtl = Ask("Average tech level");
            var bstl = Ask("best tl");

            return new UniversalisConfederationTableGenerator(name,refr,militaryTotal,navy,army,
                economic,tradvol,uccl,external,extrade,ucpower,interalpower,
                cit,ece,tl,avtl,bstl);
        }

        public string ToHtml()
        {
            return $@"<tr id=""tableInfo""><td><a href=""{htmlReference}.html"" style=""color:white""> {Name} </a></td><td>{MilitaryTotal} ( {NavyTotal} , {ArmyTotal} ) </td><td> {EconomicTotal} ( {TradeVol} , {UCCL} ) </td><td> {ExternalTotal} ( {TradeWithOthers} , {UCPOwer} ) </td><td> {InteralTotal} ( {CitizenControl} , {ExtraCitizenController} ) </td><td> {TechLevel} ( {AverageTechleve} , {HighestTechLevel} ) </td></tr>";
        }
    }
}
