using System.Drawing;
using Eternity.Controls;
using Eternity.Graphics;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuHeaderControl : Control
    {
        public MenuHeaderControl()
        {
            PreferredSize = new Size(200, 20);
        }

        public override void OnRender(IRenderContext context)
        {
            context.DisableTextures();
            context.StartQuads();

            context.SetColour(Color.Red);
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