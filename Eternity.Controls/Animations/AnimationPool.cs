using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Controls.Animations
{
    public static class AnimationPool
    {
        private static readonly AnimationQueue Queue;

        static AnimationPool()
        {
            Queue = new AnimationQueue();
        }

        public static void AddAnimation(IAnimation anim)
        {
            Queue.AddAnimation(anim);
        }

        public static void Update(FrameInfo info, IInputState state)
        {
            Queue.Update(info, state);
            Queue.RemoveCompletedAnimations();
        }
    }
}
