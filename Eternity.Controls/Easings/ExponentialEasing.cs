using System;

namespace Eternity.Controls.Easings
{
    public class ExponentialEasing : IEasing
    {
        private readonly int _exp;

        public ExponentialEasing(int exp)
        {
            _exp = exp;
        }

        public virtual double CalculateEasing(double p)
        {
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            return Math.Pow(p, _exp);
        }
    }

    public class QuadEasing  : ExponentialEasing { public QuadEasing()  : base(2) { } }
    public class CubicEasing : ExponentialEasing { public CubicEasing() : base(3) { } }
    public class QuartEasing : ExponentialEasing { public QuartEasing() : base(4) { } }
    public class QuintEasing : ExponentialEasing { public QuintEasing() : base(5) { } }
    public class ExpoEasing  : ExponentialEasing { public ExpoEasing()  : base(6) { } }
}