using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns
{
    /// <summary>
    /// A tile interaction set is a transaction wrapped around tile input.
    /// Tile interactions should know about the battle and call
    /// <code>Battle.EndUnitAction()</code> when complete.
    /// </summary>
    public interface ITileInteractionSet
    {
        /// <summary>
        /// A mouse button has been released on a tile
        /// </summary>
        /// <param name="e">The mouse up event</param>
        /// <param name="tile">The tile that the event applies to</param>
        void TileMouseUp(EternityEvent e, Tile tile);

        /// <summary>
        /// The mouse is hovering over a tile
        /// </summary>
        /// <param name="tile">The tile that the event applies to</param>
        void TileHovered(Tile tile);

        /// <summary>
        /// A mouse button has been pressed on a tile
        /// </summary>
        /// <param name="e">The mouse down event</param>
        /// <param name="tile">The tile that the event applies to</param>
        void TileMouseDown(EternityEvent e, Tile tile);

        /// <summary>
        /// Finish the interaction and clear all state.
        /// </summary>
        void Complete();
    }
}