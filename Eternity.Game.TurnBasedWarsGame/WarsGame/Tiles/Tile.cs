﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Structures;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Graphics.Sprites;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles
{
    public class Tile : IComparable<Tile>, IEquatable<Tile>
    {
        public class TileGroupCollection
        {
            public List<TileGroup> Groups { get; private set; }

            public TileGroupCollection()
            {
                Groups = new List<TileGroup>();
            }

            public void RemoveLayers(string groupName)
            {
                foreach (var g in Groups.Where(x => x.GroupName == groupName))
                {
                    g.Layers.Clear();
                }
            }

            public void RemoveLayer(string groupName, string layerName)
            {
                foreach (var g in Groups.Where(x => x.GroupName == groupName))
                {
                    g.Layers.RemoveAll(x => x.LayerName == layerName);
                }
            }

            public void AddLayer(string groupName, string layerName, string name, SpriteDrawingOptions opt = null)
            {
                var g = Groups.FirstOrDefault(x => x.GroupName == groupName);
                if (g != null)
                {
                    g.Layers.Add(new TileLayer(layerName, name, opt));
                }
            }

            public void SetGroupVisibility(string groupName, bool visible)
            {
                var g = Groups.FirstOrDefault(x => x.GroupName == groupName);
                if (g != null)
                {
                    g.Visible = visible;
                }
            }
        }

        public class TileGroup
        {
            public TileGroup(string groupName, string spriteGroup)
            {
                GroupName = groupName;
                SpriteGroup = spriteGroup;
                Layers = new List<TileLayer>();
                Visible = true;
            }

            public string GroupName { get; set; }
            public string SpriteGroup { get; set; }
            public List<TileLayer> Layers { get; set; }
            public bool Visible { get; set; }
        }

        public class TileLayer
        {
            public string LayerName { get; set; }
            public string SpriteName { get; set; }
            public SpriteDrawingOptions DrawingOptions { get; set; }

            public TileLayer(string layerName, string spriteName, SpriteDrawingOptions drawingOptions = null)
            {
                LayerName = layerName;
                SpriteName = spriteName;
                DrawingOptions = drawingOptions;
            }
        }

        public TileGroupCollection BaseGroups { get; private set; }
        public TileGroupCollection OverlayGroups { get; private set; }
        
        public Map Parent { get; private set; }
        public TileType Type { get; private set; }
        public Point Location { get; private set; }

        public bool CanMoveTo { get; set; }
        public bool CanAttack { get; set; }

        public Structure Structure { get; set; }

        private Unit _unit;
        public Unit Unit
        {
            get { return _unit; }
            set
            {
                if (_unit != null)
                {
                    _unit.Tile = null;
                }
                _unit = value;
                if (_unit != null)
                {
                    _unit.Tile = this;
                }
                UpdateUnitLayers();
            }
        }


        public void UpdateUnitLayers()
        {
            BaseGroups.RemoveLayers("Unit");
            OverlayGroups.RemoveLayers("UnitHealth");
            OverlayGroups.RemoveLayers("UnitStatus");
            if (_unit == null) return;

            BaseGroups.AddLayer("Unit", "Unit", _unit.Style,
                new SpriteDrawingOptions { MirrorX = _unit.Army.ArmyRules.MirrorX });
            var num = _unit.HealthOutOfTen;
            if (num < 10)
            {
                OverlayGroups.AddLayer("UnitHealth", "Health", num.ToString());
            }

            if (Unit.Tile != null && Unit.Tile.Structure != null && Unit.Tile.Structure.IsBeingCaptured())
            {
                OverlayGroups.AddLayer("UnitHealth", "Capture", "Flag",
                                       new SpriteDrawingOptions {DockX = SpriteDrawingOptions.Dock.Left});
            }
        }

        #region Tile Getters
        public Tile East
        {
            get { return Parent.GetTile(Location.X + 1, Location.Y); }
        }

        public Tile West
        {
            get { return Parent.GetTile(Location.X - 1, Location.Y); }
        }

        public Tile North
        {
            get { return Parent.GetTile(Location.X, Location.Y - 1); }
        }

        public Tile South
        {
            get { return Parent.GetTile(Location.X, Location.Y + 1); }
        }

        public Tile SouthEast
        {
            get { return Parent.GetTile(Location.X + 1, Location.Y + 1); }
        }

        public Tile NorthWest
        {
            get { return Parent.GetTile(Location.X - 1, Location.Y - 1); }
        }

        public Tile NorthEast
        {
            get { return Parent.GetTile(Location.X + 1, Location.Y - 1); }
        }

        public Tile SouthWest
        {
            get { return Parent.GetTile(Location.X - 1, Location.Y + 1); }
        }

        public IEnumerable<Tile> GetAdjacentTiles()
        {
            return new[] {North, South, East, West}.Where(x => x != null);
        }
        #endregion

        public Tile(Map parent, TileType type, Point location)
        {
            Parent = parent;
            Type = type;
            Location = location;
            BaseGroups = new TileGroupCollection();
            BaseGroups.Groups.Add(new TileGroup("Terrain", "Terrain"));
            BaseGroups.Groups.Add(new TileGroup("Unit", "Units"));
            OverlayGroups = new TileGroupCollection();
            OverlayGroups.Groups.Add(new TileGroup("TerrainOverlays", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("UnitHealth", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("UnitStatus", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("Highlight", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("Arrow", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("UnitAnimations", "UnitAnimations"));
        }

        private static TileType GetType(Tile t, TileType def)
        {
            return t == null ? def : t.Type;
        }

        public void CalculateOverlays()
        {
            var n = GetType(North, TileType.Sea);
            var w = GetType(West, TileType.Sea);
            var nw = GetType(NorthWest, TileType.Sea);

            if (n == TileType.Mountain) OverlayGroups.AddLayer("TerrainOverlays", "MountainOverlay", "GreyOverlayC2");
            if (nw == TileType.Mountain) OverlayGroups.AddLayer("TerrainOverlays", "MountainOverlay", "GreyOverlayC1");
            if (w == TileType.Mountain) OverlayGroups.AddLayer("TerrainOverlays", "MountainOverlay", "GreyOverlayC3");
            if (Type == TileType.Mountain) OverlayGroups.AddLayer("TerrainOverlays", "MountainOverlay", "GreyOverlayC4");

            if (Type != TileType.Sea) return;

            var s = GetType(South, TileType.Sea);
            var e = GetType(East, TileType.Sea);
            var ne = GetType(NorthEast, TileType.Sea);
            var sw = GetType(SouthWest, TileType.Sea);
            var se = GetType(SouthEast, TileType.Sea);

            var nsea = n == TileType.Sea;
            var ssea = s == TileType.Sea;
            var esea = e == TileType.Sea;
            var wsea = w == TileType.Sea;

            var nwsea = nw == TileType.Sea;
            var nesea = ne == TileType.Sea;
            var swsea = sw == TileType.Sea;
            var sesea = se == TileType.Sea;

            var nall = nsea && nesea && nwsea;
            var sall = ssea && sesea && swsea;
            var wall = wsea && nwsea && swsea;
            var eall = esea && nesea && sesea;

            var nno = (nsea ? 0 : 1) + (nesea ? 0 : 1) + (nwsea ? 0 : 1);
            var sno = (ssea ? 0 : 1) + (sesea ? 0 : 1) + (swsea ? 0 : 1);
            var wno = (wsea ? 0 : 1) + (nwsea ? 0 : 1) + (swsea ? 0 : 1);
            var eno = (esea ? 0 : 1) + (nesea ? 0 : 1) + (sesea ? 0 : 1);

            if (nsea && wsea && esea && ssea && nwsea && nesea && swsea && sesea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayFull");

            else if ((!wsea || wno > 1) && nsea && ssea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayE");
            else if ((!esea || eno > 1) && nsea && ssea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayW");
            else if ((!ssea || sno > 1) && esea && wsea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayN");
            else if ((!nsea || nno > 1) && esea && wsea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayS");

            else if (eall && sall && nsea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayA1");
            else if (wall && sall && nsea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayA2");
            else if (eall && nall && ssea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayA3");
            else if (wall && nall && ssea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayA4");

            else if (nsea && wsea && nwsea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayR1");
            else if (nsea && esea && nesea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayR2");
            else if (wsea && ssea && swsea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayR3");
            else if (esea && ssea && sesea) OverlayGroups.AddLayer("TerrainOverlays", "OceanOverlay", "BlueOverlayR4");
        }

        public int DistanceFrom(Tile other)
        {
            return Math.Abs(Location.X - other.Location.X) + Math.Abs(Location.Y - other.Location.Y);
        }

        public int CompareTo(Tile other)
        {
            var c = Location.X.CompareTo(other.Location.X);
            return c != 0 ? c : Location.Y.CompareTo(other.Location.Y);
        }

        public bool Equals(Tile other)
        {
            return Location.Equals(other.Location);
        }

        public override string ToString()
        {
            var sb = new StringBuilder("Tile\n{\n\tLocation: ");
            sb.Append(Location.X);
            sb.Append(" ");
            sb.Append(Location.Y);
            sb.Append("\n\tTerrainType: ");
            sb.Append(Type);
            if (Unit != null)
            {
                sb.Append("\n\n\tUnit\n\t{\n\t\tUnitType: ");
                sb.Append(Unit.UnitType);
                sb.Append("\n\t\tArmy: OS");
                sb.Append("\n\t\tStyle: ");
                sb.Append(Unit.Style);
                sb.Append("\n\t}");
            }
            sb.Append("\n}\n");
            return sb.ToString();
        }
    }
}
