using System;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Controls;
using Eternity.Controls.Easings;
using Eternity.DataStructures.Primitives;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapEdit
{
    public class CircleMenuCursor : Control, IOverlayControl
    {
        private class MenuCursorCorner : SpriteControl
        {
            public Point Point { get; set; }
            public MenuCursorCorner(string group, string name, Point point) : base(group, name)
            {
                Point = point;
            }

            public override void OnRender(Graphics.IRenderContext context)
            {
                base.OnRender(context);
            }
        }

        private MenuCursorCorner[] _corners;

        public CircleMenuCursor()
        {
            Reset();
        }

        public void Reset()
        {
            StopAnimations();
            Remove(x => true);
            if (Parent == null) return;

            var pos = new[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1) };
            _corners = new MenuCursorCorner[4];
            for (var i = 0; i < _corners.Length; i++)
            {
                _corners[i] = new MenuCursorCorner("MainMenu", "MenuCursorCircle", pos[i]);
                var ps = _corners[i].GetPreferredSize();
                var hps = ps / 2;
                var p = new Point(pos[i].X * Parent.Box.Width, pos[i].Y * Parent.Box.Height);
                _corners[i].ResizeSafe(new Box(p.X - hps.Width, p.Y - hps.Height, ps.Width, ps.Height));
                Add(_corners[i]);
            }

            StartAnimation();
        }

        private void StartAnimation()
        {
            for (var i = 0; i < _corners.Length; i++)
            {
                var c = _corners[i];
                var left = c.Point.X == 0;
                var top = c.Point.Y == 0;

                int start = c.Box.X, end = start + Parent.Box.Width;
                var x = true;
                Point newPoint;
                if (top && left)
                {
                    // Top left: animate x positive
                    newPoint = new Point(1, 0);
                }
                else if (top)
                {
                    // Top right: animate y positive
                    start = c.Box.Y;
                    end = start + Parent.Box.Height;
                    x = false;
                    newPoint = new Point(1, 1);
                }
                else if (!left)
                {
                    // Bottom right: animate x negative
                    end = start - Parent.Box.Width;
                    newPoint = new Point(0, 1);
                }
                else
                {
                    // Bottom left: animate y negative
                    start = c.Box.Y;
                    end = start - Parent.Box.Height;
                    x = false;
                    newPoint = new Point(0, 0);
                }
                AddAnimation(new Animation<int>(() => start, () => end, 300, new QuadEasing(),
                    v => AnimationCallback(c, x, v), _ => c.Point = newPoint));
            }
            AddAnimation(Animation<int>.Delay(800, _ => StartAnimation()));
        }

        private void AnimationCallback(Control c, bool x, int val)
        {
            var p = new Point(x ? val : c.Box.X, x ? c.Box.Y : val);
            c.ResizeSafe(new Box(p, c.Box.Size));
        }

        public bool InterceptEvent(EternityEvent e)
        {
            return false;
        }

        public bool ListenEvent(EternityEvent e)
        {
            return false;
        }
    }
}
