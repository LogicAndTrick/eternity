using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Layouts;
using Eternity.Graphics;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class DebugControl : LayoutControl
    {
        private Color _color;
        public DebugControl(ILayout layout, Color color) : base(layout)
        {
            _color = color;
        }

        public override void OnRender(IRenderContext context)
        {
            context.DisableTextures();
            context.StartQuads();

            context.SetColour(_color);
            context.Point2(0, 0);
            context.Point2(Box.Width, 0);
            context.Point2(Box.Width, Box.Height);
            context.Point2(0, Box.Height);

            context.SetColour(Color.White);
            context.End();
            context.EnableTextures();
        }
    }
}