using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.BuildUnit
{
    public class BuildUnitActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();

            // Make sure the unit can build units
            if (!context.Unit.CanBuildUnits()) return false;

            // The unit can't have done anything (the move is caught in the next step)
            if (queue.Any(x => x.Type != UnitActionType.None && x.Type != UnitActionType.Move)) return false;

            // The unit can't have moved off its original tile
            var moves = queue.Select(x => x.Action.GetMoveSet()).Where(x => x != null).SelectMany(x => x);
            return moves.All(x => x.MoveType == MoveType.Move && x.MoveTile == context.Unit.Tile);
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            var context = queue.Last();
            return context.Unit.UnitRules.BuildUnits.Select(x => new BuildUnitAction(context, x));
        }
    }
}