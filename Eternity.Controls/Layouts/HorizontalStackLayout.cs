using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public class HorizontalStackLayout : ILayout
    {
        private int _gap;

        public HorizontalStackLayout(int gap)
        {
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
            var insets = parent.TotalInsets;
            w += insets.TotalX + _gap * (children.Count - 1);
            h += insets.TotalY;
            return new Size(w, h);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var insets = parent.TotalInsets;
            double x = insets.Left;
            double y = insets.Top;
            var width = parent.Box.Width - insets.Right - insets.Left;
            var height = parent.Box.Height - insets.Bottom - insets.Top;
            var total = width - (_gap * (children.Count - 1));
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