using System;

namespace Eternity.DataStructures.Primitives
{
    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public static readonly Size Zero = new Size(0, 0);

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static Size operator +(Size s1, Size s2)
        {
            return new Size(s1.Width + s2.Width, s1.Height + s2.Height);
        }

        public static Size operator -(Size s1, Size s2)
        {
            return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
        }

        public static Size operator /(Size s1, int div)
        {
            if (div == 0) throw new DivideByZeroException();
            return new Size(s1.Width / div, s1.Height / div);
        }

        public static Size operator *(Size s1, int mul)
        {
            return new Size(s1.Width * mul, s1.Height * mul);
        }

        public static Size operator -(Size s)
        {
            return new Size(-s.Width, -s.Height);
        }

        protected bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Size) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }

        public Size Add(int x, int y)
        {
            return new Size(Width + x, Height + y);
        }
    }
}