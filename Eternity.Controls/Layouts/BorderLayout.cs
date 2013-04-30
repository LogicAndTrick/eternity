using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    [Flags]
    public enum Direction
    {
        Top = 0x01,
        Left = 0x02,
        Right = 0x04,
        Bottom = 0x08,
        Center = 0x10,
        Fill = 0x20
    }

    public class BorderLayout : ILayout
    {
        protected virtual Direction ExtractDirection(object constraints)
        {
            if (constraints is Direction) return (Direction)constraints;
            return Direction.Top | Direction.Left;
        }

        public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var insets = parent.TotalInsets;
            int t = insets.Top, b = insets.Bottom, l = insets.Left, r = insets.Right;
            int cw = 0, ch = 0;
            foreach (var child in children)
            {
                var ps = child.GetPreferredSize();
                var dir = ExtractDirection(constraints.Where(kv => kv.Key == child).Select(kv => kv.Value).FirstOrDefault());

                if (dir.HasFlag(Direction.Top)) t += ps.Height;
                else if (dir.HasFlag(Direction.Bottom)) b += ps.Height;
                else ch = Math.Max(ch, ps.Height);

                if (dir.HasFlag(Direction.Left)) l += ps.Width;
                else if (dir.HasFlag(Direction.Right)) r += ps.Width;
                else cw = Math.Max(cw, ps.Width);
            }
            return new Size(l + r + cw, t + b + ch);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var insets = parent.TotalInsets;
            var left = insets.Left;
            var right = parent.Box.Right.Start.X - insets.Right;
            var top = insets.Top;
            var bottom = parent.Box.Bottom.Start.Y - insets.Bottom;
            var center = new Point(left + (right - left) / 2, top + (bottom - top) / 2);
            var preferred = children.ToDictionary(x => x, x => x.GetPreferredSize());
            // todo when the size isn't the preferred one
            foreach (var child in children)
            {
                var ps = child.GetPreferredSize();
                var dir = ExtractDirection(constraints.Where(kv => kv.Key == child).Select(kv => kv.Value).FirstOrDefault());

                var x = center.X - ps.Width / 2;
                var y = center.Y - ps.Height / 2;
                if (dir.HasFlag(Direction.Fill))
                {
                    child.ResizeSafe(new Box(0, 0, parent.Box.Width, parent.Box.Height));
                }
                else
                {
                    if (dir.HasFlag(Direction.Left))
                    {
                        x = left;
                        left += ps.Width;
                    }
                    else if (dir.HasFlag(Direction.Right))
                    {
                        x = right - ps.Width;
                        right -= ps.Width;
                    }
                    if (dir.HasFlag(Direction.Top))
                    {
                        y = top;
                        top += ps.Height;
                    }
                    else if (dir.HasFlag(Direction.Bottom))
                    {
                        y = bottom - ps.Height;
                        bottom -= ps.Height;
                    }
                    child.ResizeSafe(new Box(x, y, ps.Width, ps.Height));
                }
            }
        }
    }
}
