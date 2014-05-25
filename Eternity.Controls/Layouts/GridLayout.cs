using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public class GridLayout : ILayout
    {
        private readonly int _width;
        private readonly int _height;

        public GridLayout(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var x = 0d;
            var y = 0d;
            foreach (var ps in children.Select(child => child.GetPreferredSize()))
            {
                x = Math.Max(x, ps.Width);
                y = Math.Max(y, ps.Height);
            }
            return new Size(x * _width, y * _height);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var insets = parent.TotalInsets;
            var left = insets.Left;
            var right = parent.Box.Width - insets.Right;
            var top = insets.Top;
            var bottom = parent.Box.Height - insets.Bottom;
            var gridWidth = (right - left) / (double) _width;
            var gridHeight = (bottom - top) / (double) _height;
            foreach (var child in children)
            {
                var position = new Point(0, 0);
                if (constraints.ContainsKey(child) && constraints[child] is Point)
                {
                    position = (Point)constraints[child];
                }
                var x = left + (int) (gridWidth * position.X);
                var y = top + (int) (gridHeight * position.Y);
                child.ResizeSafe(new Box(x, y, (int) gridWidth, (int) gridHeight));
            }
        }
    }
}
