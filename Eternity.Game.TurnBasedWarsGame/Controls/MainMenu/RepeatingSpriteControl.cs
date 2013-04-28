using Eternity.Controls;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class RepeatingSpriteControl : Control
    {
        public SpriteReference Sprite { get; set; }
        public SpriteDrawingOptions DrawingOptions { get; private set; }
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public RepeatingSpriteControl(SpriteReference sprite)
        {
            Sprite = sprite;
            DrawingOptions = new SpriteDrawingOptions();
        }

        public RepeatingSpriteControl(string group, string name)
        {
            Sprite = new SpriteReference(group, name);
        }

        public override Size GetPreferredSize()
        {
            return new Size(Sprite.Width, Sprite.Height);
        }

        public override void OnUpdate(FrameInfo info, Input.IInputState state)
        {
            OffsetX = (OffsetX + 1) % Sprite.Width;
        }

        public override void OnRender(IRenderContext context)
        {
            var offset = RepeatX ? OffsetX - Sprite.Width : OffsetX;
            for (var i = Box.X + offset; i < Box.Width; i += Sprite.Width)
            {
                DrawVertical(context, i);
                if (!RepeatX) break;
            }
        }

        private void DrawVertical(IRenderContext context, int x)
        {
            var offset = RepeatY ? OffsetY - Sprite.Height : OffsetY;
            for (var i = Box.Y + offset; i < Box.Height; i += Sprite.Height)
            {
                Draw(context, x, i);
                if (!RepeatY) break;
            }
        }

        private void Draw(IRenderContext context, int x, int y)
        {
            SpritePool.DrawSprite(context, Sprite, new Box(x, y, Sprite.Width, Sprite.Height), DrawingOptions);
        }
    }
}
