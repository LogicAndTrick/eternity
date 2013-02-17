using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns.Actions
{
    /// <summary>
    /// Wait ends a turn without any further action.
    /// </summary>
    public class Wait : IUnitAction, IUnitActionGenerator
    {
        public UnitActionType ActionType { get { return UnitActionType.Wait; } }

        public void Execute(UnitActionSet set, Action callback)
        {
            callback();
        }

        public bool IsValidFor(UnitActionSet set)
        {
            var target = set.CurrentMoveSet.LastOrDefault();
            if (target == null) return true;
            if (target.UnitToAttack != null) return false;
            if (target.MoveTile.Unit != null && target.MoveTile.Unit != set.Unit) return false;
            return true;
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            yield return new Wait();
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            return new List<ValidTile>();
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            //
        }

        public void Cancel(UnitActionSet set)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Wait";
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