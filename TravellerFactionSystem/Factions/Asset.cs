using System;
using Traveller_subsector_generator;

namespace TravellerFactionSystem.Factions
{
    class Attack
    {

    }

    class Damage
    {

    }

    enum AssetTypes
    {

    }

    enum Orders
    {

    }
    abstract class Asset
    {
        public int TechLevel { get; }
        public int Cost { get; }
        public int CurrentHealth { get; protected set; }
        public int MaxHealth { get; private set; }

        public Attack AssetAttack { get; }
        public Damage CounterAttack { get; }
        public AssetTypes AssetType { get; }
        public Orders Order { get; }

        public Action DefaultAction { get; }
        public BaseOfOperations ControllingBase { get; private set; }

        public Faction AssetFaction => ControllingBase.CreatorFaction;
        //The world it is on can change
        public World WorldOn { get; private set; }

        public int CalculateTotalTransitTime()
        {
            return 0;
        }

        /// <summary>
        /// Check if an asset requires any subassets to be used.
        /// </summary>
        /// <returns>By default returns true</returns>
        public virtual bool HasRequiredAssets()
        {
            return true;
        }

        public virtual void Repair()
        {
            var rand = new Random();
            var repairAmount = rand.Next(1, MaxHealth - CurrentHealth);
            CurrentHealth += repairAmount;
        }

        public virtual void MoveAsset(World newWorld)
        {
            WorldOn = newWorld;
        }

        public virtual void Sell()
        {
            //Lowest you can get is half of the original price.
            AssetFaction.GetPayed(Cost / (17-TechLevel));
            ControllingBase = null;
        }

        public virtual void Update(Action action)
        {
            action.Update(this);
        }
    }
}