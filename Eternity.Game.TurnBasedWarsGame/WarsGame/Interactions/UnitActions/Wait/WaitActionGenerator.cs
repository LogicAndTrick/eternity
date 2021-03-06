using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Wait
{
    public class WaitActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();
            if (context.Tile.HasVisibleUnit(context.Unit.Army) && context.Tile.Unit != context.Unit) return false;
            return true;
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new WaitAction(queue.Last());
        }
    }
}