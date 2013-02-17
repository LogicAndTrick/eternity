using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.COs;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Armies
{
    public class Army
    {
        public ArmyRules ArmyRules { get; private set; }
        public CO CO { get; private set; }
        public List<Unit> Units { get; private set; }
        public List<Unit> DeadUnits { get; private set; }
        public int FundsAvailable { get; private set; }
        public int FundsSpent { get; private set; }
        public int DamageDealt { get; private set; }
        public int DamageTaken { get; private set; }

        public Army(CO co, string army)
        {
            ArmyRules = RuleSet.GetArmyRules(army);
            CO = co;
            Units = new List<Unit>();
            DeadUnits = new List<Unit>();
            FundsAvailable = FundsSpent = DamageDealt = DamageTaken = 0;
        }

        public string GetUnitStyle(UnitType buildType)
        {
            var rules = RuleSet.GetUnitRules(buildType);
            return ArmyRules.Colour + buildType +
                   (rules.ClassType == UnitClassType.Infantry ? ArmyRules.InfantryStyle : "");
        }
    }
}
