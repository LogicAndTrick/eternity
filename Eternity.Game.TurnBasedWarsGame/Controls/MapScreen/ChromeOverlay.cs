using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Controls;
using Eternity.Controls.Easings;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Input;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class ChromeOverlay : LayoutControl, IOverlayControl
    {
        public ChromeOverlay() : base(new ChromeBorderLayout())
        {
        }

        public override void OnMouseMove(Input.EternityEvent e)
        {
            var point = new Point(e.X, e.Y);
            var chasers = Children
                .Select(x => new {Control = x as TileInfoChromeControl, Constraints = GetConstraints(x) as ChromeConstraints})
                .Where(x => x.Constraints != null && x.Control != null && x.Constraints.AvoidCursor && !x.Constraints.Animating)
                .Where(x => x.Control.Box.X - x.Constraints.BufferSize < e.X
                            && x.Control.Box.X + x.Control.Box.Width + x.Constraints.BufferSize > e.X
                            && x.Control.Box.Y - x.Constraints.BufferSize < e.Y
                            && x.Control.Box.Y + x.Control.Box.Height + x.Constraints.BufferSize > e.Y);

            var parentWidth = Box.Width;
            var parentHeight = Box.Height;

            foreach (var chaser in chasers)
            {
                var constraints = chaser.Constraints;
                var control = chaser.Control;
                constraints.Animating = true;

                var width = control.Box.Width;
                var height = control.Box.Height;
                
                var isVertical = constraints.StickyDirection == Direction.Left || constraints.StickyDirection == Direction.Right;
                var isPositive = (constraints.CurrentDirection.HasFlag(Direction.Left) && !isVertical) || (constraints.CurrentDirection.HasFlag(Direction.Top) && isVertical);

                var addX = isVertical ? 0 : width;
                var addY = isVertical ? height : 0;

                // The first animation start and end values
                var startX = control.Box.X;
                var startY = control.Box.Y;

                var midX = isPositive ? startX - addX : startX + addX;
                var midY = isPositive ? startY - addY : startY + addY;

                // The second animation start and end values
                var finalX = isVertical ? startX : parentWidth - width - startX;
                var finalY = isVertical ? parentHeight - height - startY : startY;

                var newStartX = isPositive ? finalX + addX : finalX - addX;
                var newStartY = isPositive ? finalY + addY : finalY - addY;

                // The docking value reassignments
                var currentVert = constraints.CurrentDirection.HasFlag(Direction.Top) ? Direction.Top : Direction.Bottom;
                var currentHor = constraints.CurrentDirection.HasFlag(Direction.Left) ? Direction.Left : Direction.Right;

                var newVert = isVertical ? (currentVert == Direction.Top ? Direction.Bottom : Direction.Top) : currentVert;
                var newHor = isVertical ? currentHor : (currentHor == Direction.Left ? Direction.Right : Direction.Left);

                AddAnimationSequential(
                    new Animation<int>(
                        isVertical ? startY : startX,
                        isVertical ? midY : midX,
                        100,
                        new QuadEasing(),
                        x => control.ResizeSafe(new Box(isVertical ? midX : x, isVertical ? x : midY, width, height)),
                        x =>
                            {
                                constraints.CurrentDirection = newVert | newHor;
                                control.ResizeSafe(new Box(newStartX, newStartY, width, height));
                            }),
                    new Animation<int>(
                        isVertical ? newStartY : newStartX,
                        isVertical ? finalY : finalX,
                        100,
                        new EasingOut(new QuadEasing()),
                        x => control.ResizeSafe(new Box(isVertical ? finalX : x, isVertical ? x : finalY, width, height)),
                        x => constraints.Animating = false)
                    );
            }
        }

        public bool InterceptEvent(EternityEvent e)
        {
            return false;
        }

        public bool ListenEvent(EternityEvent e)
        {
            return true;
        }
    }

    public class ChromeBorderLayout : BorderLayout
    {
        public ChromeBorderLayout() : base()
        {
        }

        protected override Direction ExtractDirection(object constraints)
        {
            if (constraints is ChromeConstraints) return ((ChromeConstraints) constraints).CurrentDirection;
            return base.ExtractDirection(constraints);
        }
    }

    public class TileInfoChromeControl : LayoutControl
    {
        public TileInfoChromeControl() : base(new HorizontalStackLayout(0))
        {
        }

        public void SetTile(Tile tile)
        {
            Children.ForEach(x => x.Dispose());
            Children.Clear();
            var ctrl = new LayoutControl(new VerticalStackLayout(1));// { Box = new Box(0, 0, 50, 50) };
            ctrl.Add(new Label(tile.Type.ToString(), Color.White));
            var rules = RuleSet.GetTerrainRules(tile.Type);
            var str = tile.Structure;
            if (str != null && !tile.Structure.IsUnderConstruction)
            {
                var army = "Neutral";
                if (str.Army != null) army = str.Army.ArmyRules.Name;
                ctrl.Add(new Label("Army: " + army, Color.White));
                ctrl.Add(new Label("CP: " + str.CapturePoints, Color.White));
                ctrl.Add(new Label("HP: " + str.Health, Color.White));
            }
            ctrl.Add(new Label("Capt: " + (rules.Capturable ? "Yes" : "No"), Color.White));
            ctrl.Add(new Label("Dest: " + (rules.Destroyable ? "Yes" : "No"), Color.White));
            ctrl.Add(new Label("Def: " + rules.Defense, Color.White));
            ctrl.Add(new Label("Vis: " + rules.Vision, Color.White));
            Children.Add(ctrl);

            Box = new Box(Box.X, Box.Y, Box.Width, ctrl.NumChildren * 15);

            DoLayout();
        }

        public override void OnRender(Graphics.IRenderContext context)
        {
            context.ClearTexture();
            context.SetColour(Color.FromArgb(128, Color.Gray));
            context.StartQuads();
            context.Point2(0, 0);
            context.Point2(0, Box.Height);
            context.Point2(Box.Width, Box.Height);
            context.Point2(Box.Width, 0);
            context.End();
        }
    }

    public class ChromeConstraints
    {
        public bool AvoidCursor { get; set; }
        public Direction StickyDirection { get; set; }
        public Direction CurrentDirection { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int BufferSize { get; set; }
        public bool Animating { get; set; }

        public ChromeConstraints()
        {
            BufferSize = 10;
        }
    }
}
