using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Eternity.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;
using Eternity.Graphics.Textures;
using Eternity.Input;
using OpenTK.Graphics.OpenGL;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Game.TurnBasedWarsGame
{
    public class TurnBasedWarsMode : IGameMode
    {
        private LayoutControl _root;
        private Battle _battle;

        public TurnBasedWarsMode(Battle battle)
        {
            _battle = battle;
        }

        public void StartUp(IRenderContext context)
        {
            SpriteManager.RegisterResourceGroup(context, "Terrain");
            TextureManager.RegisterResourceGroup(context, "Terrain");

            SpriteManager.RegisterResourceGroup(context, "Units");
            TextureManager.RegisterResourceGroup(context, "Units");

            SpriteManager.RegisterResourceGroup(context, "UnitAnimations");
            TextureManager.RegisterResourceGroup(context, "UnitAnimations");

            SpriteManager.RegisterResourceGroup(context, "Overlays");
            TextureManager.RegisterResourceGroup(context, "Overlays");

            InitialiseControls(context);
        }

        private void InitialiseControls(IRenderContext context)
        {
            _root = new RootPanel(_battle)
            {
                Box = new Box(new Point(0, 0), new Point(context.ScreenWidth, context.ScreenHeight))
            };

            _root.Add(new GradientBackground(Color.Cyan, Color.Black));
            var scrollingMapPanel = new ScrollingMapPanel();
            var gameBoard = new GameBoard(_battle.Map);
            foreach (var tile in _battle.Map.Tiles)
            {
                gameBoard.Add(new TileControl(tile), tile.Location);
            }
            _battle.GameBoard = gameBoard;
            gameBoard.TileHovered += (s, e) => _battle.TileHovered(e.Tile);
            gameBoard.TileMouseDown += (s, e) => _battle.TileMouseDown(e.Event, e.Tile);
            gameBoard.TileMouseUp += (s, e) => _battle.TileMouseUp(e.Event, e.Tile);
            scrollingMapPanel.Add(gameBoard);
            _root.Add(scrollingMapPanel);

            var chrome = new ChromeOverlay();
            var tileInfo = new TileInfoChromeControl {Box = new Box(0, 0, 100, 50)};
            chrome.Add(tileInfo,
                       new ChromeConstraints
                           {
                               AvoidCursor = true,
                               StickyDirection = Direction.Top,
                               CurrentDirection = Direction.Left
                           });

            _battle.TileInfoChromeControl = tileInfo;

            scrollingMapPanel.AddOverlay(chrome);

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
