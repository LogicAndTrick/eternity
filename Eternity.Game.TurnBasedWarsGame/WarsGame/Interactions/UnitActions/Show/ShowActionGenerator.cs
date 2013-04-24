using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Show
{
    public class ShowActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();

            // Make sure the unit can hide and it's currently hidden
            if (!context.Unit.UnitRules.CanHide || !context.Unit.IsHidden) return false;

            // The unit can't have done anything except move
            return !queue.Any(x => x.Type != UnitActionType.None && x.Type != UnitActionType.Move);
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            var context = queue.Last();
            yield return new ShowAction(context);
        }
    }
}