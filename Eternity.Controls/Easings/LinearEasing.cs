namespace Eternity.Controls.Easings
{
    public class LinearEasing : IEasing
    {
        public double CalculateEasing(double timeProgress)
        {
            return timeProgress;
        }
    }
}