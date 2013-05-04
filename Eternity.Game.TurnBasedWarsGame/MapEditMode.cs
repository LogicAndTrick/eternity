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

        public MapEditMode(ResourceDefinition mapDefinition)
        {
            _map = null;
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
            _root = new LayoutControl(new CardLayout())
            {
                Box = new Box(new Point(0, 0), new Point(context.ScreenWidth, context.ScreenHeight))
            };

            _root.Add(new GradientBackground(Color.Cyan, Color.Black));

            var container = new LayoutControl(new BorderLayout());

            container.Add(new TerrainList(), Direction.Left);

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
