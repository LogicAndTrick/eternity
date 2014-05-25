using System;
using System.Linq;

namespace Eternity.DataStructures.Primitives
{
    public class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Test if this line intersects another
        /// </summary>
        /// <remarks>
        /// http://www.gamedev.net/topic/440350-2d-line-box-intersection/
        /// http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
        /// </remarks>
        /// <param name="that"></param>
        /// <param name="intersectionPoint">Guaranteed to be null iff the return value is false.</param>
        /// <returns></returns>
        public bool Intersects(Line that, out Point intersectionPoint)
        {
            intersectionPoint = null;

            var v1 = Start;
            var v2 = End;
            var v3 = that.Start;
            var v4 = that.End;

            var denom = ((v4.Y - v3.Y) * (v2.X - v1.X)) - ((v4.X - v3.X) * (v2.Y - v1.Y));
            var numerator = ((v4.X - v3.X) * (v1.Y - v3.Y)) - ((v4.Y - v3.Y) * (v1.X - v3.X));
            var numerator2 = ((v2.X - v1.X) * (v1.Y - v3.Y)) - ((v2.Y - v1.Y) * (v1.X - v3.X));

            if (denom == 0)
            {
                return false;
            }

            var ua = numerator / (decimal) denom;
            var ub = numerator2 / (decimal) denom;

            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            {
                var x = v1.X + ua * (v2.X - v1.X);
                var y = v1.Y + ua * (v2.Y - v1.Y);
                intersectionPoint = new Point((int) x, (int) y);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Test if this line intersects a box
        /// </summary>
        /// <remarks>http://www.gamedev.net/topic/440350-2d-line-box-intersection/</remarks>
        /// <param name="that"></param>
        /// <param name="entryPoint"></param>
        /// <param name="exitPoint"></param>
        /// <returns></returns>
        public bool Intersects(Box that, out Point entryPoint, out Point exitPoint)
        {
            // Check if it is inside
            var startInside = that.Contains(Start);
            var endInside = that.Contains(End);
            if (startInside && endInside)
            {
                // Completely inside, no in/out 
                entryPoint = exitPoint = null;
                return true;
            }
            // Check each line for intersection
            Point ti, bi, li, ri;
            // NOTE: Don't use short-curcuiting ORs here, we want ALL the intersects!
            var isect = Intersects(that.Top, out ti)
                        | Intersects(that.Left, out li)
                        | Intersects(that.Right, out ri)
                        | Intersects(that.Bottom, out bi);
            if (!isect)
            {
                entryPoint = exitPoint = null;
                return false;
            }

            // Get the intersecting points
            var points = new[] { ti, li, ri, bi }.Where(x => x != null).ToList();
            // Entry point = closest intersecting point to the start of the line
            entryPoint = points.OrderBy(x => (x - Start).Magnitude()).FirstOrDefault();
            // Exit point = closest intersecting point to the end of the line
            exitPoint = points.OrderBy(x => (End - x).Magnitude()).FirstOrDefault();

            if (startInside) entryPoint = null;
            if (endInside) exitPoint = null;

            return true;
        }

        public decimal Length()
        {
            var x = End.X - Start.X;
            var y = End.Y - Start.Y;
            return (decimal) Math.Sqrt(x * x + y * y);
        }
    }
}
