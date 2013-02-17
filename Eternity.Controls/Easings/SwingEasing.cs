using System;

namespace Eternity.Controls.Easings
{
    public class SwingEasing : IEasing
    {
        public double CalculateEasing(double timeProgress)
        {
            // Thanks, jQuery! https://github.com/jquery/jquery [src/effects.js]
            return -Math.Cos(timeProgress * Math.PI) / 2 + 0.5;
        }
    }
}