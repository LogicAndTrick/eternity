using System.Collections.Generic;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public class HorizontalStackLayout : ILayout
    {
        private readonly Insets _insets;
        private int _gap;

        public HorizontalStackLayout(Insets insets, int gap)
        {
            _insets = insets;
            _gap = gap;
        }

        public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            throw new System.NotImplementedException();
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            double x = _insets.Left;
            double y = _insets.Top;
            var width = parent.Box.Width - _insets.Right - _insets.Left;
            var height = parent.Box.Height - _insets.Bottom - _insets.Top;
            var total = width - (_gap * children.Count);
            var itemWidth = total / (double)children.Count;
            foreach (var child in children)
            {
                child.ResizeSafe(new Box((int)x, (int)y, (int)itemWidth, height));
                x += _gap + itemWidth;
            }
        }
    }
}