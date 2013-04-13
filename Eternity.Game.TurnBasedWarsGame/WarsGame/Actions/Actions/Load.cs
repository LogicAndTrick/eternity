using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions.Actions
{
    /// <summary>
    /// Some units can be loaded with others (carrier, APC, cruiser, etc)
    /// </summary>
    public class Load : IUnitAction, IUnitActionGenerator
    {
        public UnitActionType ActionType
        {
            get { return UnitActionType.Load; }
        }

        public void Execute(UnitActionSet set, Action callback)
        {
            // The move action does loading already
            callback();
        }

        public bool IsValidFor(UnitActionSet set)
        {
            var last = set.CurrentMoveSet.LastOrDefault();
            return last != null && last.MoveTile.Unit != null
                && last.MoveTile.Unit.CanLoadWith(set.Unit);
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            yield return new Load();
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return tile.CanMoveTo && tile.Unit != null && tile.Unit.CanJoinWith(set.Unit);
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            throw new InvalidOperationException("The load action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            throw new InvalidOperationException("The load action is instant and cannot update the move set.");
        }

        public void Cancel(UnitActionSet set)
        {
            // 
        }

        public string GetName()
        {
            return "Load";
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