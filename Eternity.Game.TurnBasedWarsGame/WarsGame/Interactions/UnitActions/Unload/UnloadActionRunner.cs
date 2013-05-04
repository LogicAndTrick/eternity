using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
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

        public void Execute(Battle battle, GameBoard gameboard, Action<ExecutionState> callback)
        {
            var moveset = new MoveSet(_child, new[]
                                                  {
                                                      Move.CreateMove(_parent.Tile, _child)
                                                  });

            var trap = _targetTile.Unit != null;

            if (!trap) moveset.Add(Move.CreateMove(_targetTile, _child));

            gameboard.AnimatePath(_child, moveset,
                              () =>
                                  {
                                      var es = new ExecutionState {StopExecution = trap};
                                      if (trap)
                                      {
                                          gameboard.AddEffect(new PopupEffect
                                                              {
                                                                  Board = gameboard,
                                                                  Time = 500,
                                                                  TileSprites = new Dictionary<Tile, string> {{_parent.Tile, "TrapRight"}}
                                                              });
                                          gameboard.Delay(500, () => callback(es));
                                      }
                                      else
                                      {
                                          _parent.LoadedUnits.Remove(_child);
                                          _targetTile.SetUnit(battle, _child);
                                          _child.HasMoved = true;
                                          foreach (var move in moveset)
                                          {
                                              gameboard.RevealFogOfWar(battle, move.MoveTile, move.UnitToMove);
                                          }
                                          callback(ExecutionState.Empty);
                                      }
                                  });
        }
    }
}