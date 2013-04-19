using System.Collections.Generic;
using System.Linq;
using Eternity.Algorithms;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    public class MoveSet : List<Move>
    {
        public MoveSet(Unit unit, IEnumerable<Move> collection) : base(collection)
        {
            Unit = unit;
        }

        public Unit Unit { get; private set; }

        public MoveSet(Unit unit)
        {
            Unit = unit;
            TryMovePathTo(Move.CreateMove(unit.Tile, unit));
        }

        public static MoveSet AllPossibleMoves(Unit unit, Tile tile = null)
        {
            var move = Move.CreateMove(tile ?? unit.Tile, unit);
            var states = Search.GetAllStates(
                move, (m, c) => m.GetMovableTiles(c),
                Equals, x => x.GetMovementCost());
            return new MoveSet(unit, states);
        }

        public static MoveSet AllPossibleAttackPositions(Unit unit, Tile tile, bool allowMovement)
        {
            var set = new MoveSet(unit);
            if (unit.CanAttackDirectly())
            {
                var adj = allowMovement
                              ? AllPossibleMoves(unit, tile)
                                    .Where(x => x.MoveType == MoveType.Move)
                                    .SelectMany(x => x.MoveTile.GetAdjacentTiles())
                              : tile.GetAdjacentTiles();
                set.AddRange(adj.Where(x => x != null).Select(x => Move.CreateAttack(x, unit)));
            }
            if (allowMovement && unit.CanAttackIndirectly(true))
            {
                var moves = AllPossibleMoves(unit, tile).Where(x => x.MoveType == MoveType.Move);
                set.AddRange(tile.Parent.Tiles
                                 .Where(x => moves.Any(y => unit.CanAttackIndirectly(y.MoveTile, x)))
                                 .Select(x => Move.CreateAttack(x, unit)));
            }
            if (unit.CanAttackIndirectly(false))
            {
                set.AddRange(tile.Parent.Tiles
                                 .Where(x => unit.CanAttackIndirectly(tile, x))
                                 .Select(x => Move.CreateAttack(x, unit)));
            }
            return set;
        }

        public int GetMovementCost()
        {
            return this.Where(x => x.MoveTile != Unit.Tile && x.MoveType == MoveType.Move)
                .Sum(x => x.GetMovementCost());
        }

        public MoveSet GetPossibleAttackMoves()
        {
            var fromTile = Count == 0 ? Unit.Tile : this.Last().MoveTile;
            var currentCost = this.Skip(1).Sum(x => x.GetMovementCost());
            var attackable = Unit.GetAttackableTiles(fromTile, currentCost > 0);
            var states = attackable.Select(x => Move.CreateAttack(x, Unit));
            return new MoveSet(Unit, states);
        }

        public MoveSet GetMovementMoves()
        {
            return new MoveSet(Unit, this.Where(x => x.MoveType == MoveType.Move));
        }

        public Move GetAttackMove()
        {
            return this.FirstOrDefault(x => x.MoveType == MoveType.Attack);
        }

        public MoveSet GetUnloadMoves()
        {
            return new MoveSet(Unit, this.Where(x => x.MoveType == MoveType.Unload));
        }

        public bool TryMovePathTo(params Move[] candidates)
        {
            Move target;
            List<Move> path = null;
            // Check if this set already moves to any of the candidates
            if (this.Any() && candidates.Contains(this.Last()))
            {
                target = this.Last();
                path = new List<Move>(this);
            }
            else
            {
                var pathDone = false;
                if (this.Any())
                {
                    // try and extend the current path
                    var lowestVal = int.MaxValue;
                    foreach (var move in candidates)
                    {
                        var temp = move;
                        var tempPath = Search.InformedSearch(this.Last(),
                                                             this.Skip(1).Sum(x => x.GetMovementCost()),
                                                             (m, c) => m.GetMovableTiles(c),
                                                             (x, y) => x.Equals(y),
                                                             x => x.GetMovementCost(),
                                                             x => x.Equals(temp));
                        if (!tempPath.Any() || tempPath.Count >= lowestVal) continue;

                        // Found a path running from the last tile to the target
                        pathDone = true;
                        path = new List<Move>(this);
                        var index = path.FindIndex(tempPath.Contains);
                        path.RemoveRange(index + 1, path.Count - index - 1);
                        index = tempPath.IndexOf(path[index]);
                        path.AddRange(tempPath.GetRange(index + 1, tempPath.Count - index - 1));
                        lowestVal = tempPath.Count;
                    }
                }

                if (!pathDone)
                {
                    // Recalculate a new path running to the candidate
                    var lowestVal = int.MaxValue;
                    var start = Move.CreateMove(Unit.Tile, Unit);
                    foreach (var tile in candidates)
                    {
                        var temp = tile;
                        var tempPath = Search.InformedSearch(start, 0,
                                                             (m, c) => m.GetMovableTiles(c),
                                                             (x, y) => x.Equals(y),
                                                             x => x.GetMovementCost(),
                                                             x => x.Equals(temp));
                        if (tempPath.Count >= lowestVal) continue;

                        path = tempPath;
                        lowestVal = tempPath.Count;
                    }
                }
                target = path == null ? null : path.LastOrDefault();
            }
            if (path != null && target != null && path.Count > 0)
            {
                Clear();
                AddRange(path);
                return true;
            }
            return false;
        }
    }
}