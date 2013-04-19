using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Load
{
    public class LoadActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();
            return context.Tile.Unit != null
                && context.Tile.Unit.CanLoadWith(context.Unit);
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new LoadAction(queue.Last());
        }
    }
}