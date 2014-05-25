using System;

namespace Eternity.DataStructures.Primitives
{
    public class Size
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public static readonly Size Zero = new Size(0, 0);

        public Size(double width, double height)
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
            return Width.Equals(other.Width) && Height.Equals(other.Height);
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
                return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
            }
        }

        public Size Add(int x, int y)
        {
            return new Size(Width + x, Height + y);
        }
    }
}