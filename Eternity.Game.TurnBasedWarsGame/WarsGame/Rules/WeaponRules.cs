using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Weapons;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Rules
{
    public class WeaponRules
    {
        public WeaponClassType ClassType { get; private set; }
        public WeaponType WeaponType { get; private set; }
        public AttackType AttackType { get; private set; }
        public int MinRange { get; private set; }
        public int MaxRange { get; private set; }
        public int Ammo { get; private set; }
        public bool CanMoveAndFire { get; private set; }

        public WeaponRules(ResourceDefinition def)
        {
            WeaponClassType ct;
            Enum.TryParse(def.GetData("ClassType"), out ct);
            ClassType = ct;
            WeaponType wt;
            Enum.TryParse(def.GetData("WeaponType"), out wt);
            WeaponType = wt;
            AttackType at;
            Enum.TryParse(def.GetData("AttackType"), out at);
            AttackType = at;
            MinRange = int.Parse(def.GetData("MinRange", "0"));
            MaxRange = int.Parse(def.GetData("MaxRange", "0"));
            Ammo = int.Parse(def.GetData("Ammo", "0"));
            CanMoveAndFire = AttackType == AttackType.Direct || bool.Parse(def.GetData("CanMoveAndFire", "False"));
        }
    }
}
