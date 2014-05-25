using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public class CardLayout : ILayout
    {
        public Control ActiveChild { get; set; }

        public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            double w = 0, h = 0;
            foreach (var ps in children.Select(child => child.GetPreferredSize()))
            {
                w = Math.Max(w, ps.Width);
                h = Math.Max(h, ps.Height);
            }
            var insets = parent.TotalInsets;
            w += insets.TotalX;
            h += insets.TotalY;
            return new Size(w, h);
        }

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            foreach (var control in children)
            {
                control.ResizeSafe(parent.InnerBox);
            }
        }
    }
}
