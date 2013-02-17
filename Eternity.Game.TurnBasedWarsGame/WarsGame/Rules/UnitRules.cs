using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Rules
{
    public class UnitRules
    {
        public UnitType UnitType { get; private set; }
        public UnitClassType ClassType { get; private set; }
        public List<WeaponRules> WeaponRules { get; private set; }

        public int Vision { get; private set; }
        public int Fuel { get; private set; }
        public int Price { get; private set; }
        public bool CanSupply { get; private set; }

        public int MovePoints { get; private set; }
        public UnitMoveType MoveType { get; private set; }

        public int LoadCapacity { get; private set; }
        public List<UnitClassType> LoadClassTypes { get; private set; }

        public bool CanBuildBuildings { get; private set; }
        public Dictionary<TileType, List<TileType>> BuildBuildings { get; private set; }
        public int BuildingMaterial { get; private set; }

        public bool CanBuildUnits { get; private set; }
        public List<UnitType> BuildUnits { get; private set; }
        public int UnitMaterial { get; private set; }

        public UnitRules(ResourceDefinition def)
        {
            UnitType ut;
            Enum.TryParse(def.GetData("Name"), out ut);
            UnitType = ut;
            UnitClassType ct;
            Enum.TryParse(def.GetData("ClassType"), out ct);
            ClassType = ct;
            Vision = int.Parse(def.GetData("Vision"));
            MovePoints = int.Parse(def.GetData("MovePoints"));
            UnitMoveType mt;
            Enum.TryParse(def.GetData("MoveType"), out mt);
            MoveType = mt;
            Fuel = int.Parse(def.GetData("Fuel"));
            Price = int.Parse(def.GetData("Price"));
            WeaponRules = def.ChildrenDefinitions
                .Where(x => x.DefinitionType == "Weapon")
                .Select(x => new WeaponRules(x))
                .ToList();
            LoadCapacity = 0;
            LoadClassTypes = new List<UnitClassType>();
            CanSupply = bool.Parse(def.GetData("CanSupply", "False"));
            var load = def.ChildrenDefinitions.FirstOrDefault(x => x.DefinitionType == "Load");
            if (load != null)
            {
                LoadCapacity = int.Parse(load.GetData("Capacity", "0"));
                var classTypes = load.GetData("ClassType", "").Split(' ');
                foreach (var classType in classTypes)
                {
                    if (Enum.TryParse(classType, out ct)) LoadClassTypes.Add(ct);
                }
            }
            CanBuildBuildings = CanBuildUnits = false;
            BuildingMaterial = UnitMaterial = 0;
            BuildBuildings = new Dictionary<TileType, List<TileType>>();
            BuildUnits = new List<UnitType>();
            foreach (var cons in def.ChildrenDefinitions.Where(x => x.DefinitionType == "Construct"))
            {
                var type = cons.GetData("Type");
                var material = int.Parse(cons.GetData("Material", "0"));
                if (type.ToLower().Equals("building"))
                {
                    CanBuildBuildings = true;
                    BuildingMaterial = material;
                    foreach (var tt in Enum.GetValues(typeof(TileType)).OfType<TileType>())
                    {
                        var val = def.GetData(tt.ToString());
                        if (!String.IsNullOrWhiteSpace(val))
                        {
                            BuildBuildings.Add(tt, new List<TileType>());
                            foreach (var spl in val.Split(' '))
                            {
                                TileType build;
                                if (Enum.TryParse(spl, true, out build))
                                {
                                    BuildBuildings[tt].Add(build);
                                }
                            }
                        }
                    }
                }
                else
                {
                    CanBuildUnits = true;
                    UnitMaterial = material;
                    foreach (var spl in cons.GetData("UnitType").Split(' '))
                    {
                        UnitType build;
                        if (Enum.TryParse(spl, true, out build))
                        {
                            BuildUnits.Add(build);
                        }
                    }
                }
            }
        }
    }
}
