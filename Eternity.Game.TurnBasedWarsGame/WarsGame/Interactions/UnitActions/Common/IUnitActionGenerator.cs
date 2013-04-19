using System.Collections.Generic;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    /// <summary>
    /// A unit action generator. Provides the actions with state when required.
    /// </summary>
    public interface IUnitActionGenerator
    {
        /// <summary>
        /// True if this action is valid given the current action set.
        /// </summary>
        /// <param name="queue">The current context queue</param>
        /// <returns>True if this action is allowed at this point</returns>
        bool IsValidFor(ContextQueue queue);

        /// <summary>
        /// Get all the available actions for this generator given the current action set.
        /// </summary>
        /// <param name="queue">The current context queue</param>
        /// <returns>The list of available actions</returns>
        IEnumerable<IUnitAction> GetActions(ContextQueue queue);
    }
}