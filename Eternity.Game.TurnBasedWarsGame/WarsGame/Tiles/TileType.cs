namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles
{
    public static class TileTypeExtensions
    {
        public static bool IsCapturable(this TileType tile)
        {
            return
                tile == TileType.Headquarters
                || tile == TileType.City
                || tile == TileType.Base
                || tile == TileType.Airport
                || tile == TileType.TemporaryAirport
                || tile == TileType.Port
                || tile == TileType.TemporaryPort
                || tile == TileType.CommsTower
                || tile == TileType.Radar;
        }
    }
    public enum TileType
    {
        // Ground Terrain
        Plain,
        Wood,
        Mountain,
        Wasteland,
        Ruins,
        Road,
        Bridge,

        // Pipes
        Pipe,
        PipeSeam,
        DestroyedPipeSeam,

        // Watery Stuff
        River,
        Shoal,
        Sea,
        RoughSea,
        FoggySea,
        Reef,

        // Special
        Silo, // Fires ballistic missiles. You know the drill.
        Fire, // Non-traversable. Reveals FOW to... 2? 3? squares.
        Meteor, // Non-traversable, destroyable. Sometimes used with plasma walls.
        Plasma, // Non-traversable. Destroy the meteors at each end of the wall to destroy plasma.

        // Properties
        Headquarters,
        City,
        Base,
        Airport,
        TemporaryAirport,
        Port,
        TemporaryPort,
        CommsTower,
        Radar,
    }
}
