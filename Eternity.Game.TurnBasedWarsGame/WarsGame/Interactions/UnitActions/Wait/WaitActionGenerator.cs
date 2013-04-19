using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Wait
{
    public class WaitActionGenerator : IUnitActionGenerator
    {
        public UnitActionType ActionType { get { return UnitActionType.Wait; } }

        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();
            if (context.Tile.Unit != null && context.Tile.Unit != context.Unit) return false;
            return true;
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new WaitAction(queue.Last());
        }
    }
}