using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns.Actions
{
    /// <summary>
    /// A valid tile used by the unit actions.
    /// Tells the action set what the tile is and what type of move is valid.
    /// </summary>
    public class ValidTile
    {
        public MoveType MoveType { get; set; }
        public Tile Tile { get; set; }
    }
}