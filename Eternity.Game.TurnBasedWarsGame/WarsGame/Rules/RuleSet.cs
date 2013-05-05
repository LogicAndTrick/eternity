using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Weapons;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Rules
{
    public static class RuleSet
    {
        public static ResourceDefinition CurrentRuleSet { get; private set; }

        private static Dictionary<TileType, TerrainRules> _terrainRules;
        private static Dictionary<UnitType, UnitRules> _unitRules;
        private static Dictionary<string, ArmyRules> _armyRules;
        private static DamageTable _damageTable;

        public static void SetCurrentRuleSet(string set)
        {
            CurrentRuleSet = ResourceManager.GetResourceDefinition("RuleSet", set);
            _terrainRules = new Dictionary<TileType, TerrainRules>();
            _unitRules = new Dictionary<UnitType, UnitRules>();
            _armyRules = new Dictionary<string, ArmyRules>();
            foreach (var cd in CurrentRuleSet.ChildrenDefinitions)
            {
                if (cd.DefinitionType == "Unit")
                {
                    var ur = new UnitRules(cd);
                    _unitRules.Add(ur.UnitType, ur);
                }
                else if (cd.DefinitionType == "Terrain")
                {
                    var tr = new TerrainRules(cd);
                    _terrainRules.Add(tr.TileType, tr);
                }
                else if (cd.DefinitionType == "Army")
                {
                    var ar = new ArmyRules(cd);
                    _armyRules.Add(ar.Name, ar);
                }
                else if (cd.DefinitionType == "Damage")
                {
                    _damageTable = new DamageTable(cd);
                }
            }
        }

        public static TerrainRules GetTerrainRules(TileType tt)
        {
            return _terrainRules[tt];
        }

        public static UnitRules GetUnitRules(UnitType ut)
        {
            return _unitRules[ut];
        }

        public static ArmyRules GetArmyRules(string army)
        {
            return _armyRules[army];
        }

        public static IEnumerable<ArmyRules> GetAllArmyRules()
        {
            return _armyRules.Select(x => x.Value);
        }

        public static DamageTable GetDamageTable()
        {
            return _damageTable;
        }

        public static bool CanAttack(UnitType attacker, UnitType defender, WeaponClassType weaponClass)
        {
            return _damageTable.CanAttack(attacker, defender, weaponClass);
        }

        public static int GetDamageValue(UnitType attacker, UnitType defender, WeaponClassType weaponClass)
        {
            return _damageTable.GetDamageValue(attacker, defender, weaponClass);
        }
    }
}