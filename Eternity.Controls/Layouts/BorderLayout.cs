using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public enum Direction
    {
        Top,
        Left,
        Right,
        Bottom,
        Center
    }

    public class BorderLayout : ILayout
    {
        protected virtual Direction ExtractDirection(object constraints)
        {
            if (constraints is Direction) return (Direction)constraints;
            return Direction.Center;
        }

        private Size CalculatePreferredSize(Control parent, IEnumerable<Control> children, Dictionary<Control, object> constraints, IDictionary<Control, Size> preferred)
        {
            var insets = parent.TotalInsets;
            int t = insets.Top, b = insets.Bottom, l = insets.Left, r = insets.Right;
            int cw = 0, ch = 0;
            foreach (var child in children)
            {
                var ps = preferred[child];
                var dir = ExtractDirection(constraints.Where(kv => kv.Key == child).Select(kv => kv.Value).FirstOrDefault());

                switch (dir)
                {
                    case Direction.Top:
                        t += ps.Height;
                        break;
                    case Direction.Bottom:
                        b += ps.Height;
                        break;
                    case Direction.Left:
                        l += ps.Width;
                        break;
                    case Direction.Right:
                        r += ps.Width;
                        break;
                    case Direction.Center:
                        cw = Math.Max(cw, ps.Width);
                        ch = Math.Max(ch, ps.Height);
                        break;
                }
            }
            return new Size(l + r + cw, t + b + ch);
        }

        public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var preferred = children.ToDictionary(x => x, x => x.GetPreferredSize());
            return CalculatePreferredSize(parent, children, constraints, preferred);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var ib = parent.InnerBox;
            var left = ib.X;
            var right = left + ib.Width;
            var top = ib.Y;
            var bottom = top + ib.Height;

            var preferred = children.ToDictionary(x => x, x => x.GetPreferredSize());
            var center = new List<Control>();

            var ideal = CalculatePreferredSize(parent, children, constraints, preferred);
            var actual = ib.Size;

            var xratio = actual.Width >= ideal.Width ? 1f : actual.Width / (float)ideal.Width;
            var yratio = actual.Height >= ideal.Height ? 1f : actual.Height / (float)ideal.Height;

            foreach (var child in children)
            {
                var ps = preferred[child];
                var act = new Size((int) (ps.Width * xratio), (int) (ps.Height * yratio));
                var dir = ExtractDirection(constraints.Where(kv => kv.Key == child).Select(kv => kv.Value).FirstOrDefault());

                if (dir == Direction.Center)
                {
                    center.Add(child);
                    continue;
                }

                var x = left;
                var y = top;
                var wid = act.Width;
                var hei = act.Height;
                switch (dir)
                {
                    case Direction.Left:
                        x = left;
                        y = top;
                        wid = act.Width;
                        hei = bottom - top;
                        left += act.Width;
                        break;
                    case Direction.Right:
                        x = right - act.Width;
                        y = top;
                        wid = act.Width;
                        hei = bottom - top;
                        right -= act.Width;
                        break;
                    case Direction.Top:
                        x = left;
                        y = top;
                        wid = right - left;
                        hei = act.Height;
                        top += act.Height;
                        break;
                    case Direction.Bottom:
                        x = left;
                        y = bottom - act.Height;
                        wid = right - left;
                        hei = act.Height;
                        bottom -= act.Height;
                        break;
                }

                child.ResizeSafe(new Box(x, y, wid, hei));
            }

            foreach (var child in center)
            {
                child.ResizeSafe(new Box(left, top, right - left, bottom - top));
            }
        }
    }
}
