using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Game
{
    public interface IUpdatable
    {
        void Update(FrameInfo info, IInputState state);
    }
}
