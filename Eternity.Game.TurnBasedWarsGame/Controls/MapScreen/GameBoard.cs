using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Algorithms;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Easings;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Structures;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;
using Eternity.Graphics.Textures;
using Eternity.Input;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class GameBoard : LayoutControl
    {
        public class TileEventArgs : EventArgs
        {
            public Tile Tile { get; set; }
            public EternityEvent Event { get; set; }
        }

        public event EventHandler<TileEventArgs> TileHovered;
        public event EventHandler<TileEventArgs> TileMouseUp;
        public event EventHandler<TileEventArgs> TileMouseDown;

        private void OnTileHovered(Tile tile, EternityEvent e)
        {
            if (TileHovered != null)
            {
                TileHovered.Invoke(this, new TileEventArgs { Tile = tile, Event = e });
            }
        }

        private void OnTileMouseUp(Tile tile, EternityEvent e)
        {
            if (TileMouseUp != null)
            {
                TileMouseUp.Invoke(this, new TileEventArgs { Tile = tile, Event = e });
            }
        }

        private void OnTileMouseDown(Tile tile, EternityEvent e)
        {
            if (TileMouseDown != null)
            {
                TileMouseDown.Invoke(this, new TileEventArgs { Tile = tile, Event = e });
            }
        }

        public Map Map { get; private set; }
        public int GridSize { get; private set; }

        private readonly Dictionary<MoveSet, Animation<double>> _pathAnimations; 

        public GameBoard(Map map) : base(new GridLayout(map.Width, map.Height))
        {
            Map = map;
            GridSize = 24;
            Box = new Box(0, 0, Map.Width * GridSize, Map.Height * GridSize);
            _pathAnimations = new Dictionary<MoveSet, Animation<double>>();

            var bgi = new ScrollingBackgroundImage(TextureManager.GetTexture("Overlays", "Clouds"), 5, 5, 0.03, -0.06);
            AddOverlay(bgi);
        }

        public MenuDialog ShowDialog(params MenuDialog.MenuDialogAction[] actions)
        {
            return ShowDialog(new Box(new Point(Box.Width / 2, Box.Height / 2), Size.Zero), actions);
        }

        public MenuDialog ShowDialog(Tile tile, params MenuDialog.MenuDialogAction[] actions)
        {
            return ShowDialog(GetTileControl(tile.Location).Box, actions);
        }

        public MenuDialog ShowDialog(Box box, params MenuDialog.MenuDialogAction[] actions)
        {
            var dialog = new MenuDialog(Box, box, actions);
            AddOverlay(dialog);
            return dialog;
        }

        public bool HasDialog()
        {
            return GetAllChildren().Any(x => x is MenuDialog);
        }

        public void HideDialog()
        {
            Remove(x => x is MenuDialog);
        }

        public void DeselectUnit(Unit u)
        {
            foreach (var tile in Map.Tiles)
            {
                tile.ClearHighlight();
                tile.ClearArrow();
                tile.ClearUnitAnimation();
                if (u != null && tile.Unit == u)
                {
                    tile.ShowUnit();
                }
            }
        }

        public void SelectUnit(Unit u, Tile t = null)
        {
            if (t == null) t = u.Tile;
            if (t == u.Tile) t.HideUnit();
            t.AddUnitAnimation(u.Style);
        }

        public void UpdateTileHighlights()
        {
            Map.Tiles.ForEach(x => x.ClearHighlight());
            foreach (var tile in Map.Tiles.Where(x => x.CanMoveTo || x.CanAttack))
            {
                tile.AddHighlight(tile.CanMoveTo ? "MoveTile" : "AttackTile");
            }
        }

        public void UpdateHealthOverlays(Battle battle)
        {
            Map.Tiles.ForEach(x => x.UpdateUnitLayers(battle));
        }

        public void SetFogOfWar(Battle battle)
        {
            Map.Tiles.ForEach(x => x.SetFog(battle, x.ShouldHaveFog(battle.CurrentTurn.Army)));
            foreach (var tile in Map.Tiles)
            {
                var uv = tile.Unit != null && tile.Unit.Army == battle.CurrentTurn.Army ? tile.Unit.UnitRules.Vision + tile.Rules.GetVisionBonus(tile.Unit.UnitRules.MoveType) : 0;
                var sv = tile.Structure != null && tile.Structure.Army == battle.CurrentTurn.Army && !tile.Structure.IsUnderConstruction ? tile.Structure.Rules.Vision : 0;
                if (uv == 0 && sv == 0) continue;
                RevealFogOfWar(battle, tile, uv, sv);
            }
        }

        /// <summary>
        /// Reveals the fog of war around this tile, as viewed by the provided unit.
        /// </summary>
        /// <param name="tile">The tile the unit is on</param>
        /// <param name="unit">The unit that is viewing from the tile</param>
        public void RevealFogOfWar(Battle battle, Tile tile, Unit unit)
        {
            var uv = unit.UnitRules.Vision + tile.Rules.GetVisionBonus(unit.UnitRules.MoveType);
            if (uv > 0) RevealFogOfWar(battle, tile, uv, 0);
        }


        /// <summary>
        /// Reveals the fog of war around this tile, as viewed by the provided structure.
        /// </summary>
        /// <param name="tile">The tile the structure is on</param>
        /// <param name="structure">The structure that is viewing from the tile</param>
        public void RevealFogOfWar(Battle battle, Tile tile, Structure structure)
        {
            var sv = structure.Rules.Vision;
            if (sv > 0) RevealFogOfWar(battle, tile, 0, sv);
        }

        private static void RevealFogOfWar(Battle battle, Tile tile, int unitVision, int structureVision)
        {
            var states = Search.GetAllStates(Tuple.Create(tile, true),
                                             (x, i) => TileVision(x, i, unitVision, structureVision),
                                             (a, b) => a.Item1 == b.Item1,
                                             x => 1);
            foreach (var tuple in states.Where(x => x.Item2))
            {
                tuple.Item1.SetFog(battle, false);
            }
        }

        private static IEnumerable<Tuple<Tile, bool>> TileVision(Tuple<Tile, bool> tuple, int cost, int unitVision, int structureVision)
        {
            var t = tuple.Item1;
            if (cost >= Math.Max(unitVision, structureVision)) return null;
            if (cost == 0 && unitVision > 0) return t.GetAdjacentTiles().Where(x => x != null).Select(x => Tuple.Create(x, true));
            return t.GetAdjacentTiles().Where(x => x != null).Select(x => Tuple.Create(x, !x.Rules.BlocksVision));
        }

        public void CalculateArrowOverlays(MoveSet set)
        {
            Map.Tiles.ForEach(x => x.ClearArrow());
            if (set == null) return;
            var path = set.GetMovementMoves();
            if (path.Count >= 2)
            {
                for (var i = 0; i < path.Count; i++)
                {
                    string arrow;
                    var bef = i > 0 ? path[i - 1].MoveTile.Location : null;
                    var cur = path[i].MoveTile.Location;
                    var after = i < path.Count - 1 ? path[i + 1].MoveTile.Location : null;
                    string prev = null, next = null;
                    if (bef != null) prev = bef.X != cur.X ? (bef.X < cur.X ? "W" : "E") : (bef.Y < cur.Y ? "N" : "S");
                    if (after != null) next = after.X != cur.X ? (after.X < cur.X ? "W" : "E") : (after.Y < cur.Y ? "N" : "S");
                    if (bef == null) arrow = "ArrowStart" + next;
                    else if (after == null) arrow = "ArrowEnd" + prev;
                    else arrow = "ArrowLine" + OrderDirections(prev, next);
                    path[i].MoveTile.AddArrow(arrow);
                }
            }
            var attack = set.GetAttackMove();
            if (attack != null)
            {
                attack.MoveTile.AddArrow("Crosshair");
            }
            var unload = set.GetUnloadMoves();
            foreach (var ul in unload)
            {
                var start = ul.LoaderTile.Location;
                var end = ul.MoveTile.Location;
                var dir = start.X == end.X ? (start.Y > end.Y ? "N" : "S") : (start.X > end.X ? "W" : "E");
                ul.MoveTile.AddArrow("UnloadArrowEnd" + dir);
            }
        }

        private static string OrderDirections(params string[] d1)
        {
            var result = "";
            if (d1.Contains("E")) result += "E";
            if (d1.Contains("W")) result += "W";
            if (d1.Contains("N")) result += "N";
            if (d1.Contains("S")) result += "S";
            return result;
        }

        public void SetRangeCursor(Tile center, int radius)
        {
            ClearRangeCursor();
            var list = new List<Point>();
            for (var i = 0; i < radius; i++)
            {
                list.Add(new Point(radius - i, i)); // East, moving SW
                list.Add(new Point(-i, radius - i)); // South, moving NW
                list.Add(new Point(-radius + i, -i)); // West, moving NE
                list.Add(new Point(i, -radius + i)); // North, moving SE
            }
            foreach (var point in list)
            {
                var loc = center.Location + point;
                var tile = Map.GetTile(loc);
                if (tile == null) continue;

                var sprite = "RangeCursorNE";
                if (point.X == 0) sprite = "RangeCursorN";
                else if (point.Y == 0) sprite = "RangeCursorE";

                tile.AddRangeCursor(sprite, point.X < 0, point.Y > 0);
            }
        }

        public void ClearRangeCursor()
        {
            Map.Tiles.ForEach(x => x.ClearRangeCursor());
        }

        public void AnimatePath(Unit unit, MoveSet path, Action callback = null)
        {
            if (path.Count <= 1)
            {
                if (callback != null) callback();
                return;
            }
            var animation = new Animation<double>(0, 1, (path.Count - 1) * 100, new LinearEasing(),
                                                  completedCallback: x =>
                                                                         {
                                                                             AnimationComplete(path);
                                                                             if (callback != null) callback();
                                                                         });
            if (path.Unit.Tile != null) path.Unit.Tile.HideUnit();
            _pathAnimations.Add(path, animation);
            AddAnimation(animation);
        }

        private void AnimationComplete(MoveSet path)
        {
            if (path.Unit.Tile != null) path.Unit.Tile.ShowUnit();
            _pathAnimations.Remove(path);
        }

        private void DrawAnimation(IRenderContext context)
        {
            foreach (var kv in _pathAnimations)
            {
                var path = kv.Key;
                var animation = kv.Value;

                var result = (path.Count - 1) * animation.CurrentValue;
                var tileIndex = (int) Math.Floor(result);

                var move = path[tileIndex];
                var tileControl = GetTileControl(move.MoveTile.Location);

                var topLeft = tileControl.Box.TopLeft;
                Point diff;
                if (tileIndex < path.Count - 1)
                {
                    var nextTile = path[tileIndex + 1];
                    var percentage = result - tileIndex;
                    diff = nextTile.MoveTile.Location - move.MoveTile.Location;
                    var xadd = diff.X * tileControl.Box.Width * percentage;
                    var yadd = diff.Y * tileControl.Box.Height * percentage;
                    topLeft = new Point(topLeft.X + (int) Math.Round(xadd), topLeft.Y + (int) Math.Round(yadd));
                }
                else
                {
                    var prevTile = path[tileIndex - 1];
                    diff = move.MoveTile.Location - prevTile.MoveTile.Location;
                }

                var actualFacing = diff.X != 0 ? (diff.X < 0 ? "W" : "E") : (diff.Y < 0 ? "N" : "S");
                var renderFacing = (actualFacing == "E" ? "W" : actualFacing);
                var mirror = actualFacing != renderFacing;

                SpritePool.DrawSprite(context, "UnitAnimations", path.Unit.Style + renderFacing + "_24",
                                      new Box(topLeft.X, topLeft.Y, tileControl.Box.Width, tileControl.Box.Height),
                                      new SpriteDrawingOptions { MirrorX = mirror, DockX = SpriteDrawingOptions.Dock.Center });

            }
        }

        public TileControl GetTileControl(Point p)
        {
            return Children.OfType<TileControl>().FirstOrDefault(x => x.Tile.Location.Equals(p));
        }

        public override void OnAfterRender(IRenderContext context)
        {
            base.OnAfterRender(context);
            for (var i = Children.Count - 1; i >= 0; i--)
            {
                var c = Children[i] as TileControl;
                if (c != null)
                {
                    context.Translate(c.Box.X, c.Box.Y);
                    c.RenderOverlays(context);
                    context.Translate(-c.Box.X, -c.Box.Y);
                }
            }
            DrawAnimation(context);
        }

        public override void OnMouseMove(EternityEvent e)
        {
            var tc = GetChildAt(e.X, e.Y) as TileControl;
            if (tc != null) OnTileHovered(tc.Tile, e);
        }

        public override void OnMouseDown(EternityEvent e)
        {
            var tc = GetChildAt(e.X, e.Y) as TileControl;
            if (tc != null)
            {
                e = e.Clone();
                var shift = tc.GetLocationInTree() - GetLocationInTree();
                e.Translate(shift.X, shift.Y);
                OnTileMouseDown(tc.Tile, e);
            }
        }

        public override void OnMouseUp(EternityEvent e)
        {
            var tc = GetChildAt(e.X, e.Y) as TileControl;
            if (tc != null)
            {
                e = e.Clone();
                var shift = tc.GetLocationInTree() - GetLocationInTree();
                e.Translate(shift.X, shift.Y);
                OnTileMouseUp(tc.Tile, e);
            }
        }
    }
}
