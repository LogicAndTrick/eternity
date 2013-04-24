using System;

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
        /// <param name="callback">The callback to run once the action is complete.</param>
        void Execute(Action<ExecutionState> callback);
    }
}