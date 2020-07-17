using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Traveller_subsector_generator;
using TravellerFactionSystem.Person;

namespace TravellerFactionSystem.Factions
{
    enum FactionTags
    {

    }

    enum FactionSizes
    {

    }

    class Faction
    {
        public string Name { get; }
        public World HomeWorld { get; private set; }
        public List<World> Worlds { get; }
        public List<FactionTags> FactionTags { get; }
        public FactionSizes Size { get; private set; }
        public int Health { get; private set; }
        public IPerson Leader { get; private set; }
        public int Credits { get; private set; }
        public int WealthRating { get; private set; }
        public int ForceRating { get; private set; }
        public int CunningRating { get; private set; }
        public Levels level { get; private set; }
        public int CurrentExperience { get; private set; }
        public int TechLevel
        {
            get
            {
                return Worlds.Sum(world
                    => world.BasesOfOperations.First(baseOfOp 
                        => baseOfOp.CreatorFaction == this).TechLevel);
            }
        }

        public Action CurrentAction => PastActions.Peek();
        public Stack<Action> PastActions { get; }
        public List<Asset> ControlledAssets { get; }

        public IEnumerable<ForceAsset> ForceAssets => ControlledAssets.OfType<ForceAsset>();
        public IEnumerable<WealthAsset> WealthAssets => ControlledAssets.OfType<WealthAsset>();
        public IEnumerable<CunningAsset> CunningAssets => ControlledAssets.OfType<CunningAsset>();

        private Action GenerateNextAction()
        {
            var rand = new Random();
            var action = (Actions) rand.Next(0, Enum.GetValues(typeof(Actions)).Length);
            return new Action(action);
        }

        public void GetPayed(int amount = 0)
        {
            Credits += amount;
        }

        public void Update()
        {
            //If we have finished the last action, generate a new one
            if (CurrentAction.Finished)
            {
                PastActions.Push(GenerateNextAction());
            }

            //Either way though, we need to update the asset with the action
            foreach (var asset in ControlledAssets)
            {
                asset.Update(CurrentAction);
            }
        }

    }

    enum Levels
    {

    }
}
