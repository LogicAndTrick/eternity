using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Capture
{
    public class CaptureActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();
            return queue.All(x => x.Type == UnitActionType.None || x.Type == UnitActionType.Move)
                   && context.Tile.Type.IsCapturable()
                   && context.Tile.Structure != null
                   && !context.Tile.Structure.IsUnderConstruction
                   && context.Tile.Structure.Army != context.Unit.Army
                   && context.Unit.UnitType.CanCapture();
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new CaptureAction(queue.Last());
        }
    }
}