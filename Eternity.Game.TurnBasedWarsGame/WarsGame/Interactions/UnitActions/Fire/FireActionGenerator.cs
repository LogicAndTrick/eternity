using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Fire
{
    public class FireActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();
            var target = context.Tile;
            var hasMoved = queue.Any(x => x.Type == UnitActionType.Move && x.Tile != x.Unit.Tile);
            return queue.All(x => x.Type != UnitActionType.Fire)
                   && context.Unit.GetAttackableTiles(target, hasMoved).Any();
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new FireAction(queue.Last());
        }
    }
}