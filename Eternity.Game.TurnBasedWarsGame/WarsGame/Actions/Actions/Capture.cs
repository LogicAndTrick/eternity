using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions.Actions
{
    /// <summary>
    /// Infantry can capture buildings
    /// </summary>
    public class Capture : IUnitAction, IUnitActionGenerator
    {
        public UnitActionType ActionType
        {
            get { return UnitActionType.Capture; }
        }

        public void Execute(UnitActionSet set, Action callback)
        {
            var last = set.CurrentMoveSet.Last();
            var structure = last.MoveTile.Structure;
            var points = structure.CapturePoints - set.Unit.HealthOutOfTen;
            if (points <= 0) structure.Capture(set.Unit.Army);
            else structure.CapturePoints = points;
            callback();
        }

        public bool IsValidFor(UnitActionSet set)
        {
            var last = set.CurrentMoveSet.LastOrDefault();
            return last != null
                   && last.MoveType == MoveType.Move
                   && last.MoveTile.Type.IsCapturable()
                   && last.MoveTile.Structure != null
                   && last.MoveTile.Structure.Army != set.Unit.Army
                   && set.Unit.UnitType.CanCapture();
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            yield return new Capture();
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            throw new InvalidOperationException("The capture action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            throw new InvalidOperationException("The capture action is instant and cannot update the move set.");
        }

        public void Cancel(UnitActionSet set)
        {
            // 
        }

        public string GetName()
        {
            return "Capture";
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