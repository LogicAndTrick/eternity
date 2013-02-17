using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public class VerticalStackLayout : ILayout
    {
        private readonly Insets _insets;
        private int _gap;

        public VerticalStackLayout(Insets insets, int gap)
        {
            _insets = insets;
            _gap = gap;
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var x = _insets.Left;
            double y = _insets.Top;
            var width = parent.Box.Width - _insets.Right - _insets.Left;
            var height = parent.Box.Height - _insets.Bottom - _insets.Top;
            var total = height - (_gap * children.Count);
            var itemHeight = total / (double) children.Count;
            foreach (var child in children)
            {
                child.ResizeSafe(new Box(x, (int) y, width, (int) itemHeight));
                y += _gap + itemHeight;
            }
        }
    }
}
