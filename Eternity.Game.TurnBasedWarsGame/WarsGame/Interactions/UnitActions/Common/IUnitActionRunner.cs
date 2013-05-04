using System;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    /// <summary>
    /// Runs an action.
    /// </summary>
    public interface IUnitActionRunner
    {
        /// <summary>
        /// Execute and animate the action. Runs asynchronously.
        /// </summary>
        /// <param name="battle"> </param>
        /// <param name="gameboard"> </param>
        /// <param name="callback">The callback to run once the action is complete.</param>
        void Execute(Battle battle, GameBoard gameboard, Action<ExecutionState> callback);
    }
}