using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Weapons;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Units
{
    /// <summary>
    /// The unit represents a single unit on the game board.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// Get or set the tile that the unit is currently on.
        /// Do not use this to change a unit's tile, use the Tile.Unit property instead.
        /// </summary>
        public Tile Tile { get; set; }

        /// <summary>
        /// Get the unit's army
        /// </summary>
        public Army Army { get; private set; }

        /// <summary>
        /// Get the unit's type
        /// </summary>
        public UnitType UnitType { get; private set; }

        /// <summary>
        /// Get the unit's sprite style
        /// </summary>
        public string Style { get; private set; }

        /// <summary>
        /// Get the unit's UnitRules from the active RuleSet
        /// </summary>
        public UnitRules UnitRules { get; private set; }

        /// <summary>
        /// Get or set the unit's current health (out of 100hp)
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// Get the unit's health, rounded up to the closest integer out of ten
        /// </summary>
        public int HealthOutOfTen { get { return (int)Math.Ceiling(CurrentHealth / 10.0); } }

        /// <summary>
        /// Get or set the unit's current fuel amount
        /// </summary>
        public int CurrentFuel { get; set; }

        /// <summary>
        /// Get the unit's primary weapon
        /// </summary>
        public Weapon PrimaryWeapon { get; private set; }

        /// <summary>
        /// Get the unit's secondary weapon
        /// </summary>
        public Weapon SecondaryWeapon { get; private set; }

        /// <summary>
        /// The units currently loaded within this unit
        /// </summary>
        public List<Unit> LoadedUnits { get; private set; }

        /// <summary>
        /// Get or set the unit's current amount of building material
        /// </summary>
        public int BuildingMaterial { get; set; }

        /// <summary>
        /// Get or set the unit's current amount of unit material
        /// </summary>
        public int UnitMaterial { get; set; }

        /// <summary>
        /// Get or set whether this unit has been moved in the current turn or not
        /// </summary>
        public bool HasMoved { get; set; }

        /// <summary>
        /// Create a unit from an army and a resource definition from a map file.
        /// </summary>
        /// <param name="army">The unit's army</param>
        /// <param name="def">The unit's resource definition</param>
        public Unit(Army army, ResourceDefinition def)
        {
            Army = army;
            army.Units.Add(this);
            UnitType unitType;
            Enum.TryParse(def.GetData("UnitType"), out unitType);
            UnitType = unitType;
            Style = def.GetData("Name");
            UnitRules = RuleSet.GetUnitRules(unitType);
            CurrentFuel = UnitRules.Fuel;
            CurrentHealth = 100;
            LoadedUnits = new List<Unit>();
            BuildingMaterial = UnitRules.BuildingMaterial;
            UnitMaterial = UnitRules.UnitMaterial;
            InitWeapons();
        }

        /// <summary>
        /// Create a unit from an army, type, and style
        /// </summary>
        /// <param name="army">The unit's army</param>
        /// <param name="unitType">The unit's type</param>
        /// <param name="style">The unit's sprite style</param>
        public Unit(Army army, UnitType unitType, string style)
        {
            Army = army;
            army.Units.Add(this);
            UnitType = unitType;
            Style = style;
            UnitRules = RuleSet.GetUnitRules(unitType);
            CurrentFuel = UnitRules.Fuel;
            CurrentHealth = 100;
            LoadedUnits = new List<Unit>();
            BuildingMaterial = UnitRules.BuildingMaterial;
            UnitMaterial = UnitRules.UnitMaterial;
            InitWeapons();
        }

        private void InitWeapons()
        {
            var primary = UnitRules.WeaponRules.FirstOrDefault(x => x.ClassType == WeaponClassType.Primary);
            if (primary != null) PrimaryWeapon = new Weapon(this, primary);
            var secondary = UnitRules.WeaponRules.FirstOrDefault(x => x.ClassType == WeaponClassType.Secondary);
            if (secondary != null) SecondaryWeapon = new Weapon(this, secondary);
        }

        /// <summary>
        /// Heal this unit by the specified amount. The unit's health cannot go over 100 points.
        /// </summary>
        /// <param name="points">The number of points (out of 100) to heal</param>
        public void Heal(int points)
        {
            CurrentHealth = Math.Min(100, CurrentHealth + points);
        }

        /// <summary>
        /// Resupply this unit with fuel and ammo.
        /// </summary>
        public void Resupply()
        {
            CurrentFuel = UnitRules.Fuel;
            if (PrimaryWeapon != null) PrimaryWeapon.CurrentAmmo = PrimaryWeapon.WeaponRules.Ammo;
            if (SecondaryWeapon != null) SecondaryWeapon.CurrentAmmo = SecondaryWeapon.WeaponRules.Ammo;
        }

        public bool CanResupply()
        {
            return UnitRules.CanSupply;
        }

        public bool CanBeResupplied()
        {
            return CurrentFuel != UnitRules.Fuel
                   || (PrimaryWeapon != null && PrimaryWeapon.CanBeResupplied())
                   || (SecondaryWeapon != null && SecondaryWeapon.CanBeResupplied());
        }

        /// <summary>
        /// Check if this unit can be loaded with the supplied unit. Takes capacity and class type into account.
        /// </summary>
        /// <param name="unit">The unit to load into this one</param>
        /// <returns>True if the load is valid</returns>
        public bool CanLoadWith(Unit unit)
        {
            return unit != null &&
                   UnitRules.LoadCapacity > LoadedUnits.Count &&
                   UnitRules.LoadClassTypes.Contains(unit.UnitRules.ClassType);
        }

        /// <summary>
        /// Load the supplied unit into this one.
        /// </summary>
        /// <param name="unit">The unit to load</param>
        public void LoadWith(Unit unit)
        {
            LoadedUnits.Add(unit);
        }

        /// <summary>
        /// Check if this unit can be joined with the supplied unit.
        /// </summary>
        /// <param name="unit">The unit to join with</param>
        /// <returns>True if the join is valid</returns>
        public bool CanJoinWith(Unit unit)
        {
            return unit != null &&
                   Army == unit.Army &&
                   UnitType == unit.UnitType &&
                   unit.CurrentHealth < 100 &&
                   unit != this;
        }

        /// <summary>
        /// Join another unit with this one. Merges HP, fuel, and ammo.
        /// TODO veterancy status
        /// </summary>
        /// <param name="unit">The unit to join with</param>
        public void JoinWith(Unit unit)
        {
            if (Army != unit.Army) throw new ArgumentException("Units must be the same army to join.");
            if (UnitType != unit.UnitType) throw new ArgumentException("Units must be the same type to join.");
            if (CurrentHealth == 100) throw new ArgumentException("Cannot join with a unit that has full health.");
            if (unit == this) throw new ArgumentException("Cannot join a unit with itself.");
            Heal(unit.CurrentHealth);
            CurrentFuel = Math.Min(UnitRules.Fuel, CurrentHealth + unit.CurrentFuel);
            if (PrimaryWeapon != null) PrimaryWeapon.CurrentAmmo = Math.Min(PrimaryWeapon.WeaponRules.Ammo, PrimaryWeapon.CurrentAmmo + unit.PrimaryWeapon.CurrentAmmo);
            if (SecondaryWeapon != null) SecondaryWeapon.CurrentAmmo = Math.Min(SecondaryWeapon.WeaponRules.Ammo, SecondaryWeapon.CurrentAmmo + unit.SecondaryWeapon.CurrentAmmo);
        }

        /// <summary>
        /// Attack an enemy who will counter if possible. This unit and
        /// the enemy may no longer be on the board after this is called.
        /// </summary>
        /// <param name="unitToAttack">The unit to attack</param>
        public void AttackUnit(Unit unitToAttack)
        {
            // Attack
            var dmg = DamageCalculator.CalculateDamageAW4(this, unitToAttack,
                                                          GetPreferredAttackWeaponClassType(unitToAttack));
            unitToAttack.CurrentHealth -= dmg;
            if (unitToAttack.CurrentHealth <= 0)
            {
                // Defending unit is dead
                unitToAttack.Tile.Unit = null;
            }
            else if (unitToAttack.CanCounterAttack(this))
            {
                unitToAttack.CounterAttackUnit(this);
            }
        }

        /// <summary>
        /// Counter an enemy who is firing on this unit.
        /// </summary>
        /// <param name="unitToCounter">The unit to counter</param>
        private void CounterAttackUnit(Unit unitToCounter)
        {
            var dmg = DamageCalculator.CalculateDamageAW4(this, unitToCounter,
                                                          GetPreferredCounterAttackWeaponClassType(unitToCounter));
            unitToCounter.CurrentHealth -= dmg;
            if (unitToCounter.CurrentHealth <= 0)
            {
                unitToCounter.Tile.Unit = null;
            }
        }

        /// <summary>
        /// True if this unit can move on the specified tile type
        /// </summary>
        /// <param name="tt">The tile type to test</param>
        /// <returns>True if the unit can move on the tile type</returns>
        public bool CanMoveOn(TileType tt)
        {
            return RuleSet.GetTerrainRules(tt).CanMove(UnitRules.MoveType);
        }

        /// <summary>
        /// Gets the movement cost of the unit moving on the specified tile type.
        /// </summary>
        /// <param name="tt">The tile type to test</param>
        /// <returns>The movement cost of the unit moving on the tile type</returns>
        public int GetMovementCost(TileType tt)
        {
            return RuleSet.GetTerrainRules(tt).GetMoveCost(UnitRules.MoveType);
        }

        /// <summary>
        /// Test if this unit can move on a tile, taking into account movement costs, move points, and fuel.
        /// Tiles with fog are assumed to be movable.
        /// </summary>
        /// <param name="tile">The tile to move to</param>
        /// <param name="currentCost">The current cost of the path</param>
        /// <returns>True if this tile is valid for path continuation</returns>
        public bool CanMove(Tile tile, int currentCost)
        {
            if (tile == null || !CanMoveOn(tile.Type)) return false; 
            if (!tile.Fog && tile.Unit != null && tile.Unit.Army.ArmyRules.Name != Army.ArmyRules.Name) return false;
            var newCost = GetMovementCost(tile.Type) + currentCost;
            return newCost <= UnitRules.MovePoints && newCost <= CurrentFuel;
        }

        /// <summary>
        /// Gets the preferred weapon class type for this unit to attack the specified unit.
        /// Assumes that the attack is a valid move.
        /// </summary>
        /// <param name="unitToAttack">The unit to attack</param>
        /// <returns>The preferred weapon class type to attack the unit</returns>
        public WeaponClassType GetPreferredAttackWeaponClassType(Unit unitToAttack)
        {
            // TODO WEAPONS AND AMMO
            if (RuleSet.CanAttack(UnitType, unitToAttack.UnitType, WeaponClassType.Primary)) return WeaponClassType.Primary;
            return WeaponClassType.Secondary;
        }

        /// <summary>
        /// Gets the preferred weapon class type for this unit to counter the specified unit.
        /// Assumes that the counter is a valid move.
        /// </summary>
        /// <param name="unitToCounter">The unit to counter attack</param>
        /// <returns>The preferred weapon class type to counter the unit</returns>
        public WeaponClassType GetPreferredCounterAttackWeaponClassType(Unit unitToCounter)
        {
            if (PrimaryWeapon != null && CanCounterAttack(unitToCounter, PrimaryWeapon)) return WeaponClassType.Primary;
            else return WeaponClassType.Secondary;
        }

        /// <summary>
        /// Tests if this unit is able to attack the specified unit.
        /// Takes range and ammo into account. Assumes the unit has not moved.
        /// </summary>
        /// <param name="unitToAttack">The unit to test</param>
        /// <returns>True if this unit can attack the other</returns>
        public bool CanAttack(Unit unitToAttack)
        {
            if (PrimaryWeapon != null && CanAttack(unitToAttack, PrimaryWeapon, false)) return true;
            if (SecondaryWeapon != null && CanAttack(unitToAttack, SecondaryWeapon, false)) return true;
            return false;
        }

        /// <summary>
        /// Tests if this unit is able to counter the specified unit.
        /// Takes range and ammo into account. Countering is only possible for direct-attack units.
        /// </summary>
        /// <param name="unitToCounter">The unit to test</param>
        /// <returns>True if this unit can counter the other</returns>
        public bool CanCounterAttack(Unit unitToCounter)
        {
            if (PrimaryWeapon != null && CanCounterAttack(unitToCounter, PrimaryWeapon)) return true;
            if (SecondaryWeapon != null && CanCounterAttack(unitToCounter, SecondaryWeapon)) return true;
            return false;
        }

        private bool CanAttack(Unit unitToAttack, Weapon weapon, bool hasMoved)
        {
            if (!weapon.CanAttack(unitToAttack)) return false;
            if (weapon.WeaponRules.AttackType != AttackType.Direct && hasMoved && !weapon.WeaponRules.CanMoveAndFire) return false;
            return true;
        }

        private bool CanCounterAttack(Unit unitToCounter, Weapon weapon)
        {
            return weapon.CanAttack(unitToCounter) &&
                   weapon.WeaponRules.AttackType == AttackType.Direct &&
                   unitToCounter.Tile.DistanceFrom(Tile) == 1;
        }

        /// <summary>
        /// Gets the list of attackable tiles from the specified tile with the specified path cost.
        /// Indirect weapons can only attack tiles if they haven't moved or if they can fire after moving.
        /// Tiles with fog cannot be attacked.
        /// </summary>
        /// <param name="tile">The tile from attack from</param>
        /// <param name="hasMoved">Whether the unit has moved or not</param>
        /// <returns>The list of attackable tiles</returns>
        public List<Tile> GetAttackableTiles(Tile tile, bool hasMoved)
        {
            var list = new List<Tile>();
            // Cannot attack any tile if this unit is overlapping another (while moving)
            if (tile.Unit != null && tile.Unit != this) return list;
            if (PrimaryWeapon != null)
            {
                var primaryTiles = tile.Parent.Tiles.Where(x => PrimaryWeapon.InRange(tile, x) && !x.Fog);
                list.AddRange(primaryTiles.Where(x => CanAttack(x.Unit, PrimaryWeapon, hasMoved)));
            }
            if (SecondaryWeapon != null)
            {
                var secondaryTiles = tile.Parent.Tiles.Where(x => SecondaryWeapon.InRange(tile, x) && !x.Fog);
                list.AddRange(secondaryTiles.Where(x => CanAttack(x.Unit, SecondaryWeapon, hasMoved)));
            }
            return list;
        }

        /// <summary>
        /// Gets the list of movable tiles from the specified tile with the specified path cost.
        /// </summary>
        /// <param name="tile">The tile to move from</param>
        /// <param name="currentCost">The current path cost</param>
        /// <returns>The list of movable tiles</returns>
        public List<Tile> GetMovableTiles(Tile tile, int currentCost)
        {
            return tile.GetAdjacentTiles().Where(x => CanMove(x, currentCost)).ToList();
        }
        
        /// <summary>
        /// Test if this unit has a direct attack weapon
        /// </summary>
        /// <returns>True if any weapon is direct and has ammo</returns>
        public bool CanAttackDirectly()
        {
            return (PrimaryWeapon != null && PrimaryWeapon.WeaponRules.AttackType == AttackType.Direct
                    && PrimaryWeapon.HasAmmo()) ||
                   (SecondaryWeapon != null && SecondaryWeapon.WeaponRules.AttackType == AttackType.Direct
                    && SecondaryWeapon.HasAmmo());
        }

        /// <summary>
        /// Test if this unit has a indirect attack weapon
        /// </summary>
        /// <returns>True if any weapon is indirect and has ammo</returns>
        public bool CanAttackIndirectly(bool afterMoving)
        {
            return (PrimaryWeapon != null && PrimaryWeapon.WeaponRules.AttackType == AttackType.Indirect
                    && PrimaryWeapon.HasAmmo() && (PrimaryWeapon.WeaponRules.CanMoveAndFire || !afterMoving)) ||
                   (SecondaryWeapon != null && SecondaryWeapon.WeaponRules.AttackType == AttackType.Indirect
                    && SecondaryWeapon.HasAmmo() && (SecondaryWeapon.WeaponRules.CanMoveAndFire || !afterMoving));
        }

        /// <summary>
        /// Test if this unit can indirectly attack the given tile
        /// </summary>
        /// <param name="position">The tile to attack from</param>
        /// <param name="target">The tile to attack</param>
        /// <returns>True if the tile is in weapon range and the weapon has ammo</returns>
        public bool CanAttackIndirectly(Tile position, Tile target)
        {
            return (PrimaryWeapon != null && PrimaryWeapon.WeaponRules.AttackType == AttackType.Indirect
                    && PrimaryWeapon.HasAmmo() && PrimaryWeapon.InRange(position, target)) ||
                   (SecondaryWeapon != null && SecondaryWeapon.WeaponRules.AttackType == AttackType.Indirect
                    && SecondaryWeapon.HasAmmo() && SecondaryWeapon.InRange(position, target));
        }

        public bool CanSupply()
        {
            return UnitRules.CanSupply;
        }

        public bool CanBuildUnits()
        {
            return UnitRules.CanBuildUnits
                   && UnitRules.LoadCapacity > LoadedUnits.Count
                   && UnitMaterial > 0;
        }

        public bool CanBuildBuildings(TileType type)
        {
            return UnitRules.CanBuildBuildings
                   && UnitRules.BuildBuildings.ContainsKey(type)
                   && UnitRules.BuildBuildings[type].Count > 0
                   && BuildingMaterial > 0;
        }
    }
}
