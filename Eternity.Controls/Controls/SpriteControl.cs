using Eternity.DataStructures.Primitives;
using Eternity.Graphics.Sprites;

namespace Eternity.Controls.Controls
{
    public class SpriteControl : Control
    {
        public SpriteReference Sprite { get; set; }
        public SpriteDrawingOptions DrawingOptions { get; private set; }

        public SpriteControl(SpriteReference sprite)
        {
            Sprite = sprite;
            DrawingOptions = new SpriteDrawingOptions();
        }

        public SpriteControl(string group, string name)
        {
            Sprite = new SpriteReference(group, name);
            DrawingOptions = new SpriteDrawingOptions();
        }

        public override Size GetPreferredSize()
        {
            return new Size(Sprite.Width, Sprite.Height);
        }

        public override void OnRender(Graphics.IRenderContext context)
        {
            SpritePool.DrawSprite(context, Sprite.Group, Sprite.Name, new Box(0, 0, Box.Width, Box.Height), DrawingOptions);
        }
    }
}
