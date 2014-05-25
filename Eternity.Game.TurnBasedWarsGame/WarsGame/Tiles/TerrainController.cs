using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles
{
    public static class TerrainController
    {
        private static Random _rand;

        static TerrainController()
        {
            _rand = new Random();
        }

        public static void ChangeTerrainType(Tile tile, TileType type)
        {
            ChangeTerrainType(tile, type, GetRandomTerrainStyle(type));
        }

        public static string GetRandomTerrainStyle(TileType type)
        {
            var defs = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
            {
                {"Group", "Terrain"},
                {"Type", "Terrain"},
                {"TerrainType", type.ToString()}
            });
            var styles = new List<string>();
            foreach (var rd in defs)
            {
                var freq = rd.GetData("TerrainFrequency", "1");
                int num;
                if (!int.TryParse(freq, out num)) continue;
                for (var i = 0; i < num; i++) styles.Add(rd.GetData("Name"));
            }
            var random = _rand.Next(styles.Count);
            return styles[random];
        }

        public static void ChangeTerrainType(Tile tile, TileType type, string style)
        {
            // todo structures
            // todo units

            tile.Type = type;
            style = GetCorrectTerrainStyle(tile, style);
            var res = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
            {
                {"Group", "Terrain"},
                {"Type", "Terrain"},
                {"TerrainType", type.ToString()},
                {"Name", style}
            }).First();
            var underlay = res.GetData("Underlay");

            tile.UpdateTerrain(underlay, style, null);

            // todo neighbours

            tile.CalculateOverlays();
            tile.GetNeighbouringTiles().ToList().ForEach(x => x.CalculateOverlays());
        }

        public static string GetCorrectTerrainStyle(Tile tile, string defaultStyle)
        {
            var n = tile.North == null ? TileType.Plain : tile.North.Type;
            var s = tile.South == null ? TileType.Plain : tile.South.Type;
            var e = tile.East == null ? TileType.Plain : tile.East.Type;
            var w = tile.West == null ? TileType.Plain : tile.West.Type;
            var suffix = "";
            switch (tile.Type)
            {
                case TileType.Wood:
                    defaultStyle = "Wood1";
                    if (e == TileType.Wood && w == TileType.Wood) defaultStyle = "Wood3";
                    else if (e == TileType.Wood) defaultStyle = "Wood2";
                    else if (w == TileType.Wood) defaultStyle = "Wood4";
                    break;
                case TileType.Mountain:
                    defaultStyle = "Mountain";
                    if (e == TileType.Mountain && w == TileType.Mountain) defaultStyle = "MountainEW";
                    else if (e == TileType.Mountain) defaultStyle = "MountainE";
                    else if (w == TileType.Mountain) defaultStyle = "MountainW";
                    break;
                case TileType.Road:
                    //todo edges & standalone
                    var prefix = defaultStyle.Substring(0, 5);
                    if (e == TileType.Road) suffix += "E";
                    if (w == TileType.Road) suffix += "W";
                    if (n == TileType.Road) suffix += "N";
                    if (s == TileType.Road) suffix += "S";
                    if (suffix.Length == 0) suffix = "None";
                    defaultStyle = prefix + suffix;
                    break;
                case TileType.Bridge:
                    // todo standalone
                    var nonBridge = new List<TileType> { TileType.FoggySea, TileType.Port, TileType.Reef, TileType.River, TileType.RoughSea, TileType.Sea, TileType.Shoal, TileType.TemporaryPort };
                    if (!nonBridge.Contains(e)) suffix += "E";
                    if (!nonBridge.Contains(w)) suffix += "W";
                    if (!nonBridge.Contains(n)) suffix += "N";
                    if (!nonBridge.Contains(s)) suffix += "S";
                    if (suffix.Length == 0) suffix = "None";
                    if (suffix == "E" || suffix == "W") suffix = "EW";
                    if (suffix == "N" || suffix == "S") suffix += "NS";
                    defaultStyle = "Bridge" + suffix;
                    break;
                case TileType.Pipe:
                    //todo.
                    break;
                case TileType.PipeSeam:
                    //todo.
                    break;
                case TileType.River:
                    if (e == TileType.Road || e == TileType.Bridge) suffix += "E";
                    if (w == TileType.Road || w == TileType.Bridge) suffix += "W";
                    if (n == TileType.Road || n == TileType.Bridge) suffix += "N";
                    if (s == TileType.Road || s == TileType.Bridge) suffix += "S";
                    if (suffix.Length == 0) suffix = "None";
                    defaultStyle = "River" + suffix;
                    break;
                case TileType.Shoal:
                    break;
                case TileType.FoggySea:
                    break;
                case TileType.Meteor:
                    break;
                case TileType.Plasma:
                    break;
            }
            return defaultStyle;
        }
    }
}
