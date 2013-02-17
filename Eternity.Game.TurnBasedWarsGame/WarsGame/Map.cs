using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Structures;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame
{
    public class Map
    {
        public Battle Battle { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<Tile> Tiles { get; private set; }

        public Map(int width, int height, List<Tile> tiles)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
        }

        public Map(Battle battle, ResourceDefinition def)
        {
            Battle = battle;
            Width = int.Parse(def.GetData("Width"));
            Height = int.Parse(def.GetData("Height"));
            Tiles = new List<Tile>();
            foreach (var rd in def.ChildrenDefinitions.Where(rd => rd.DefinitionType == "Tile"))
            {
                var locSpl = rd.GetData("Location").Split(' ');
                var location = new Point(int.Parse(locSpl[0]), int.Parse(locSpl[1]));
                TileType tt;
                Enum.TryParse(rd.GetData("TerrainType"), out tt);
                var style = rd.GetData("Base");
                var overlay = rd.GetData("Overlay");
                var baseResource = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
                                                                   {
                                                                       {"Type", "Terrain"},
                                                                       {"Name", style}
                                                                   }).FirstOrDefault();
                var underlay = baseResource == null ? "" : baseResource.GetData("Underlay");

                var tile = new Tile(this, tt, location);
                if (underlay != "") tile.BaseGroups.AddLayer("Terrain", "TerrainUnderlay", underlay);
                tile.BaseGroups.AddLayer("Terrain", "TerrainBase", style);
                if (overlay != "") tile.BaseGroups.AddLayer("Terrain", "TerrainOverlay", overlay);

                var structure = rd.ChildrenDefinitions.FirstOrDefault(x => x.DefinitionType == "Structure");
                if (structure != null)
                {
                    var army = structure.GetData("Army");
                    var tstructure = new Structure(tile) {Army = battle.GetArmy(army)};
                    tile.Structure = tstructure;
                }

                var unit = rd.ChildrenDefinitions.FirstOrDefault(x => x.DefinitionType == "Unit");
                if (unit != null)
                {
                    var army = unit.GetData("Army");
                    var unitRd = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
                                                                    {
                                                                        {"Type", "Unit"},
                                                                        {"Name", unit.GetData("Style")},
                                                                        {"UnitType", unit.GetData("UnitType")}
                                                                    }).FirstOrDefault();
                    if (unitRd != null)
                    {
                        var tunit = new Unit(battle.GetArmy(army), unitRd);
                        var hlth = unit.GetData("Health");
                        if (!String.IsNullOrWhiteSpace(hlth)) tunit.CurrentHealth = int.Parse(hlth);
                        tile.Unit = tunit;
                    }
                }
                Tiles.Add(tile);
            }
            Tiles.Sort();
            Tiles.ForEach(x => x.CalculateOverlays());
        }

        public Tile GetTile(int x, int y)
        {
            return GetTile(new Point(x, y));
        }

        public Tile GetTile(Point p)
        {
            return Tiles.FirstOrDefault(x => x.Location.Equals(p));
        }
    }
}
