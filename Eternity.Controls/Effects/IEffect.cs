using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game;
using Eternity.Graphics;

namespace Eternity.Controls.Effects
{
    /// <summary>
    /// An arbitrary time-based effect with no specific duration.
    /// Useful for non-linear tasks or operations that need to co-ordinate complex changes.
    /// </summary>
    public interface IEffect : IUpdatable, IRenderable
    {
        bool IsFinished();
        void Start(long currentTimeInMilliseconds);
    }
}
