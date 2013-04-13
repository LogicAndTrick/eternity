namespace Eternity.Controls.Easings
{
    public static class EasingExtensions
    {
        public static IEasing Out(this IEasing easing)
        {
            return new EasingOut(easing);
        }
        public static IEasing InOut(this IEasing easing)
        {
            return new EasingInOut(easing);
        }
    }
}