using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Easings;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapEdit
{
    public class SquareCursor : Control, IOverlayControl
    {
        private int _position;
        private IEasing _easing;

        public SquareCursor()
        {
            _easing = new EasingInOut(new QuartEasing());
            Reset();
        }

        private void Reset()
        {
            _position = 0;
            StopAnimations();
            AddAnimation(new Animation<double>(0, 10, x => (x + 0.015) % 1, Progress));
        }

        private void Progress(double progress)
        {
            _position = (int) (_easing.CalculateEasing(progress) * ActualSize.Width);
        }

        public override void OnRender(IRenderContext context)
        {
            double x = 1, y = 0, w = ActualSize.Width - 1, h = ActualSize.Height - 1;
            context.ClearTexture();
            context.StartLines();

            context.SetColour(Color.White);
            context.Point2(x, y);
            context.Point2(w, y);
            context.Point2(w, y);
            context.Point2(w, h);
            context.Point2(w, h);
            context.Point2(x, h);
            context.Point2(x, h);
            context.Point2(x, y);

            var pos = _position;
            context.SetColour(Color.LightGray);
            context.Point2(pos - 8, 0); context.Point2(pos + 8, 1);
            context.Point2(pos - 8, h); context.Point2(pos + 8, h + 1);
            context.SetColour(Color.DarkGray);
            context.Point2(pos - 6, 0); context.Point2(pos + 6, 1);
            context.Point2(pos - 6, h); context.Point2(pos + 6, h + 1);
            context.SetColour(Color.Gray);
            context.Point2(pos - 4, 0); context.Point2(pos + 4, 1);
            context.Point2(pos - 4, h); context.Point2(pos + 4, h + 1);
            context.SetColour(Color.Black);
            context.Point2(pos - 2, 0); context.Point2(pos + 2, 1);
            context.Point2(pos - 2, h); context.Point2(pos + 2, h + 1);

            context.End();
        }

        public bool InterceptEvent(EternityEvent e)
        {
            return false;
        }

        public bool ListenEvent(EternityEvent e)
        {
            return false;
        }
    }
}