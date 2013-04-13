using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Controls.Layouts
{
    public interface ILayout
    {
        Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints);
        void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints);
    }
}
