using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eternity.Graphics
{
    public interface IRenderable
    {
        void Render(IRenderContext context);
    }
}
