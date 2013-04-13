using System.Collections.Generic;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public class GridLayout : ILayout
    {
        private readonly Insets _insets;
        private readonly int _width;
        private readonly int _height;

        public GridLayout(int width, int height, Insets insets)
        {
            _width = width;
            _height = height;
            _insets = insets;
        }

        public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            throw new System.NotImplementedException();
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var left = _insets.Left;
            var right = parent.Box.Width - _insets.Right;
            var top = _insets.Top;
            var bottom = parent.Box.Height - _insets.Bottom;
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
