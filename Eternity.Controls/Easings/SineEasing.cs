using System;

namespace Eternity.Controls.Easings
{
    public class SineEasing : IEasing
    {
        public virtual double CalculateEasing(double p)
        {
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            return 1 - Math.Cos(p * Math.PI / 2);
        }
    }
}