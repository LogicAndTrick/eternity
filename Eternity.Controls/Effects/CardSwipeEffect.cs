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
        private Action _callback;

        public CardSwipeEffect(Control exit, Control enter, long speed, IEasing easing, Action callback = null)
        {
            _animations = new List<IAnimation>();
            _exit = exit;
            _enter = enter;
            _easing = easing;
            _speed = speed;
            _callback = callback;
            if (_enter == null && _exit == null) throw new Exception();

            if (_exit != null) _animations.Add(new Animation<int>(_exit.Box.X, _exit.Box.X - _exit.Box.Width, speed, easing, ExitProgress, ExitComplete));
            if (_enter != null) _animations.Add(new Animation<int>(_enter.Box.X, _enter.Box.X - _enter.Box.Width, speed, easing, EnterProgress));
        }

        private void EnterProgress(int obj)
        {
            _enter.ResizeSafe(new Box(obj, _enter.Box.Y, _enter.Box.Width, _enter.Box.Height));
        }

        private void ExitProgress(int obj)
        {
            _exit.ResizeSafe(new Box(obj, _exit.Box.Y, _exit.Box.Width, _exit.Box.Height));
        }

        private void ExitComplete(int obj)
        {
            if (_callback != null)
            {
                _callback();
            }
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
