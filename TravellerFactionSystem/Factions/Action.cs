using System;

namespace TravellerFactionSystem.Factions
{

    enum Actions
    {
        BuyAsset,
        SellAsset,
        RepairAsset,
        UpgradeAsset,
        DowngradeAsset,
        MoveAsset,
        ChangeAsset,
        Attack,
        ExpandInfluence,
        MoveHomeworld,
        SiezePlanet,
        UseAsset
    }
    class Action
    {
        public Actions ActionName { get; }
        public int TimeSoFar { get; private set; }
        public int TotalActionTime { get; private set; }
        public bool Finished { get; private set; }
        private System.Action<Asset> _action;

        public Action(Actions name)
        {
            ActionName = name;
            TimeSoFar = 0;
            Finished = false;
            _action = GenerateActionAndAssignTotalTime();
        }

        public Action<Asset> GenerateActionAndAssignTotalTime()
        {
            var random = new Random();
            switch (ActionName)
            {
                //Assets themselves can not buy new assets, but bases of operation can
                case Actions.BuyAsset:
                    TotalActionTime = random.Next(1,4);
                    return asset =>
                    {
                        if (asset.GetType() == typeof(BaseOfOperations))
                        {
                            ((BaseOfOperations) asset).BuyNewAsset();
                        }
                        asset.Update(asset.DefaultAction);
                    };
                case Actions.SellAsset:
                    TotalActionTime = random.Next(1,6);
                    return asset =>
                    {
                        if (asset.ControllingBase.ShouldSellAsset(asset))
                        {
                            asset.Sell();
                        }
                        else
                        {
                            asset.Update(asset.DefaultAction);
                        }
                    };
                case Actions.RepairAsset:
                    TotalActionTime = random.Next(2, 11);
                    return asset =>
                    {
                        if (asset.ControllingBase.ShouldRepairAsset(asset))
                        {
                            asset.Repair();
                        }
                    };
                case Actions.UpgradeAsset:
                    TotalActionTime = random.Next(5, 21);
                    return asset => { };
                case Actions.DowngradeAsset:
                    TotalActionTime = random.Next(2, 7);
                    return asset => { };
                case Actions.MoveAsset:
                    return asset => { TotalActionTime = asset.CalculateTotalTransitTime();};
                case Actions.ChangeAsset:
                    TotalActionTime = random.Next(5, 21);
                    return asset => { };
                case Actions.Attack:
                    TotalActionTime = random.Next(1, 51);
                    return asset => { };
                case Actions.ExpandInfluence:
                    TotalActionTime = random.Next(2, 46);
                    return asset => { };
                case Actions.MoveHomeworld:
                    return asset => { TotalActionTime = asset.CalculateTotalTransitTime();};
                case Actions.SiezePlanet:
                    TotalActionTime = random.Next(1, 30);
                    return asset => { };
                case Actions.UseAsset:
                    TotalActionTime = 1;
                    return asset =>
                    { };
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return asset =>
            {
                asset.Update(asset.DefaultAction);
            };
        }

        public void Update(Asset asset)
        {
            if (TimeSoFar < TotalActionTime)
            {
                _action(asset);
                TimeSoFar++;
            }
            else
            {
                Finished = true;
            }
        }
    }
}