using Eternity.Controls;
using System.Drawing;
using Eternity.Graphics;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class GradientBackground : Control
    {
        private readonly Color _startColour;
        private readonly Color _endColour;
        private readonly Color _halfwayColour;

        public GradientBackground(Color startColour, Color endColour)
        {
            _startColour = startColour;
            _endColour = endColour;
            _halfwayColour = Color.FromArgb(
                (_startColour.A + _endColour.A) / 2,
                (_startColour.R + _endColour.R) / 2,
                (_startColour.G + _endColour.G) / 2,
                (_startColour.B + _endColour.B) / 2
                );
        }

        public GradientBackground(Color startColour, Color halfwayColour, Color endColour)
        {
            _startColour = startColour;
            _endColour = endColour;
            _halfwayColour = halfwayColour;
        }

        public override void OnRender(IRenderContext context)
        {
            //var h = Math.Sqrt(Box.Width * Box.Width + Box.Height * Box.Height * 1.0);
            //var w = (Box.Height * Box.Width) / h;
            //var yc = (int) ((Box.Width / h) * w);
            //var xc = (int) Math.Sqrt(w * w - yc * yc);
            //var p1 = Box.BottomLeft + new Point(-xc, -yc);
            //var p2 = Box.BottomLeft + new Point(xc, yc);
            //var p3 = Box.TopRight + new Point(xc, yc);
            //var p4 = Box.TopRight + new Point(-xc, -yc);

            context.DisableTextures();
            context.StartQuads();

            context.SetColour(_startColour);
            context.Point2(0, 0);
            context.SetColour(_halfwayColour);
            context.Point2(0, Box.Height);
            context.SetColour(_endColour);
            context.Point2(Box.Width, Box.Height);
            context.SetColour(_halfwayColour);
            context.Point2(Box.Width, 0);
            context.SetColour(Color.White);

            context.End();
            context.EnableTextures();
        }
    }
}
