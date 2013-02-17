using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eternity.Controls.Layouts
{
    public interface ILayout
    {
        void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints);
    }
}
