using System;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Wait
{
    public class WaitActionRunner : IUnitActionRunner
    {
        public void Execute(Battle battle, GameBoard gameboard, Action<ExecutionState> callback)
        {
            callback(ExecutionState.Empty);
        }
    }
}