using System.Collections.Generic;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions.Actions
{
    /// <summary>
    /// A unit action generator. Provides the actions with state when required.
    /// </summary>
    public interface IUnitActionGenerator
    {
        UnitActionType ActionType { get; }

        /// <summary>
        /// True if this action is valid given the current action set.
        /// </summary>
        /// <param name="set">The current action set</param>
        /// <returns>True if this action is allowed at this point</returns>
        bool IsValidFor(UnitActionSet set);

        /// <summary>
        /// Get all the available actions for this generator given the current action set.
        /// </summary>
        /// <param name="set">The current action set</param>
        /// <returns>The list of available actions</returns>
        IEnumerable<IUnitAction> GetActions(UnitActionSet set);
    }
}