using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns.Actions
{
    /// <summary>
    /// A unit action such as move, fire, wait, etc.
    /// </summary>
    public interface IUnitAction
    {
        UnitActionType ActionType { get; }

        /// <summary>
        /// Execute and animate the action. Runs asynchronously.
        /// </summary>
        /// <param name="set">The current action set.</param>
        /// <param name="callback">The callback to run once the action is complete.</param>
        void Execute(UnitActionSet set, Action callback);

        /// <summary>
        /// True if the given tile is valid for the current action set.
        /// </summary>
        /// <param name="tile">The tile to test</param>
        /// <param name="set">The current action set</param>
        /// <returns>True if this action is valid for the given tile</returns>
        bool IsValidTile(Tile tile, UnitActionSet set);

        /// <summary>
        /// Get a list of valid tiles for the given start tile and action set.
        /// </summary>
        /// <param name="set">The current action set</param>
        /// <returns>A list of valid tiles and their states</returns>
        List<ValidTile> GetValidTiles(UnitActionSet set);

        /// <summary>
        /// Update an action set's current move set to point to the given tile.
        /// </summary>
        /// <param name="tile">The tile to point the move set to</param>
        /// <param name="set">The current action set</param>
        void UpdateMoveSet(Tile tile, UnitActionSet set);

        /// <summary>
        /// Cancel this action and undo any state that it may have set.
        /// </summary>
        /// <param name="set">The current action set</param>
        void Cancel(UnitActionSet set);

        /// <summary>
        /// Get the name of this action.
        /// </summary>
        /// <returns>The action's name</returns>
        string GetName();

        /// <summary>
        /// Returns true if this action has no preview mode and executes instantly.
        /// </summary>
        /// <param name="set">The current action set</param>
        /// <returns>True if this action is instant</returns>
        bool IsInstantAction(UnitActionSet set);

        /// <summary>
        /// Returns true if this action is final and commits the action after selection.
        /// </summary>
        /// <param name="set">The current action set</param>
        /// <returns>True if this action is committing</returns>
        bool IsCommittingAction(UnitActionSet set);
    }
}
