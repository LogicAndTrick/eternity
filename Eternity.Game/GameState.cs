using System;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Game
{
    public class GameState : IRenderable, IUpdatable
    {
        private IGameMode _currentMode;

        public GameState(IGameMode mode)
        {
            _currentMode = mode;
        }

        public void Update(FrameInfo frame, IInputState state)
        {
            _currentMode.Update(frame, state);
        }
        
        public void Render(IRenderContext context)
        {
            _currentMode.Render(context);
        }

        public void ChangeMode(IGameMode mode)
        {
            if (mode == null) throw new Exception("Cannot assign a null game state.");
            _currentMode = mode;
        }

        public void StartUp(IRenderContext context)
        {
            if (_currentMode == null) throw new Exception("Cannot startup a null game.");
            _currentMode.StartUp(context);
        }

        public void ShutDown()
        {
            if (_currentMode == null) throw new Exception("Cannot shut down a null game.");
            _currentMode.ShutDown();
        }

        /*
        [Subscribe(Messages.ChangeGameState)]
        public void ChangeMode(params object[] parameters)
        {
            if (parameters.Length == 1 && parameters[0] != null && parameters[0].GetType() == typeof(GameMode))
            {
                ChangeMode((GameMode)parameters[0]);
                return;
            }
            if (parameters.Length == 0 || parameters[0] == null || parameters[0].GetType() != typeof(string))
            {
                throw new ArgumentException("The first parameter passed to GameState.ChangeState must be a string.");
            }
            var constructParams = parameters.Skip(1).ToArray();
            var constructTypes = constructParams.Select(x => x.GetType()).ToArray();
            var str = (string)parameters[0];
            var resolvedType = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Eternity"))
                .SelectMany(a => a.GetTypes().Where(t => t.Name == str))
                .Where(t => t != null && t.IsSubclassOf(typeof(GameMode)))
                .FirstOrDefault();
            if (resolvedType == null)
            {
                throw new ArgumentException("Could not resolve GameMode type: " + str);
            }
            var constructor = resolvedType.GetConstructor(constructTypes);
            if (constructor == null)
            {
                throw new ArgumentException("Could not find a suitable constructor for GameMode type: " + str + " with parameters: "
                    + constructTypes.Select(x => x.Name).Aggregate((x, y) => x + ", " + y));
            }
            var instance = constructor.Invoke(constructParams) as GameMode;
            if (instance == null)
            {
                throw new ArgumentException("Unable to construct an instance of GameMode type: " + str);
            }
            ChangeMode(instance);
        }
        */
    }
}
