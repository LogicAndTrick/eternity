using Eternity.Game;

namespace Eternity.Controls.Animations
{
    /// <summary>
    /// A linear-time operation on a control or value.
    /// </summary>
    public interface IAnimation : IUpdatable
    {
        bool IsNew();
        bool IsRunning();
        bool IsFinished();
        bool IsInfinite();
        void Start(long currentTimeInMilliseconds);
        object GetCurrentValue();
        void Stop();
        void Reset();
    }
}