using System;

namespace Eternity.DataStructures.Primitives
{
    public class Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public static readonly Point Zero = new Point(0, 0);

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator /(Point p1, int div)
        {
            if (div == 0) throw new DivideByZeroException();
            return new Point(p1.X / div, p1.Y / div);
        }

        public static Point operator *(Point p1, int mul)
        {
            return new Point(p1.X * mul, p1.Y * mul);
        }

        public static Point operator -(Point p)
        {
            return new Point(-p.X, -p.Y);
        }

        protected bool Equals(Point other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
    }
}
