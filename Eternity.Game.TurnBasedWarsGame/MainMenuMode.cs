using System;
using Eternity.Controls.Animations;
using Eternity.Controls.Easings;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.Controls;
using Eternity.Game.TurnBasedWarsGame.Controls.MainMenu;
using Eternity.Graphics;
using Eternity.Graphics.Textures;
using Eternity.Input;
using Eternity.Controls;
using Point = Eternity.DataStructures.Primitives.Point;
using Eternity.Graphics.Sprites;

namespace Eternity.Game.TurnBasedWarsGame
{
    public class MainMenuMode : IGameMode
    {
        private LayoutControl _root;

        public void StartUp(IRenderContext context)
        {
            SpriteManager.RegisterResourceGroup(context, "MainMenu");
            TextureManager.RegisterResourceGroup(context, "MainMenu");
            InitialiseControls(context);
        }

        private void InitialiseControls(IRenderContext context)
        {
            _root = new LayoutControl(new CardLayout())
                        {
                            Box = new Box(new Point(0, 0), new Point(context.ScreenWidth, context.ScreenHeight))
                        };

            _root.Add(new ScrollingBackgroundImage(TextureManager.GetTexture("MainMenu", "Background0"), 0, 0, 0, 0));
            _root.Add(new ScrollingBackgroundImage(TextureManager.GetTexture("MainMenu", "Background1"), 5, 5, 0.5, 0.5));
            _root.Add(new ScrollingBackgroundImage(TextureManager.GetTexture("MainMenu", "Background2"), 5, 5, 0.4, 0.4));
            _root.Add(new ScrollingBackgroundImage(TextureManager.GetTexture("MainMenu", "Background3"), 5, 5, 0.3, 0.3));
            _root.Add(new ScrollingBackgroundImage(TextureManager.GetTexture("MainMenu", "BackgroundLogos"), 5, 5, -0.2, 0.2));

            var topLayer = new LayoutControl(new BorderLayout(new Insets(0, 0, 0, 0)));
            topLayer.Add(new MenuControl {Box = new Box(0, 0, 250, 378)}, Direction.Center);
            _root.Add(topLayer);
            _root.SetUp(context);
        }

        public void ShutDown()
        {
            //todo
        }

        public void Render(IRenderContext context)
        {
            context.Orthographic();
            context.Clear();
            _root.Render(context);
        }
 
        public void Update(FrameInfo info, IInputState state)
        {
            _root.Update(info, state);
        }
    }
}
