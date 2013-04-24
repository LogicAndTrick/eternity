using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.MoveAction
{
    public class MoveActionRunner : IUnitActionRunner
    {
        private readonly MoveSet _set;

        public MoveActionRunner(MoveSet set)
        {
            _set = set;
        }

        private bool ValidMove(Move move)
        {
            return move.MoveType == MoveType.Move
                   && ValidInSet(move);
        }

        private bool ValidInSet(Move move)
        {
            if (move.MoveType == MoveType.Attack)
            {
                return true;
            }
            if (move.MoveType == MoveType.Move)
            {
                return move.MoveTile.Unit == null || move.MoveTile.Unit.Army == _set.Unit.Army;
            }
            return false;
        }

        public void Execute(Action<ExecutionState> callback)
        {
            // Fog of war traps: target is the last valid move, not the last tile in the set.
            // When FOW is disabled, the set will not be invalid, however this method works as expected either way.
            // If the set has no moves, than simply use the current tile and move the unit nowhere.
            var target = _set.TakeWhile(ValidMove).LastOrDefault() ?? Move.CreateMove(_set.Unit.Tile, _set.Unit);

            // The actual set is the path that the unit will actually take, after taking traps into account
            var actualSet = new MoveSet(_set.Unit, _set.TakeWhile(ValidMove));

            // There is a trap if any non-valid moves are found
            var trap = _set.Any(x => !ValidInSet(x));

            var startTile = actualSet.Unit.Tile;
            var endTile = target.MoveTile;

            if (startTile != endTile && startTile.Structure != null)
            {
                // If the unit is moving off a structure tile, reset that structure's capture points
                startTile.Structure.ResetCapturePoints();
            }

            var board = actualSet.Unit.Tile.Parent.Battle.GameBoard;
            board.AnimatePath(actualSet.Unit, actualSet.GetMovementMoves(),
                              () =>
                                  {
                                      if (startTile.Unit == actualSet.Unit) startTile.Unit = null;
                                      if (endTile.Unit == null) endTile.Unit = actualSet.Unit;
                                      foreach (var move in actualSet)
                                      {
                                          board.RevealFogOfWar(move.MoveTile, move.UnitToMove);
                                      }
                                      var es = new ExecutionState {StopExecution = trap};
                                      if (trap)
                                      {
                                          board.AddEffect(new PopupEffect
                                                              {
                                                                  Board = board,
                                                                  Time = 500,
                                                                  TileSprites = new Dictionary<Tile, string> { { endTile, "TrapRight" } }
                                                              });
                                          board.Delay(500, () => callback(es));
                                      }
                                      else
                                      {
                                          callback(es);
                                      }
                                  });
        }
    }
}