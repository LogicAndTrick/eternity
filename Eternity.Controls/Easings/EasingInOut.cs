namespace Eternity.Controls.Easings
{
    public class EasingInOut : IEasing
    {
        private readonly IEasing _base;

        public EasingInOut(IEasing baseEasing)
        {
            _base = baseEasing;
        }

        public double CalculateEasing(double timeProgress)
        {
            // Thanks again, jQuery!
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            return timeProgress < 0.5
                       ? _base.CalculateEasing(timeProgress * 2) / 2
                       : 1 - _base.CalculateEasing(timeProgress * -2 + 2) / 2;
        }
    }
}