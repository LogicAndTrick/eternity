using System;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Move
{
    public class MoveActionRunner : IUnitActionRunner
    {
        private readonly MoveSet _set;

        public MoveActionRunner(MoveSet set)
        {
            _set = set;
        }

        public void Execute(Action callback)
        {
            var target = _set.LastOrDefault(x => x.MoveType == MoveType.Move)
                         ?? Common.Move.CreateMove(_set.Unit.Tile, _set.Unit);
            var startTile = _set.Unit.Tile;
            var endTile = target.MoveTile;
            var board = _set.Unit.Tile.Parent.Battle.GameBoard;
            if (startTile != endTile && startTile.Structure != null) startTile.Structure.ResetCapturePoints();
            board.AnimatePath(_set.Unit, _set.GetMovementMoves(),
                              () =>
                                  {
                                      startTile.Unit = null;
                                      if (endTile.Unit == null) endTile.Unit = _set.Unit;
                                      callback();
                                  });
        }
    }
}