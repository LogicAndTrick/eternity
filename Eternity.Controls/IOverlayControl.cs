using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Input;

namespace Eternity.Controls
{
    public interface IOverlayControl
    {
        bool InterceptEvent(EternityEvent e);
        bool ListenEvent(EternityEvent e);
    }
}
