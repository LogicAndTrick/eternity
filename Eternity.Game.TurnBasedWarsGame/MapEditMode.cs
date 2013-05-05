using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Eternity.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.Controls.MapEdit;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.COs;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;
using Eternity.Graphics.Textures;
using Eternity.Input;
using Eternity.Resources;
using OpenTK.Graphics.OpenGL;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Game.TurnBasedWarsGame
{
    public class MapEditMode : IGameMode
    {
        private LayoutControl _root;
        private Map _map;
        private readonly List<Army> _armies;

        public MapEditMode(ResourceDefinition mapDefinition)
        {
            _armies = new List<Army>();
            _map = new Map(GetArmy, mapDefinition);
        }

        private Army GetArmy(string army)
        {
            var a = _armies.FirstOrDefault(x => x.ArmyRules.Name == army);
            if (a == null)
            {
                a = new Army(new CO(), army);
                _armies.Add(a);
            }
            return a;
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

            SpriteManager.RegisterResourceGroup(context, "MapEdit");
            TextureManager.RegisterResourceGroup(context, "MapEdit");

            InitialiseControls(context);
        }

        private void InitialiseControls(IRenderContext context)
        {
            _root = new LayoutControl(new CardLayout())
            {
                Box = new Box(new Point(0, 0), new Point(context.ScreenWidth, context.ScreenHeight))
            };

            _root.Add(new GradientBackground(Color.White, Color.MediumPurple));

            var gameBoard = new GameBoard(_map);
            foreach (var tile in _map.Tiles)
            {
                gameBoard.Add(new TileControl(tile), tile.Location);
            }

            var scrollingMapPanel = new ScrollingMapPanel();
            scrollingMapPanel.Add(gameBoard);

            var sidebar = new LayoutControl(new BorderLayout());
            sidebar.Add(new ArmyList(), Direction.Bottom);
            sidebar.Add(new TerrainList(), Direction.Center);

            var container = new LayoutControl(new BorderLayout());
            container.Add(sidebar, Direction.Left);
            container.Add(scrollingMapPanel, Direction.Center);

            _root.Add(container);
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
