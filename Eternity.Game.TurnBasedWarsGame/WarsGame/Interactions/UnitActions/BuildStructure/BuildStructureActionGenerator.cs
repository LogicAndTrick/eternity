using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Capture;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.BuildStructure
{
    public class BuildStructureActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();

            // Must be able to build on the tile
            if (!context.Unit.CanBuildBuildings(context.Tile.Type)) return false;

            // Can't have done anything aside from move
            if (!queue.All(x => x.Type == UnitActionType.None || x.Type == UnitActionType.Move)) return false;

            // If a structure is currently being built, it's allowed if the unit hasn't moved at all
            if (context.Tile.Structure != null && context.Tile.Structure.IsUnderConstruction)
            {
                // The unit can't have moved off its original tile
                var moves = queue.Select(x => x.Action.GetMoveSet()).Where(x => x != null).SelectMany(x => x);
                return moves.All(x => x.MoveType == MoveType.Move && x.MoveTile == context.Unit.Tile);
            }

            // Otherwise the structure can be built if one isn't already there
            return context.Tile.Structure == null;
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            yield return new BuildStructureAction(queue.Last());
        }
    }
}