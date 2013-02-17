using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Weapons;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Rules
{
    public static class DamageCalculator
    {
        public static int CalculateDamageAW1(Unit attacker, Unit defender, WeaponClassType classType)
        {
            // I love the internet.
            // http://www.warsworldnews.com/index.php?page=aw/battlemechanics/index.php

            // ((((((attack * baseDamage) * attackNorm) * defense) * defendNorm) * attackHP) * ((100 - terrainBonus * defendHP) / 100))
            // The value inside each parenthesis must be rounded down.

            decimal baseDamage = RuleSet.GetDamageValue(attacker.UnitType, defender.UnitType, classType);

            decimal attack = 1; // TODO: Determined by CO, com towers, power use, verterancy status, etc
            decimal attackNorm = 1; // TODO: Determined by CO

            decimal attackHP = attacker.CurrentHealth / 100m;
            decimal defendHP = attacker.CurrentHealth / 100m;

            decimal defense = 1; // TODO: Determined by CO
            decimal defendNorm = 1; // TODO: Determined by CO

            decimal terrainBonus = RuleSet.GetTerrainRules(defender.Tile.Type).Defense * 10;

            var terrainVal = Math.Floor(Math.Floor(100 - terrainBonus * defendHP) / 100m);
            var value = Math.Floor(attack * baseDamage);
            value = Math.Floor(value * attackNorm);
            value = Math.Floor(value * defense);
            value = Math.Floor(value * defendNorm);
            value = Math.Floor(value * attackHP);
            value = Math.Floor(value * terrainVal);
            return (int) value;
        }

        public static int CalculateDamageAW4(Unit attacker, Unit defender, WeaponClassType classType)
        {
            // http://www.warsworldnews.com/dor/aw4-color.png
            // base damage * attacker HP / 10 * total attack power / total defensive power
            decimal baseDamage = RuleSet.GetDamageValue(attacker.UnitType, defender.UnitType, classType);
            decimal attackHP = attacker.CurrentHealth / 100m;
            decimal defendHP = attacker.CurrentHealth / 100m;
            decimal attack = 1;  // TODO: Determined by CO, com towers, power use, verterancy status, terrain bonus, etc
            decimal defense = 1; // TODO: Determined by CO, com towers, power use, verterancy status, terrain bonus, etc
            return (int) Math.Floor(baseDamage * attackHP * attack / defense);
        }
    }
}
