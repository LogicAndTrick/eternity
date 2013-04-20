using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Unload
{
    public class UnloadActionGenerator : IUnitActionGenerator
    {
        private static readonly UnitActionType[] AllowedActionTypes = new[] { UnitActionType.None, UnitActionType.Move, UnitActionType.Unload };
 
        public bool IsValidFor(ContextQueue queue)
        {
            var context = queue.Last();

            // Must have move loaded units than current unloads
            var unloads = queue.Where(x => x.Type == UnitActionType.Unload)
                .Select(x => x.Action as UnloadAction)
                .Where(x => x != null)
                .SelectMany(x => x.GetMoveSet())
                .ToList();
            if (context.Unit.LoadedUnits.Count <= unloads.Count) return false;

            // Must be moving to a blank tile
            if (queue.Any(x => !AllowedActionTypes.Contains(x.Type))) return false;
            if (context.Tile.Unit != null && context.Tile.Unit != context.Unit) return false;

            // Must be able to move the loaded units onto empty adjacent tiles with no other units already unloading onto them
            var adjacent = context.Tile.GetAdjacentTiles()
                .Where(x => x != null && (x.Unit == null || x.Unit == context.Unit))
                .Where(x => unloads.All(y => y.MoveTile != x))
                .ToList();
            return context.Unit.LoadedUnits.Any(x => adjacent.Any(y => x.CanMoveOn(y.Type)));
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            var context = queue.Last();
            var unloads = queue.Where(x => x.Type == UnitActionType.Unload)
                .Select(x => x.Action as UnloadAction)
                .Where(x => x != null)
                .SelectMany(x => x.GetMoveSet())
                .ToList();
            var adjacent = context.Tile.GetAdjacentTiles()
                .Where(x => x != null && (x.Unit == null || x.Unit == context.Unit))
                .ToList();
            return context.Unit.LoadedUnits
                .Where(x => adjacent.Any(y => x.CanMoveOn(y.Type)))
                .Where(x => unloads.All(y => y.UnitToMove != x))
                .Select(x => new UnloadAction(context, x, unloads));
        }
    }
}