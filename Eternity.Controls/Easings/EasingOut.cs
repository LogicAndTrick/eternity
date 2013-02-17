namespace Eternity.Controls.Easings
{
    public class EasingOut : IEasing
    {
        private readonly IEasing _base;

        public EasingOut(IEasing baseEasing)
        {
            _base = baseEasing;
        }

        public double CalculateEasing(double timeProgress)
        {
            // Thanks again, jQuery!
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            return 1 - _base.CalculateEasing(1 - timeProgress);
        }
    }
}