using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Point = Eternity.DataStructures.Primitives.Point;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Controls.Controls
{
    public class CenterPanel : LayoutControl
    {
        private class CenterLayout : ILayout
        {
            public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
            {
                var x = 0;
                var y = 0;
                foreach (var ps in children.Select(c => c.GetPreferredSize()))
                {
                    x = Math.Max(ps.Width, x);
                    y = Math.Max(ps.Height, y);
                }
                return new Size(x, y);
            }

            public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
            {
                var center = parent.InnerBox.GetCenter();
                foreach (var child in children)
                {
                    var ps = child.GetPreferredSize();
                    var half = new Point(ps.Width / 2, ps.Height / 2);
                    child.ResizeSafe(new Box(center - half, ps));
                }
            }
        }

        protected LayoutControl Child { get; private set; }

        public CenterPanel(LayoutControl child) : base(null)
        {
            Clip = true;
            SetLayout(new CenterLayout());
            Child = child;
            Add(Child);
        }

        public override Size GetPreferredSize()
        {
            var ti = TotalInsets;
            return Child.GetPreferredSize() + new Size(ti.TotalX, ti.TotalY);
        }
    }
}
