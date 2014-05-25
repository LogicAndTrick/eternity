using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;

namespace Eternity.Controls.Borders
{
    public class LineBorder : Border
    {
        public Color Colour { get; set; }

        public LineBorder(Color colour, int top, int right, int bottom, int left)
            : base(top, right, bottom, left)
        {
            Colour = colour;
        }

        public LineBorder(Color colour, int width)
            : base(width)
        {
            Colour = colour;
        }

        public LineBorder(Color colour, Insets insets)
            : base(insets)
        {
            Colour = colour;
        }

        private void Rectangle(double x, double y, double w, double h, IRenderContext context)
        {
            context.Point2(x, y);
            context.Point2(x + w, y);
            context.Point2(x + w, y + h);
            context.Point2(x, y + h);
        }

        public override void Draw(Control parent, IRenderContext context)
        {
            context.DisableTextures();
            context.StartQuads();
            context.SetColour(Colour);

            if (Left > 0) Rectangle(parent.Margin.Left, parent.Margin.Top, Left, parent.Box.Height - parent.Margin.TotalY, context);
            if (Right > 0) Rectangle(parent.Box.Width - parent.Margin.Right - Right, parent.Margin.Top, Right, parent.Box.Height - parent.Margin.TotalY, context);
            if (Top > 0) Rectangle(parent.Margin.Left, parent.Margin.Top, parent.Box.Width - parent.Margin.TotalX, Top, context);
            if (Bottom > 0) Rectangle(parent.Margin.Left, parent.Box.Height - parent.Margin.Bottom - Bottom, parent.Box.Width - parent.Margin.TotalX, Bottom, context);

            context.End();
            context.EnableTextures();
        }
    }
}
