using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Structures;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Graphics.Sprites;
using Point = Eternity.DataStructures.Primitives.Point;

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

            public TileLayer GetLayer(string groupName, string layerName)
            {
                return Groups.Where(x => x.GroupName == groupName)
                    .SelectMany(x => x.Layers)
                    .FirstOrDefault(x => x.LayerName == layerName);
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
                DrawingOptions = drawingOptions ?? new SpriteDrawingOptions();
            }
        }

        public TileGroupCollection BaseGroups { get; private set; }
        public TileGroupCollection OverlayGroups { get; private set; }
        
        public Map Parent { get; private set; }

        public Point Location { get; private set; }

        public bool CanMoveTo { get; set; }
        public bool CanAttack { get; set; }

        public Structure Structure { get; set; }
        public TerrainRules Rules { get; private set; }

        private Unit _unit;
        private bool _fog;
        private TileType _type;

        public Unit Unit
        {
            get { return _unit; }
        }

        public void SetUnit(Battle battle, Unit unit)
        {
            if (_unit != null)
            {
                _unit.Tile = null;
            }
            _unit = unit;
            if (_unit != null)
            {
                _unit.Tile = this;
            }
            UpdateUnitLayers(battle);
        }

        public TileType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                if (Structure != null)
                {
                    Structure.Tile = this;
                }
            }
        }

        public bool Fog
        {
            get { return _fog; }
        }

        public void SetFog(Battle battle, bool fog)
        {
            if (_fog == fog) return;
            BaseGroups.Groups.First(x => x.GroupName == "Terrain").Layers.ForEach(x => x.DrawingOptions.Colour = Color.White);
            if (Structure != null && !Structure.IsUnderConstruction)
            {
                var colour = Structure.Army == null ? "Neutral" : Structure.Army.ArmyRules.Colour;
                BaseGroups.RemoveLayer("Terrain", "TerrainOverlay");
                BaseGroups.AddLayer("Terrain", "TerrainOverlay", colour + Type);
            }
            _fog = fog;
            if (_fog)
            {
                if (Structure != null && Type != TileType.Headquarters && !Structure.IsUnderConstruction)
                {
                    BaseGroups.RemoveLayer("Terrain", "TerrainOverlay");
                    BaseGroups.AddLayer("Terrain", "TerrainOverlay", "Neutral" + Type);
                }
                BaseGroups.Groups.First(x => x.GroupName == "Terrain").Layers.ForEach(x => x.DrawingOptions.Colour = Color.FromArgb(128, 128, 128));
            }
            UpdateUnitLayers(battle);
        }

        public bool ShouldHaveFog(Army army)
        {
            if (Unit != null && Unit.Army == army) return false;
            if (Structure != null && Structure.Army == army && !Structure.IsUnderConstruction) return false;
            return true;
        }

        public bool HasVisibleUnit(Army army)
        {
            return !Fog && Unit != null && (!Unit.IsHidden || Unit.Army == army);
        }

        public void UpdateUnitLayers(Battle battle)
        {
            BaseGroups.RemoveLayers("Unit");
            OverlayGroups.RemoveLayers("UnitHealth");
            OverlayGroups.RemoveLayers("UnitStatus");
            if (Unit == null || (battle != null && !HasVisibleUnit(battle.CurrentTurn.Army))) return;

            BaseGroups.AddLayer("Unit", "Unit", _unit.Style,
                new SpriteDrawingOptions { MirrorX = _unit.Army.ArmyRules.MirrorX, Colour = _unit.HasMoved ? Color.Gray : Color.White });
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
            return new[] { North, South, East, West }.Where(x => x != null);
        }

        public IEnumerable<Tile> GetNeighbouringTiles()
        {
            return new[] { North, South, East, West, NorthWest, NorthEast, SouthWest, SouthEast }.Where(x => x != null);
        }
        #endregion

        public Tile(Map parent, TileType type, Point location)
        {
            Parent = parent;
            Type = type;
            Location = location;
            Rules = RuleSet.GetTerrainRules(Type);

            BaseGroups = new TileGroupCollection();
            BaseGroups.Groups.Add(new TileGroup("Terrain", "Terrain"));
            BaseGroups.Groups.Add(new TileGroup("Unit", "Units"));
            OverlayGroups = new TileGroupCollection();
            OverlayGroups.Groups.Add(new TileGroup("TerrainOverlays", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("UnitHealth", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("UnitStatus", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("Highlight", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("RangeCursor", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("Arrow", "Overlays"));
            OverlayGroups.Groups.Add(new TileGroup("UnitAnimations", "UnitAnimations"));
        }

        public void UpdateTerrain(string underlay, string style, string overlay)
        {
            BaseGroups.RemoveLayer("Terrain", "TerrainUnderlay");
            if (!String.IsNullOrWhiteSpace(underlay))
            {
                BaseGroups.AddLayer("Terrain", "TerrainUnderlay", underlay);
            }
            BaseGroups.RemoveLayer("Terrain", "TerrainBase");
            if (!String.IsNullOrWhiteSpace(style))
            {
                BaseGroups.AddLayer("Terrain", "TerrainBase", style);
            }
            BaseGroups.RemoveLayer("Terrain", "TerrainOverlay");
            if (!String.IsNullOrWhiteSpace(overlay))
            {
                BaseGroups.AddLayer("Terrain", "TerrainOverlay", overlay);
            }
        }

        public void ClearHighlight()
        {
            OverlayGroups.RemoveLayers("Highlight");
        }

        public void ClearArrow()
        {
            OverlayGroups.RemoveLayers("Arrow");
        }

        public void ClearUnitAnimation()
        {
            OverlayGroups.RemoveLayers("UnitAnimations");
        }

        public void ClearRangeCursor()
        {
            OverlayGroups.RemoveLayers("RangeCursor");
        }

        public void AddHighlight(string style)
        {
            OverlayGroups.AddLayer("Highlight", "Highlight", style);
        }

        public void AddArrow(string style)
        {
            OverlayGroups.AddLayer("Arrow", "Arrow", style,
                                   new SpriteDrawingOptions
                                       {
                                           DockX = SpriteDrawingOptions.Dock.Center,
                                           DockY = SpriteDrawingOptions.Dock.Center
                                       });
        }

        public void AddUnitAnimation(string style)
        {
            OverlayGroups.AddLayer("UnitAnimations", "Animation", style + "W_24",
                                   new SpriteDrawingOptions {DockX = SpriteDrawingOptions.Dock.Center, MirrorX = true});
        }

        public void AddRangeCursor(string style, bool mirrorx, bool mirrory)
        {
            var options = new SpriteDrawingOptions { MirrorX = mirrorx, MirrorY = mirrory };
            OverlayGroups.AddLayer("RangeCursor", "RangeCursor", style, options);
        }

        public void HideUnit()
        {
            BaseGroups.SetGroupVisibility("Unit", false);
            OverlayGroups.SetGroupVisibility("UnitHealth", false);
            OverlayGroups.SetGroupVisibility("UnitStatus", false);
        }

        public void ShowUnit()
        {
            BaseGroups.SetGroupVisibility("Unit", true);
            OverlayGroups.SetGroupVisibility("UnitHealth", true);
            OverlayGroups.SetGroupVisibility("UnitStatus", true);
        }

        private static TileType GetType(Tile t, TileType def)
        {
            return t == null ? def : t.Type;
        }

        public void CalculateOverlays()
        {
            OverlayGroups.RemoveLayer("TerrainOverlays", "MountainOverlay");
            OverlayGroups.RemoveLayer("TerrainOverlays", "OceanOverlay");

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

        public double DistanceFrom(Tile other)
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
