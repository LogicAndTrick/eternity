using System.Collections.Generic;
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
            throw new System.NotImplementedException();
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
