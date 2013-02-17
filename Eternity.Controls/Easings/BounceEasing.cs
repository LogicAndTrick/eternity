using System;

namespace Eternity.Controls.Easings
{
    public class BounceEasing : IEasing
    {
        public virtual double CalculateEasing(double p)
        {
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            double pow2;
            var bounce = 4;
            while (p < ((pow2 = Math.Pow(2, --bounce)) - 1) / 11) { }
            return 1 / Math.Pow(4, 3 - bounce) - 7.5625 * Math.Pow((pow2 * 3 - 2) / 22 - p, 2);
        }
    }
}