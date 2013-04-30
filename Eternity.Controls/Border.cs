using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;

namespace Eternity.Controls
{
    public abstract class Border : Insets
    {
        protected Border(int top, int right, int bottom, int left) : base(top, right, bottom, left)
        {
        }

        protected Border(int width) : base(width, width, width, width)
        {
        }

        protected Border(Insets insets) : base(insets.Top, insets.Right, insets.Bottom, insets.Left)
        {
        }

        public abstract void Draw(Control parent, IRenderContext context);
    }
}
