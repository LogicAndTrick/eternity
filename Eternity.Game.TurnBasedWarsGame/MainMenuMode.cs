using System.Collections.Generic;
using System.Linq;
using Eternity.Controls.Animations;
using Eternity.Controls.Easings;
using Eternity.Controls.Effects;
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
        private Control _cursor;
        private RepeatingSpriteControl _heartbeat;
        private Stack<MenuScreen> _screens;

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
            _root.Add(_heartbeat = new RepeatingSpriteControl("MainMenu", "Heartbeat") { RepeatX = true });
            
            _screens = new Stack<MenuScreen>();
            PushScreen(new MenuControl());

            _root.AddOverlay(_cursor = new MenuCursor());

            _root.SetUp(context);
        }

        private void PushScreen(MenuScreen screen)
        {
            var container = _root.GetChildren().FirstOrDefault(x => x.NumChildren == 1 && x.GetChildren().All(y => y is MenuScreen));

            _screens.Push(screen);
            var newContainer = new LayoutControl(new BorderLayout());
            newContainer.Add(screen, Direction.Center);
            _root.Add(newContainer);

            if (container != null)
            {
                var currentScreen = container.GetChildren().OfType<MenuScreen>().First();
                currentScreen.FocusControl -= FocusControl;
                currentScreen.ChangeScreen -= ChangeScreen;
                newContainer.ResizeSafe(new Box(_root.Box.Width, 0, _root.Box.Width, _root.Box.Height));
                _root.AddEffect(new CardSwipeEffect(container,
                                                    newContainer, 600,
                                                    new EasingInOut(new BackEasing()),
                                                    () =>
                                                        {
                                                            _root.Remove(container);
                                                            screen.ChangeScreen += ChangeScreen;
                                                            screen.FocusControl += FocusControl;
                                                        }));
            }
            else
            {
                screen.ChangeScreen += ChangeScreen;
                screen.FocusControl += FocusControl;
            }
        }


        private void ChangeScreen(object sender, ChangeScreenEventArgs e)
        {
            PushScreen(e.Screen);
        }

        private void FocusControl(object sender, FocusControlEventArgs e)
        {
            var pos = e.Control.GetLocationInTree() - _root.GetLocationInTree();

            var st = _cursor.Margin.Top;
            var sl = _cursor.Margin.Left;
            var ft = pos.Y - 4;
            var fl = pos.X - 4;
            var ss = _cursor.Box.Size;
            var fs = e.Control.Box.Size + new Size(pos.X + 4, pos.Y + 4);
            //_root.StopAnimations();
            _root.AddAnimation(
                new Animation<int>(0, 100, 200, new EasingOut(new QuintEasing()),
                                   x =>
                                       {
                                           _cursor.Margin = new Insets((int)(st + (ft - st) * x / 100f), 0, 0, (int)(sl + (fl - sl) * x / 100f));
                                           _cursor.Box = new Box(Point.Zero, new Size((int)(ss.Width + (fs.Width - ss.Width) * x / 100f), (int)(ss.Height + (fs.Height - ss.Height) * x / 100f)));
                                       }));
            _root.AddAnimation(new Animation<int>(_heartbeat.Margin.Top,
                                                       pos.Y + (e.Control.Box.Height - _heartbeat.Sprite.Height) / 2,
                                                       600, new EasingOut(new QuintEasing()),
                                                       t => _heartbeat.Margin = new Insets(t, 0, 0, 0)));
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
