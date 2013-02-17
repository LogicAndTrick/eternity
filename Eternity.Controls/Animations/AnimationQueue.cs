using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Controls.Animations
{
    public class AnimationQueue : IUpdatable
    {
        private List<IAnimation> _new; 
        private List<IAnimation> _animations;
        private List<IAnimation> _sequential; 

        public AnimationQueue()
        {
            _new = new List<IAnimation>();
            _animations = new List<IAnimation>();
            _sequential = new List<IAnimation>();
        }

        public bool IsEmpty()
        {
            return !_animations.Any(x => !x.IsFinished());
        }

        public bool IsSequentialEmpty()
        {
            return !_sequential.Any(x => !x.IsFinished());
        }

        public void AddAnimation(IAnimation anim)
        {
            _new.Add(anim);
        }

        public void AddSequential(IAnimation anim)
        {
            if (anim.IsInfinite()) throw new Exception("Cannot add an infinte animation into a sequential queue!");
            _sequential.Add(anim);
        }

        public void Update(FrameInfo info, IInputState state)
        {
            // Start new animations
            foreach (var animation in _new)
            {
                animation.Start(info.TotalMilliseconds);
                _animations.Add(animation);
            }
            _new.Clear();

            // Update running animations
            foreach (var animation in _animations)
            {
                animation.Update(info, state);
            }

            // Update the currently running sequential animation
            var ani = _sequential.FirstOrDefault(x => !x.IsFinished());
            if (ani == null) return;
            if (ani.IsNew()) ani.Start(info.TotalMilliseconds);
            ani.Update(info, state);
        }

        public void RemoveCompletedAnimations()
        {
            _animations.RemoveAll(x => x.IsFinished());
            _sequential.RemoveAll(x => x.IsFinished());
        }

        public void StopSequential()
        {
            _sequential.ForEach(x => x.Stop());
            _sequential.Clear();
        }
    }
}
