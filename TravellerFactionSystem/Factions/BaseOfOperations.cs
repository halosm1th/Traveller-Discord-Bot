using System.Collections.Generic;
using System.Linq;
using Traveller_subsector_generator;

namespace TravellerFactionSystem.Factions
{
    class BaseOfOperations : Asset
    {
        public new int TechLevel
        {
            get { return Assets.Sum(asset => asset.TechLevel); }
        }

        public Faction CreatorFaction { get; }

        public List<Asset> Assets { get; }

        public override void Update(Action action)
        {

        }

        public void BuyNewAsset()
        {

        }

        /// <summary>
        /// Determine if an asset should be sold.
        /// </summary>
        /// <returns>True means it should be sold, false means use default action</returns>
        public bool ShouldSellAsset(Asset asset)
        {
            return false;
        }

        public bool ShouldRepairAsset(Asset asset)
        {
            return false;
        }

    }
}