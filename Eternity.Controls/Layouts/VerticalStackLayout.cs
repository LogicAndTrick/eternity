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

        public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            int w = 0, h = 0;
            foreach (var ps in children.Select(child => child.GetPreferredSize()))
            {
                w = Math.Max(w, ps.Width);
                h += ps.Height;
            }
            w += _insets.TotalX;
            h += _insets.TotalY + _gap * (children.Count - 1);
            return new Size(w, h);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var x = _insets.Left;
            double y = _insets.Top;
            var width = parent.Box.Width - _insets.Right - _insets.Left;
            var height = parent.Box.Height - _insets.Bottom - _insets.Top;
            var total = height - (_gap * children.Count);
            var sizes = children.ToDictionary(c => c, c => c.GetPreferredSize());
            var preferred = sizes.Select(kv => kv.Value.Height).Sum();
            var ratio = Math.Min(1, total / (float) preferred);
            foreach (var child in children)
            {
                var ps = sizes[child];
                var itemHeight = ratio * ps.Height;
                child.ResizeSafe(new Box(x, (int)y, width, (int)itemHeight));
                y += _gap + itemHeight;
            }
        }
    }
}
