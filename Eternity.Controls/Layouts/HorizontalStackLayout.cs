using System;
using System.Collections.Generic;
using System.Linq;
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
            int w = 0, h = 0;
            foreach (var ps in children.Select(child => child.GetPreferredSize()))
            {
                w += ps.Width;
                h = Math.Max(h, ps.Height);
            }
            w += _insets.TotalX + _gap * (children.Count - 1);
            h += _insets.TotalY;
            return new Size(w, h);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            double x = _insets.Left;
            double y = _insets.Top;
            var width = parent.Box.Width - _insets.Right - _insets.Left;
            var height = parent.Box.Height - _insets.Bottom - _insets.Top;
            var total = width - (_gap * children.Count);
            var sizes = children.ToDictionary(c => c, c => c.GetPreferredSize());
            var preferred = sizes.Select(kv => kv.Value.Width).Sum();
            var ratio = Math.Min(1, total / (float) preferred);
            foreach (var child in children)
            {
                var ps = sizes[child];
                var itemWidth = ratio * ps.Width;
                child.ResizeSafe(new Box((int)x, (int)y, (int)itemWidth, height));
                x += _gap + itemWidth;
            }
        }
    }
}