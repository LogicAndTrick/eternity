using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Resupply
{
    public class ResupplyActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();
            return context.Unit.CanResupply() &&
                   context.Tile.GetAdjacentTiles()
                       .Where(x => x != null && x.Unit != null && x.Unit != context.Unit)
                       .Any(x => x.Unit.Army == context.Unit.Army && x.Unit.CanBeResupplied());
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new ResupplyAction(queue.Last());
        }
    }
}