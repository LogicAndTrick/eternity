using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public class VerticalStackLayout : ILayout
    {
        private int _gap;

        public VerticalStackLayout(int gap)
        {
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
            var insets = parent.TotalInsets;
            w += insets.TotalX;
            h += insets.TotalY + _gap * (children.Count - 1);
            return new Size(w, h);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var insets = parent.TotalInsets;
            var x = insets.Left;
            double y = insets.Top;
            var width = parent.Box.Width - insets.Right - insets.Left;
            var height = parent.Box.Height - insets.Bottom - insets.Top;
            var total = height - (_gap * (children.Count - 1));
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
