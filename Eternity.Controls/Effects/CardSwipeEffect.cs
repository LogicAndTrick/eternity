using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Controls.Easings;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;
using Eternity.Input;
using Eternity.Controls.Animations;

namespace Eternity.Controls.Effects
{
    public class CardSwipeEffect : IEffect
    {
        private List<IAnimation> _animations;
        private IEasing _easing;
        private Control _exit;
        private Control _enter;
        private long _speed;

        public CardSwipeEffect(Control exit, Control enter, long speed, IEasing easing)
        {
            _animations = new List<IAnimation>();
            _exit = exit;
            _enter = enter;
            _easing = easing;
            _speed = speed;
            if (_exit != null)
            {
                _animations.Add(new Animation<int>(_exit.Box.X, _exit.Box.X - _exit.Box.Width, speed, easing, ExitProgress, ExitComplete));
            }
        }

        private void ExitProgress(int obj)
        {
            _exit.ResizeSafe(new Box(obj, _exit.Box.Y, _exit.Box.Width, _exit.Box.Height));
        }

        private void ExitComplete(int obj)
        {
            //
        }

        public void Update(FrameInfo info, IInputState state)
        {
            _animations.ForEach(x => x.Update(info, state));
        }

        public bool IsFinished()
        {
            return _animations.All(x => x.IsFinished());
        }

        public void Start(long currentTimeInMilliseconds)
        {
            _animations.ForEach(x => x.Start(currentTimeInMilliseconds));
        }

        public void Render(IRenderContext context)
        {
            // 
        }
    }
}
