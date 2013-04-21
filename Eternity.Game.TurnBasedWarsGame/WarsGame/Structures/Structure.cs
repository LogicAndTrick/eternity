using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Structures
{
    public class Structure
    {
        public TerrainRules Rules { get; private set; }
        public StructureType Type { get; private set; }
        public Tile Tile { get; private set; }
        public Army Army { get; set; }
        public int Health { get; private set; }
        public int CapturePoints { get; set; }

        public Structure(Tile tile)
        {
            Tile = tile;
            Rules = RuleSet.GetTerrainRules(tile.Type);

            if (Rules.Capturable) Type = StructureType.Capturable;
            else if (Rules.Destroyable) Type = StructureType.Destroyable;
            else Type = StructureType.Static;

            Army = null; // Neutral to start, TODO: Fix this
            Health = 100;
            CapturePoints = 20;
        }

        public void Capture(Army army)
        {
            ResetCapturePoints();
            Army = army;
            var colour = army == null ? "Neutral" : army.ArmyRules.Colour;
            Tile.BaseGroups.RemoveLayer("Terrain", "TerrainOverlay");
            Tile.BaseGroups.AddLayer("Terrain", "TerrainOverlay", colour+Tile.Type);
        }

        public void ResetCapturePoints()
        {
            CapturePoints = 20;
        }

        public bool IsBeingCaptured()
        {
            return CapturePoints != 20;
        }
    }
}