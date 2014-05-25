namespace Eternity.DataStructures.Primitives
{
    public class Insets
    {
        public double Top { get; private set; }
        public double Left { get; private set; }
        public double Right { get; private set; }
        public double Bottom { get; private set; }

        public double TotalX { get { return Left + Right; } }
        public double TotalY { get { return Top + Bottom; } }

        public Insets(double top, double right, double bottom, double left)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        public static Insets All(double val)
        {
            return new Insets(val, val, val, val);
        }
    }
}
