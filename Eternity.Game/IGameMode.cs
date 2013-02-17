using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Game
{
    public interface IGameMode : IRenderable, IUpdatable
    {
        void StartUp(IRenderContext context);
        void ShutDown();
    }
}
