using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.TakeOff
{
    public class TakeOffActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();

            if (!context.Unit.UnitRules.AllowTakeOff) return false;

            // Must have loaded air-type movement units that haven't moved in this turn yet
            if (context.Unit.LoadedUnits.All(x => x.UnitRules.MoveType != UnitMoveType.Air || x.HasMoved)) return false;

            // The unit can't have done anything (the move is caught in the next step)
            if (queue.Any(x => x.Type != UnitActionType.None && x.Type != UnitActionType.Move)) return false;

            // The unit can't have moved off its original tile
            var moves = queue.Select(x => x.Action.GetMoveSet()).Where(x => x != null).SelectMany(x => x);
            return moves.All(x => x.MoveType == MoveType.Move && x.MoveTile == context.Unit.Tile);
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            var context = queue.Last();
            return context.Unit.LoadedUnits
                .Where(x => x.UnitRules.MoveType == UnitMoveType.Air && !x.HasMoved)
                .Select(x => new TakeOffAction(context, x));
        }
    }
}