using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Graphics;

namespace Eternity.Controls.Controls
{
    public class Panel : Control
    {
        public override void OnRender(IRenderContext context)
        {
            //context.SetColour(this.back);
            base.OnRender(context);
        }
    }
}
