using System;

namespace Eternity.Controls.Easings
{
    public class ElasticEasing : IEasing
    {
        public virtual double CalculateEasing(double p)
        {
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            return p < 0.0001 || p > 0.9999
                       ? p
                       : Math.Pow(2, 8 * (p - 1)) * Math.Sin(((p - 1) * 80 - 7.5) * Math.PI / 15);
        }
    }
}