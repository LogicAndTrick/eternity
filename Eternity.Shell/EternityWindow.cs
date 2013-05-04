using System;
using System.Collections.Generic;
using System.Text;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Game.TurnBasedWarsGame;
using Eternity.Game.TurnBasedWarsGame.WarsGame;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Eternity.Game;
using Eternity.Graphics;
using System.IO;
using Eternity.Resources;

namespace Eternity.Shell
{
    public class EternityWindow : GameWindow
    {
        private readonly IRenderContext _renderContext;
        private readonly IInputState _inputState;
        private readonly GameState _gameState;
        private FrameInfo _currentFrame;

        public EternityWindow() : base(640, 480, new OpenTK.Graphics.GraphicsMode(), "Eternity")
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            WindowBorder = WindowBorder.Fixed;

            var folders = new[]
                              {
                                  "ResourceDefinitions",
                                  "Maps",
                                  "Rules",
                              };

            // For debug purposes I'm semi-hard-coding this until all resources are locked down and I don't have to rebuild every time.
            var dip = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Eternity.Game.TurnBasedWarsGame", "Resources"));

            if (dip.Exists)
            {
                ResourceManager.AddResourcePath(dip.FullName);
                foreach (var folder in folders)
                {
                    var di = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Eternity.Game.TurnBasedWarsGame", folder));
                    if (di.Exists) ResourceManager.LoadResources(di);
                }
            }
            else
            {
                var dir = new FileInfo(GetType().Assembly.Location).Directory;
                if (dir != null)
                {
                    ResourceManager.AddResourcePath(Path.Combine(dir.FullName, "Resources"));
                    foreach (var folder in folders)
                    {
                        var di = new DirectoryInfo(Path.Combine(dir.FullName, folder));
                        if (di.Exists) ResourceManager.LoadResources(di);
                    }
                }
            }

            _renderContext = new GLRenderContext(640, 480);
            _inputState = new OTKInputState(this);
            var map = ResourceManager.GetResourceDefinition("Map", "Bean Island");
            var battle = new Battle(map);
            //_gameState = new GameState(new TurnBasedWarsMode(battle));
            //_gameState = new GameState(new MainMenuMode());
            _gameState = new GameState(new MapEditMode(map));

            Load += (sender, e) => StartUp();
            Unload += (sender, e) => ShutDown();
            UpdateFrame += (sender, e) => Update();
            RenderFrame += (sender, e) => Render();
        }

        private void ShutDown()
        {
            _gameState.ShutDown();
        }

        private void StartUp()
        {
            _gameState.StartUp(_renderContext);
        }

        private long last = 0;
        private long lf = 0;
        private void Update()
        {
            _currentFrame = FrameInfo.Create();
            /**
            if (_currentFrame.TotalMilliseconds - last > 1000)
            {
                Console.WriteLine(_currentFrame.FrameNumber - lf);
                lf = _currentFrame.FrameNumber;
                last = _currentFrame.TotalMilliseconds;
            }
            /**/
            SpritePool.Update(_currentFrame, _inputState);
            AnimationPool.Update(_currentFrame, _inputState);
            _gameState.Update(_currentFrame, _inputState);
            _inputState.FlushAllData();
        }

        private void Render()
        {
            _gameState.Render(_renderContext);
            SwapBuffers();
        }
    }
}
