using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions.Actions
{
    /// <summary>
    /// Join a unit with damaged unit of the same type.
    /// </summary>
    public class Join : IUnitAction, IUnitActionGenerator
    {
        public UnitActionType ActionType
        {
            get { return UnitActionType.Join; }
        }

        public void Execute(UnitActionSet set, Action callback)
        {
            // The move action takes care of joins
            callback();
        }

        public bool IsValidFor(UnitActionSet set)
        {
            var last = set.CurrentMoveSet.LastOrDefault();
            return last != null && set.Unit.CanJoinWith(last.MoveTile.Unit);
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            yield return new Join();
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return tile.CanMoveTo && set.Unit.CanJoinWith(tile.Unit);
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            throw new InvalidOperationException("The join action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            throw new InvalidOperationException("The join action is instant and cannot update the move set.");
        }

        public void Cancel(UnitActionSet set)
        {
            // 
        }

        public string GetName()
        {
            return "Join";
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