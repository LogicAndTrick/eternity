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

        public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
        {
            var ac = ActiveChild ?? children.FirstOrDefault();
            foreach (var control in children)
            {
                control.ResizeSafe(control == ac || true ? parent.Box : new Box(0, 0, 0, 0));
            }
        }
    }
}
