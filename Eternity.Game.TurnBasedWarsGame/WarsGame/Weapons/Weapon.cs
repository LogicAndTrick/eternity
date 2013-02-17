using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Weapons
{
    public class Weapon
    {
        public WeaponRules WeaponRules { get; private set; }
        public Unit Unit { get; private set; }
        public int CurrentAmmo { get; set; }

        public Weapon(Unit unit, WeaponRules rules)
        {
            WeaponRules = rules;
            Unit = unit;
            CurrentAmmo = rules.Ammo;
        }

        public bool CanAttack(Unit unit)
        {
            if (unit == null) return false;
            if (unit.Army.ArmyRules.Colour == Unit.Army.ArmyRules.Colour) return false;
            if (!RuleSet.CanAttack(Unit.UnitType, unit.UnitType, WeaponRules.ClassType)) return false;
            if (!HasAmmo()) return false;
            return true;
        }

        public bool InRange(Tile attackFromTile, Tile tileToAttack)
        {
            var distance = attackFromTile.DistanceFrom(tileToAttack);
            return WeaponRules.AttackType == AttackType.Direct
                       ? distance == 1
                       : distance >= WeaponRules.MinRange && distance <= WeaponRules.MaxRange;
        }

        public bool HasAmmo()
        {
            return CurrentAmmo > 0 || WeaponRules.Ammo == 0;
        }

        public bool CanBeResupplied()
        {
            return CurrentAmmo != WeaponRules.Ammo;
        }
    }
}
