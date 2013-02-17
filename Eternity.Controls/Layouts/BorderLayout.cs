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
        private readonly Insets _insets;

        public BorderLayout(Insets insets)
        {
            _insets = insets;
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var left = _insets.Left;
            var right = parent.Box.Right.Start.X - _insets.Right;
            var top = _insets.Top;
            var bottom = parent.Box.Bottom.Start.Y - _insets.Bottom;
            var center = new Point(left + (right - left) / 2, top + (bottom - top) / 2);
            foreach (var child in children)
            {
                var dir = Direction.Top | Direction.Left;
                if (constraints.ContainsKey(child) && constraints[child] is Direction)
                {
                    dir = (Direction) constraints[child];
                }
                var x = center.X - child.Box.Width / 2;
                var y = center.Y - child.Box.Height / 2;
                if ((dir & Direction.Fill) == Direction.Fill)
                {
                    child.ResizeSafe(new Box(0, 0, parent.Box.Width, parent.Box.Height));
                }
                else
                {
                    if ((dir & Direction.Left) == Direction.Left)
                    {
                        x = left;
                        left += child.Box.Width;
                    }
                    else if ((dir & Direction.Right) == Direction.Right)
                    {
                        x = right - child.Box.Width;
                        right -= child.Box.Width;
                    }
                    if ((dir & Direction.Top) == Direction.Top)
                    {
                        y = top;
                        top += child.Box.Height;
                    }
                    else if ((dir & Direction.Bottom) == Direction.Bottom)
                    {
                        y = bottom - child.Box.Height;
                        bottom -= child.Box.Height;
                    }
                    child.ResizeSafe(new Box(x, y, child.Box.Width, child.Box.Height));
                }
            }
        }
    }
}
