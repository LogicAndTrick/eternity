using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Easings;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Turns;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;
using Eternity.Graphics.Textures;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class GameBoard : LayoutControl
    {
        public Battle Battle { get; private set; }
        public int GridSize { get; private set; }

        private Dictionary<MoveSet, Animation<double>> _pathAnimations; 

        public GameBoard(Battle battle) : base(new GridLayout(battle.Map.Width, battle.Map.Height, new Insets(0, 0, 0, 0)))
        {
            battle.GameBoard = this;
            Battle = battle;
            GridSize = 24;
            Box = new Box(0, 0, battle.Map.Width * GridSize, battle.Map.Height * GridSize);
            _pathAnimations = new Dictionary<MoveSet, Animation<double>>();

            var bgi = new ScrollingBackgroundImage(TextureManager.GetTexture("Overlays", "Clouds"), 5, 5, 0.03, -0.06);
            AddOverlay(bgi);
        }

        public void ShowDialog(Tile tile, params ActionDialog.ActionDialogAction[] actions)
        {
            var dialog = new ActionDialog(Box, GetTileControl(tile.Location).Box, actions);
            AddOverlay(dialog);
        }

        public void HideDialog()
        {
            Remove(x => x is ActionDialog);
        }

        public void DeselectUnit(Unit u)
        {
            foreach (var tile in Battle.Map.Tiles)
            {
                tile.OverlayGroups.RemoveLayers("Highlight");
                tile.OverlayGroups.RemoveLayers("Arrow");
                tile.OverlayGroups.RemoveLayers("UnitAnimations");
                if (u != null && tile.Unit == u)
                {
                    tile.BaseGroups.SetGroupVisibility("Unit", true);
                    tile.OverlayGroups.SetGroupVisibility("UnitHealth", true);
                    tile.OverlayGroups.SetGroupVisibility("UnitStatus", true);
                }
            }
        }

        public void SelectUnit(Unit u)
        {
            var t = u.Tile;
            t.BaseGroups.SetGroupVisibility("Unit", false);
            t.OverlayGroups.SetGroupVisibility("UnitHealth", false);
            t.OverlayGroups.SetGroupVisibility("UnitStatus", false);
            t.OverlayGroups.AddLayer("UnitAnimations", "Animation", u.Style + "W_24",
                                     new SpriteDrawingOptions { DockX = SpriteDrawingOptions.Dock.Center, MirrorX = true });
        }

        public void UpdateTileHighlights()
        {
            Battle.Map.Tiles.ForEach(x => x.OverlayGroups.RemoveLayers("Highlight"));
            foreach (var tile in Battle.Map.Tiles.Where(x => x.CanMoveTo || x.CanAttack))
            {
                tile.OverlayGroups.AddLayer("Highlight", "Highlight", tile.CanMoveTo ? "MoveTile" : "AttackTile");
            }
        }

        public void UpdateHealthOverlays()
        {
            Battle.Map.Tiles.ForEach(x => x.UpdateUnitLayers());
        }

        public void CalculateArrowOverlays(MoveSet set)
        {
            var path = set.GetMovementMoves();
            Battle.Map.Tiles.ForEach(x => x.OverlayGroups.RemoveLayers("Arrow"));
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
                    path[i].MoveTile.OverlayGroups.AddLayer("Arrow", "Arrow", arrow);
                }
            }
            var attack = set.GetAttackMove();
            if (attack != null)
            {
                attack.MoveTile.OverlayGroups.AddLayer("Arrow", "Arrow", "Crosshair",
                                                       new SpriteDrawingOptions()
                                                           {
                                                               DockX = SpriteDrawingOptions.Dock.Center,
                                                               DockY = SpriteDrawingOptions.Dock.Center
                                                           });
            }
            var unload = set.GetUnloadMoves();
            foreach (var ul in unload)
            {
                var start = ul.LoaderTile.Location;
                var end = ul.MoveTile.Location;
                var dir = start.X == end.X ? (start.Y > end.Y ? "N" : "S") : (start.X > end.X ? "W" : "E");
                ul.MoveTile.OverlayGroups.AddLayer("Arrow", "Arrow", "UnloadArrowEnd" + dir);
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
            if (path.Unit.Tile != null)
            {
                path.Unit.Tile.BaseGroups.SetGroupVisibility("Unit", false);
                path.Unit.Tile.OverlayGroups.SetGroupVisibility("UnitHealth", false);
                path.Unit.Tile.OverlayGroups.SetGroupVisibility("UnitStatus", false);
            }
            _pathAnimations.Add(path, animation);
            AddAnimation(animation);
        }

        private void AnimationComplete(MoveSet path)
        {
            if (path.Unit.Tile != null)
            {
                path.Unit.Tile.BaseGroups.SetGroupVisibility("Unit", true);
                path.Unit.Tile.OverlayGroups.SetGroupVisibility("UnitHealth", true);
                path.Unit.Tile.OverlayGroups.SetGroupVisibility("UnitStatus", true);
            }
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

        public override void OnSetUp(IRenderContext context)
        {
            base.OnSetUp(context);
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

        public override void OnUpdate(FrameInfo info, Input.IInputState state)
        {
            base.OnUpdate(info, state);

            var lit = GetLocationInTree();
            var tc = GetChildAt(state.GetMouseX() - lit.X, state.GetMouseY() - lit.Y) as TileControl;
            if (tc != null) Battle.TileHovered(tc.Tile);
        }

        protected override void OnDoLayout()
        {
            //_bgi.Box = Box;
        }
    }
}
