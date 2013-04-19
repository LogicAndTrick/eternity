using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Join
{
    public class JoinActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();
            return context.Unit.CanJoinWith(context.Tile.Unit);
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new JoinAction(queue.Last());
        }
    }
}