namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    public class ExecutionState
    {
        public static readonly ExecutionState Empty = new ExecutionState();

        /// <summary>
        /// Set to true if the execution should stop after this action
        /// </summary>
        public bool StopExecution { get; set; }
    }
}