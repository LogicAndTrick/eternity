﻿using System.Drawing;
using System.Linq;
using Eternity.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Graphics;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class TileControl : LayoutControl
    {
        public Tile Tile { get; private set; }

        public TileControl(Tile tile) : base(new CardLayout())
        {
            Tile = tile;
        }

        public override void OnRender(IRenderContext context)
        {
            context.SetColour(Color.White);
            var box = new Box(0, 0, Box.Width, Box.Height);
            foreach (var group in Tile.BaseGroups.Groups.Where(x => x.Visible))
            {
                foreach (var layer in group.Layers)
                {
                    SpritePool.DrawSprite(context, group.SpriteGroup, layer.SpriteName, box, layer.DrawingOptions);
                }
                context.SetColour(Color.White);
            }
        }

        public void RenderOverlays(IRenderContext context)
        {
            context.SetColour(Color.White);
            var box = new Box(0, 0, Box.Width, Box.Height);
            foreach (var group in Tile.OverlayGroups.Groups.Where(x => x.Visible))
            {
                foreach (var layer in group.Layers)
                {
                    SpritePool.DrawSprite(context, group.SpriteGroup, layer.SpriteName, box, layer.DrawingOptions);
                }
                context.SetColour(Color.White);
            }
        }
    }
}
