using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns.Actions
{
    /// <summary>
    /// Loaded units can drop off their loadees onto adjacent tiles.
    /// </summary>
    public class Unload : IUnitAction, IUnitActionGenerator
    {
        public UnitActionType ActionType
        {
            get { return UnitActionType.Unload; }
        }

        private Unit UnitToUnload { get; set; }

        public void Execute(UnitActionSet set, Action callback)
        {
            var move = set.CurrentMoveSet.First(x => x.MoveType == MoveType.Unload && x.UnitToMove == UnitToUnload);
            var endTile = move.MoveTile;
            var board = set.Unit.Tile.Parent.Battle.GameBoard;
            var moveset = new MoveSet(UnitToUnload)
                                {
                                    Move.CreateMove(set.Unit.Tile, UnitToUnload),
                                    Move.CreateMove(endTile, UnitToUnload)
                                };
            board.AnimatePath(UnitToUnload, moveset,
                                () =>
                                {
                                    set.Unit.LoadedUnits.Remove(UnitToUnload);
                                    endTile.Unit = UnitToUnload;
                                    callback();
                                });
        }

        public bool IsValidFor(UnitActionSet set)
        {
            // Must have move loaded units than current unloads
            var unloads = set.CurrentMoveSet.Count(x => x.MoveType == MoveType.Unload);
            if (set.Unit.LoadedUnits.Count <= unloads) return false;

            // Must be moving to a blank tile
            var target = set.CurrentMoveSet.LastOrDefault(x => x.MoveType != MoveType.Unload);
            if (target == null || target.MoveType != MoveType.Move) return false;
            if (target.MoveTile.Unit != null && target.MoveTile.Unit != set.Unit) return false;

            // Must be able to move the loaded units onto adjacent tiles
            var adjacent = target.MoveTile.GetAdjacentTiles().Where(x => x != null && x.Unit == null).ToList();
            return set.Unit.LoadedUnits.Any(x => adjacent.Any(y => x.CanMoveOn(y.Type)));
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            var target = set.CurrentMoveSet.Last(x => x.MoveType != MoveType.Unload);
            var adjacent = target.MoveTile.GetAdjacentTiles().Where(x => x != null && x.Unit == null).ToList();
            return set.Unit.LoadedUnits
                .Where(x => adjacent.Any(y => x.CanMoveOn(y.Type)))
                .Where(x => !set.CurrentMoveSet.Any(y => y.UnitToMove == x && y.MoveType == MoveType.Unload))
                .Select(x => new Unload {UnitToUnload = x});
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return tile.CanMoveTo;
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            var target = set.CurrentMoveSet.Last(x => x.MoveType != MoveType.Unload);
            return target.MoveTile.GetAdjacentTiles()
                .Where(x => x != null && UnitToUnload.CanMoveOn(x.Type))
                .Where(x => x.Unit == null || x.Unit == set.Unit)
                .Where(x => !set.CurrentMoveSet.Any(y => y.MoveType == MoveType.Unload && y.MoveTile == x && y.UnitToMove != UnitToUnload))
                .Select(x => new ValidTile { MoveType = MoveType.Move, Tile = x}).ToList();
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            var target = set.CurrentMoveSet.Last(x => x.MoveType != MoveType.Unload);
            set.CurrentMoveSet.RemoveAll(x => x.MoveType == MoveType.Unload && x.UnitToMove == UnitToUnload);
            set.CurrentMoveSet.Add(Move.CreateUnload(tile, target.MoveTile, UnitToUnload));
        }

        public void Cancel(UnitActionSet set)
        {
            set.CurrentMoveSet.RemoveAll(x => x.MoveType == MoveType.Unload && x.UnitToMove == UnitToUnload);
        }

        public string GetName()
        {
            return "Unload";
        }

        public bool IsInstantAction(UnitActionSet set)
        {
            return false;
        }

        public bool IsCommittingAction(UnitActionSet set)
        {
            var unloads = set.CurrentMoveSet.GetUnloadMoves().Select(x => x.UnitToMove).ToList();
            unloads.Add(UnitToUnload);
            return set.Unit.LoadedUnits.All(unloads.Contains);
        }
    }
}