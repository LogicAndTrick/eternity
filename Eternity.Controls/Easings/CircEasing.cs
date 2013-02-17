using System;

namespace Eternity.Controls.Easings
{
    public class CircEasing : IEasing
    {
        public virtual double CalculateEasing(double p)
        {
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            return 1 - Math.Sqrt(1 - p * p);
        }
    }
}