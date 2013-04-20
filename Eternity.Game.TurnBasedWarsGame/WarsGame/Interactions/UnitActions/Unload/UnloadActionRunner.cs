using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Unload
{
    public class UnloadActionRunner : IUnitActionRunner
    {
        private readonly Unit _parent;
        private readonly Unit _child;
        private readonly Tile _targetTile;

        public UnloadActionRunner(Unit parent, Unit child, Tile targetTile)
        {
            _parent = parent;
            _child = child;
            _targetTile = targetTile;
        }

        public void Execute(Action callback)
        {
            var board = _parent.Tile.Parent.Battle.GameBoard;
            var moveset = new MoveSet(_child, new[]
                                                  {
                                                      Move.CreateMove(_parent.Tile, _child),
                                                      Move.CreateMove(_targetTile, _child)
                                                  });
            board.AnimatePath(_child, moveset,
                              () =>
                                  {
                                      _parent.LoadedUnits.Remove(_child);
                                      _targetTile.Unit = _child;
                                      _child.HasMoved = true;
                                      callback();
                                  });
        }
    }
}