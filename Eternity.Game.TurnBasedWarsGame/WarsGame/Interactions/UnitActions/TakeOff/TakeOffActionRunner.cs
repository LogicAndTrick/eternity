using System;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.TakeOff
{
    public class TakeOffActionRunner : IUnitActionRunner
    {
        private readonly Unit _parent;
        private readonly Unit _child;
        private readonly Tile _moveTile;

        public TakeOffActionRunner(Unit parent, Unit child, Tile moveTile)
        {
            _parent = parent;
            _child = child;
            _moveTile = moveTile;
        }

        public void Execute(Battle battle, GameBoard gameboard, Action<ExecutionState> callback)
        {
            _parent.LoadedUnits.Remove(_child);
            _moveTile.SetUnit(battle, _child);
            callback(ExecutionState.Empty);
        }
    }
}