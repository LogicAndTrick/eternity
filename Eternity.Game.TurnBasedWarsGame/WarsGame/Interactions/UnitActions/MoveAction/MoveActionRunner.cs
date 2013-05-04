using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
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

        public void Execute(Battle battle, GameBoard gameboard, Action<ExecutionState> callback)
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
                // If the unit is moving off a structure tile
                if (startTile.Structure.IsUnderConstruction)
                {
                    // If the structure is being built, delete it
                    startTile.Structure = null;
                }
                else
                {
                    // Otherwise just reset that structure's capture points
                    startTile.Structure.ResetCapturePoints();
                }
            }

            gameboard.AnimatePath(actualSet.Unit, actualSet.GetMovementMoves(),
                              () =>
                                  {
                                      if (startTile.Unit == actualSet.Unit) startTile.SetUnit(battle, null);
                                      if (endTile.Unit == null) endTile.SetUnit(battle, actualSet.Unit);
                                      foreach (var move in actualSet)
                                      {
                                          gameboard.RevealFogOfWar(battle, move.MoveTile, move.UnitToMove);
                                      }
                                      var es = new ExecutionState {StopExecution = trap};
                                      if (trap)
                                      {
                                          gameboard.AddEffect(new PopupEffect
                                                              {
                                                                  Board = gameboard,
                                                                  Time = 500,
                                                                  TileSprites = new Dictionary<Tile, string> { { endTile, "TrapRight" } }
                                                              });
                                          gameboard.Delay(500, () => callback(es));
                                      }
                                      else
                                      {
                                          callback(es);
                                      }
                                  });
        }
    }
}