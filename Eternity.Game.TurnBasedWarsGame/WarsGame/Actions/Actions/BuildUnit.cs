using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions.Actions
{
    /// <summary>
    /// Some units can build other units internally (carriers)
    /// </summary>
    public class BuildUnit : IUnitAction, IUnitActionGenerator
    {
        public UnitActionType ActionType
        {
            get { return UnitActionType.Build; }
        }

        private UnitType BuildType { get; set; }

        public void Execute(UnitActionSet set, Action callback)
        {
            var unit = new Unit(set.Unit.Army, BuildType, set.Unit.Army.GetUnitStyle(BuildType));
            set.Unit.UnitMaterial--;
            set.Unit.Army.Units.Add(unit);
            set.Unit.LoadWith(unit);
            callback();
        }

        public bool IsValidFor(UnitActionSet set)
        {
            return set.CurrentMoveSet.Count == 1 && set.Unit.CanBuildUnits();
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            return set.Unit.UnitRules.BuildUnits.Select(x => new BuildUnit {BuildType = x});
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            throw new InvalidOperationException("The build action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            throw new InvalidOperationException("The build action is instant and cannot update the move set.");
        }

        public void Cancel(UnitActionSet set)
        {
            // 
        }

        public string GetName()
        {
            return "Build";
        }

        public bool IsInstantAction(UnitActionSet set)
        {
            return true;
        }

        public bool IsCommittingAction(UnitActionSet set)
        {
            return true;
        }
    }
}