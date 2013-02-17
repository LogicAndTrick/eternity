using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Graphics.Sprites
{
    public class SpriteDrawingOptions
    {
        public enum Dock
        {
            Top, Left,
            Center,
            Bottom, Right
        }

        public bool MirrorX { get; set; }
        public bool MirrorY { get; set; }

        public Dock DockX { get; set; }
        public Dock DockY { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public SpriteDrawingOptions()
        {
            DockX = Dock.Right;
            DockY = Dock.Bottom;
        }

        public double[] CalculateTextureBox(double[] box)
        {
            var x1 = MirrorX ? box[2] : box[0];
            var x2 = !MirrorX ? box[2] : box[0];
            var y1 = MirrorY ? box[3] : box[1];
            var y2 = !MirrorY ? box[3] : box[1];
            return new[] {x1, y1, x2, y2};
        }

        public Box CalculatePositionBox(Box originalBox, int textureWidth, int textureHeight)
        {
            var heightDiff = textureHeight - originalBox.Height;
            var widthDiff = textureWidth - originalBox.Width;

            // Bottom Right
            var bx2 = textureWidth;
            var by2 = textureHeight;
            int bx1, by1;

            if (DockX == Dock.Center)
            {
                bx1 = originalBox.X - widthDiff / 2;
            }
            else if (DockX == Dock.Left)
            {
                bx1 = originalBox.X;
            }
            else // Right
            {
                bx1 = originalBox.X + originalBox.Width - textureWidth;
            }

            if (DockY == Dock.Center)
            {
                by1 = originalBox.Y - heightDiff / 2;
            }
            else if (DockY == Dock.Top)
            {
                by1 = originalBox.Y;
            }
            else // Bottom
            {
                by1 = originalBox.Y + originalBox.Height - textureHeight;
            }

            bx1 += OffsetX;
            by1 += OffsetY;
            return new Box(bx1, by1, bx2, by2);
        }
    }
}
