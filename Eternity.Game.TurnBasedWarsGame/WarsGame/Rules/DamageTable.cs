using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Weapons;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Rules
{
    public class DamageTable
    {
        private readonly List<TableItem> _items;

        public DamageTable(ResourceDefinition rd)
        {
            _items = new List<TableItem>();
            var attackerLeft = rd.GetData("Attacker") != "Top";
            var order = rd.GetData("Order").Split(' ')
                .Select(x => (UnitType) Enum.Parse(typeof (UnitType), x))
                .ToList();
            int temp;
            var primary = rd.GetData("Primary").Split(' ')
                .Select(x => int.TryParse(x, out temp) ? temp : 0).ToList();
            var secondary = rd.GetData("Secondary").Split(' ')
                .Select(x => int.TryParse(x, out temp) ? temp : 0).ToList();
            for (var i = 0; i < primary.Count; i++)
            {
                var row = i / order.Count;
                var col = i % order.Count;
                _items.Add(new TableItem
                               {
                                   Attacker = attackerLeft ? order[row] : order[col],
                                   Defender = attackerLeft ? order[col] : order[row],
                                   CanAttackPrimary = primary[i] > 0,
                                   CanAttackSecondary = secondary[i] > 0,
                                   PrimaryDamage = primary[i],
                                   SecondaryDamage = secondary[i]
                               });
            }
        }

        public bool CanAttack(UnitType attacker, UnitType defender, WeaponClassType weaponClass)
        {
            var item = _items.FirstOrDefault(x => x.Attacker == attacker && x.Defender == defender);
            if (item == null) return false;
            return weaponClass == WeaponClassType.Primary ? item.CanAttackPrimary : item.CanAttackSecondary;
        }

        public int GetDamageValue(UnitType attacker, UnitType defender, WeaponClassType weaponClass)
        {
            var item = _items.FirstOrDefault(x => x.Attacker == attacker && x.Defender == defender);
            if (item == null) return 0;
            return weaponClass == WeaponClassType.Primary ? item.PrimaryDamage : item.SecondaryDamage;
        }

        private class TableItem
        {
            public UnitType Attacker { get; set; }
            public UnitType Defender { get; set; }
            public bool CanAttackPrimary { get; set; }
            public int PrimaryDamage { get; set; }
            public bool CanAttackSecondary { get; set; }
            public int SecondaryDamage { get; set; }

            public bool Equals(TableItem other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Attacker, Attacker) && Equals(other.Defender, Defender);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (TableItem)) return false;
                return Equals((TableItem) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Attacker.GetHashCode() * 397) ^ Defender.GetHashCode();
                }
            }

            public static bool operator ==(TableItem left, TableItem right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TableItem left, TableItem right)
            {
                return !Equals(left, right);
            }
        }
    }
}
