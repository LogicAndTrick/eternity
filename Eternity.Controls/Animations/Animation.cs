using System;
using Eternity.Controls.Easings;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Controls.Animations
{
    public class Animation<T> : IAnimation
    {
        private long _animationStartTime;
        private readonly long _animationFrameTime;
        private readonly long _animationDuration;
        private long _animationEndTime;
        private readonly Func<T> _startValueFunc;
        private T _startValue;
        private readonly Func<T> _endValueFunc;
        private T _endValue;
        private readonly IEasing _easing;
        private readonly bool _loopingMode;
        private readonly Func<T, T> _animationLambda;
        private long _nextStep;
        private readonly Action<T> _progressCallback;
        private Action<T> _completedCallback;

        public bool HasStarted { get; private set; }
        public bool Running { get; private set; }
        public T CurrentValue { get; private set; }
        public double CurrentProgress { get; private set; }

        public Animation(T val, long frameTimeInMilliseconds, Func<T, T> lambda, Action<T> progressCallback = null)
        {
            if (val == null) throw new Exception("The value cannot be null.");

            _animationFrameTime = frameTimeInMilliseconds;
            Running = false;
            CurrentValue = val;
            _loopingMode = true;
            _animationLambda = lambda;
            _progressCallback = progressCallback;
            _nextStep = 0;
            HasStarted = false;
        }

        public Animation(T val, T targetVal, long animationTimeInMilliseconds, IEasing easing, Action<T> progressCallback = null, Action<T> completedCallback = null)
            : this(() => val, () => targetVal, animationTimeInMilliseconds, easing, progressCallback, completedCallback)
        {
        }

        public Animation(Func<T> val, Func<T> targetVal, long animationTimeInMilliseconds, IEasing easing, Action<T> progressCallback = null, Action<T> completedCallback = null)
        {
            _animationDuration = Math.Abs(animationTimeInMilliseconds);
            _startValueFunc = val;
            _endValueFunc = targetVal;
            _easing = easing ?? new LinearEasing();
            Running = false;
            CurrentValue = val();
            _loopingMode = false;
            _progressCallback = progressCallback;
            _completedCallback = completedCallback;

            CurrentProgress = 0;
            _nextStep = 0;
            HasStarted = false;
        }

        public Animation<T> Queue(Action<IAnimation> add, Func<T> val, Func<T> targetVal, long animationTimeInMilliseconds, IEasing easing, Action<T> progressCallback = null, Action<T> completedCallback = null)
        {
            var cc = _completedCallback;
            var anim = new Animation<T>(val, targetVal, animationTimeInMilliseconds, easing, progressCallback, completedCallback);
            /*_completedCallback = x =>
                                     {
                                         add(anim);
                                         if (cc != null) cc(x);
                                     };*/
            _completedCallback = x => add(anim);
            return anim;
        }

        public static Animation<T> Delay(long milliseconds, Action<T> callback = null)
        {
            if (callback == null) callback = _ => { };
            return new Animation<T>(default(T), default(T), milliseconds, new LinearEasing(), null, callback);
        }

        public void Update(FrameInfo info, IInputState state)
        {
            Update(info.TotalMilliseconds);
        }

        public bool IsNew()
        {
            return !HasStarted;
        }

        public bool IsRunning()
        {
            return Running;
        }

        public bool IsFinished()
        {
            return HasStarted && !Running;
        }

        public bool IsInfinite()
        {
            return _loopingMode;
        }

        public void Start(long currentTimeInMilliseconds)
        {
            if (Running) return;
            HasStarted = true;
            Running = true;

            if (_loopingMode)
            {
                _nextStep = currentTimeInMilliseconds + _animationFrameTime;
            }
            else
            {
                _startValue = _startValueFunc();
                _endValue = _endValueFunc();
                _animationStartTime = currentTimeInMilliseconds;
                _animationEndTime = _animationStartTime + _animationDuration;
            }
        }

        public object GetCurrentValue()
        {
            return CurrentValue;
        }

        public void SetCurrentValue(T val)
        {
            if (_loopingMode) CurrentValue = val;
        }

        public void Update(long currentTimeInMilliseconds)
        {
            if (!Running) return;
            if (_loopingMode)
            {
                if (currentTimeInMilliseconds > _nextStep)
                {
                    var steps = (int) Math.Ceiling((currentTimeInMilliseconds - _nextStep) / (double) _animationFrameTime);
                    for (var i = 0; i < steps; i++)
                    {
                        CurrentValue = _animationLambda(CurrentValue);
                        _nextStep += _animationFrameTime;
                    }
                    if (_progressCallback != null)
                    {
                        _progressCallback(CurrentValue);
                    }
                }
            }
            else
            {
                CurrentProgress = GetProgress(currentTimeInMilliseconds);
                CurrentValue = GetValueAtProgress(CurrentProgress);
                if (_progressCallback != null)
                {
                    _progressCallback(CurrentValue);
                }
            }
            if (!_loopingMode && currentTimeInMilliseconds > _animationEndTime) Stop();
        }

        public double GetProgress(long currentTimeInMilliseconds)
        {
            double ms = currentTimeInMilliseconds - _animationStartTime;
            if (ms <= 0) return 0;
            if (ms >= _animationDuration) return 1;
            return _easing.CalculateEasing(ms / _animationDuration);
        }

        public T GetValueAtProgress(double progress)
        {
            var tt = typeof (T);
            if (tt == typeof(int) || tt == typeof(double) || tt == typeof(float) || tt == typeof(long) || tt == typeof(decimal))
            {
                var sv = (double) Convert.ChangeType(_startValue, typeof (double));
                var ev = (double) Convert.ChangeType(_endValue, typeof (double));
                var pgs = sv + (ev - sv) * progress;
                return (T) Convert.ChangeType(pgs, typeof (T));
            }
            throw new Exception("Animation of this type is not supported!");
        }

        public void Stop()
        {
            if (!Running) return;
            Running = false;
            if (_loopingMode)
            {

            }
            else
            {
                CurrentValue = _endValue;
                CurrentProgress = 1;
            }
            if (_progressCallback != null)
            {
                _progressCallback(CurrentValue);
            }
            if (_completedCallback != null)
            {
                _completedCallback(CurrentValue);
            }
        }

        public void Reset()
        {
            if (!Running) return;
            Running = false;
            HasStarted = false;
            if (_loopingMode)
            {

            }
            else
            {
                CurrentValue = _startValue;
                CurrentProgress = 0;
                _animationEndTime = _animationStartTime = 0;
            }
            if (_progressCallback != null)
            {
                _progressCallback(CurrentValue);
            }
        }
    }
}
