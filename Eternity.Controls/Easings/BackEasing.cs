namespace Eternity.Controls.Easings
{
    public class BackEasing : IEasing
    {
        public double CalculateEasing(double p)
        {
            // https://github.com/jquery/jquery-ui/ [ui/jquery.effects.core.js]
            return p * p * (3 * p - 2);
        }
    }
}
