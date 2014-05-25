using System;

namespace Eternity.DataStructures.Primitives
{
    public class Box
    {

        public Point TopLeft { get; private set; }
        public Point TopRight { get; private set; }
        public Point BottomLeft { get; private set; }
        public Point BottomRight { get; private set; }

        public Line Top { get; private set; }
        public Line Left { get; private set; }
        public Line Right { get; private set; }
        public Line Bottom { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Size Size
        {
            get { return new Size(Width, Height); }
        }

        public Box(int x, int y, int width, int height)
            : this(new Point(x, y), new Point(width + x, height + y))
        {
            
        }

        public Box(Point point, Size size)
            : this(point, new Point(point.X + size.Width, point.Y + size.Height))
        {
            
        }

        public Box(Point start, Point end)
        {
            var minx = Math.Min(start.X, end.X);
            var miny = Math.Min(start.Y, end.Y);
            var maxx = Math.Max(Math.Max(start.X, end.X) - 1, minx);
            var maxy = Math.Max(Math.Max(start.Y, end.Y) - 1, miny);

            TopLeft = new Point(minx, miny);
            TopRight = new Point(maxx, miny);
            BottomLeft = new Point(minx, maxy);
            BottomRight = new Point(maxx, maxy);

            Top = new Line(TopLeft, TopRight);
            Right = new Line(TopRight, BottomRight);
            Bottom = new Line(BottomRight, BottomLeft);
            Left = new Line(BottomLeft, TopLeft);

            X = minx;
            Y = miny;
            Width = (maxx - minx) + 1;
            Height = (maxy - miny) + 1;
        }

        public bool Contains(Point p)
        {
            return X <= p.X && (X + Width) > p.X &&
                   Y <= p.Y && (Y + Height) > p.Y;
        }

        public Point GetCenter()
        {
            return (TopLeft + BottomRight) / 2;
        }

        public Box Offset(int x, int y)
        {
            return new Box(X + x, Y + y, Width, Height);
        }

        public double DistanceFrom(Point p)
        {
            var xd = p.X < X ? X - p.X : (p.X > X + Width ? p.X - (X + Width) : 0);
            var yd = p.Y < Y ? Y - p.Y : (p.Y > Y + Height ? p.Y - (Y + Height) : 0);
            return Math.Sqrt(xd * xd + yd * yd);
        }
    }
}
