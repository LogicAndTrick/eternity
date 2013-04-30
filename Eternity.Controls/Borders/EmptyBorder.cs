using Eternity.DataStructures.Primitives;
using Eternity.Graphics;

namespace Eternity.Controls.Borders
{
    public class EmptyBorder : Border
    {
        public EmptyBorder(int top, int right, int bottom, int left)
            : base(top, right, bottom, left)
        {
        }

        public EmptyBorder(int width)
            : base(width)
        {
        }

        public EmptyBorder(Insets insets)
            : base(insets)
        {
        }

        public override void Draw(Control parent, IRenderContext context)
        {

        }
    }
}