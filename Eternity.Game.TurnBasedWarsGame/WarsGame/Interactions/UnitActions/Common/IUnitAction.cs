using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    /// <summary>
    /// A unit action such as move, fire, wait, etc.
    /// </summary>
    public interface IUnitAction
    {
        /// <summary>
        /// True if the given tile is valid for the current action set.
        /// </summary>
        /// <param name="tile">The tile to test</param>
        /// <returns>True if this action is valid for the given tile</returns>
        bool IsValidTile(Tile tile);

        /// <summary>
        /// Get a list of valid tiles for the given start tile and action set.
        /// </summary>
        /// <returns>A list of valid tiles and their states</returns>
        List<ValidTile> GetValidTiles();

        /// <summary>
        /// Update an action set's current move set to point to the given tile.
        /// </summary>
        /// <param name="battle"> </param>
        /// <param name="gameboard"> </param>
        /// <param name="tile">The tile to point the move set to</param>
        void UpdateMoveSet(Battle battle, GameBoard gameboard, Tile tile);

        /// <summary>
        /// Get the action's move set, if it has one.
        /// </summary>
        /// <returns>The action's move set or null if no move set exists.</returns>
        MoveSet GetMoveSet();

        /// <summary>
        /// Clear any effects that this action may have applied.
        /// </summary>
        /// <param name="battle"> </param>
        /// <param name="gameboard"> </param>
        void ClearEffects(Battle battle, GameBoard gameboard);

        /// <summary>
        /// Cancel this action and undo any state that it may have set.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Get the name of this action.
        /// </summary>
        /// <returns>The action's name</returns>
        string GetName();

        /// <summary>
        /// Create the states required to execute this action.
        /// </summary>
        /// <returns>A list of states.</returns>
        IEnumerable<ContextState> CreateContextStates();

        /// <summary>
        /// Returns true if this action has no preview mode and executes instantly.
        /// </summary>
        /// <returns>True if this action is instant</returns>
        bool IsInstantAction();

        /// <summary>
        /// Returns true if this action is final and commits the action after selection.
        /// </summary>
        /// <returns>True if this action is committing</returns>
        bool IsCommittingAction();
    }
}
