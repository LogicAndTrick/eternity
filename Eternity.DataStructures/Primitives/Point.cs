using System;

namespace Eternity.DataStructures.Primitives
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static readonly Point Zero = new Point(0, 0);

        public Point(int x, int y)
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

        public static bool Equals(Point p1, Point p2)
        {
            return Object.Equals(p1, p2);
        }

        public bool Equals(Point other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Point)) return false;
            return Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}
