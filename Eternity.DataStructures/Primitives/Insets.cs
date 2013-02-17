namespace Eternity.DataStructures.Primitives
{
    public class Insets
    {
        public int Top { get; private set; }
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }

        public int TotalX { get { return Left + Right; } }
        public int TotalY { get { return Top + Bottom; } }

        public Insets(int top, int right, int bottom, int left)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }
    }
}
