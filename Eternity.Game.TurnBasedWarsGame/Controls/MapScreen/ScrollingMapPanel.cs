using System;
using System.Linq;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.DataStructures.Primitives;
using Eternity.Input;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class ScrollingMapPanel : LayoutControl
    {
        public Insets Border { get; private set; }
        private Point _controlPosition;
        private int _scrollX;
        private int _scrollY;

        public ScrollingMapPanel() : this(new Insets(40, 40, 40, 40))
        {
            // Nothing
        }

        public ScrollingMapPanel(Insets border) : base(null)
        {
            Border = border;
            _controlPosition = new Point(0, 0);
            _scrollX = _scrollX = 0;
            AddAnimation(new Animation<Point>(_controlPosition, 10, ScrollCalculate, ScrollCallback));
        }

        protected override void OnAdd(Control control)
        {
            if (Children.Count > 1)
            {
                throw new Exception("The ScrollingMapPanel can only have one child.");
            }
        }

        public override void OnMouseMove(EternityEvent ee)
        {
            const int scrollZone = 60;

            _scrollX = _scrollY = 0;

            if (!Children.Any()) return;

            var loc = GetLocationInTree();
            var mse = new Point(ee.X - loc.X, ee.Y - loc.Y);

            if (mse.X >= 0 && mse.X <= scrollZone) _scrollX = -1; // Scroll LEFT
            else if (mse.X >= Box.Width - scrollZone && mse.X <= Box.Width) _scrollX = 1; // Scroll RIGHT

            if (mse.Y >= 0 && mse.Y <= scrollZone) _scrollY = -1; // Scroll UP
            else if (mse.Y >= Box.Height - scrollZone && mse.Y <= Box.Height) _scrollY = 1; // Scroll DOWN
        }

        private Point ScrollCalculate(Point pt)
        {
            if (!Children.Any()) return pt;
            if (_scrollX == 0 && _scrollY == 0) return pt;

            var x = _scrollX * 3;
            var y = _scrollY * 3;
            if (_scrollX < 0 && pt.X <= 0) x = 0;
            if (_scrollY < 0 && pt.Y <= 0) y = 0;

            var child = Children.First();
            if (_scrollX > 0 && pt.X >= child.Box.Width - Box.Width + Border.TotalX) x = 0;
            if (_scrollY > 0 && pt.Y >= child.Box.Height - Box.Height + Border.TotalY) y = 0;

            return new Point(pt.X + x, pt.Y + y);
        }

        private void ScrollCallback(Point pt)
        {
            _controlPosition = pt;
            OnChildSizeChanged();
        }

        public override void OnMouseLeave(EternityEvent e)
        {
            _scrollX = _scrollY = 0;
        }

        public override void OnWindowFocusChanged(EternityEvent e)
        {
            if (!e.Focused)
            {
                _scrollX = _scrollY = 0;
            }
        }

        protected override void OnDoLayout()
        {
            var availWid = Box.Width - Border.TotalX;
            var availHei = Box.Height - Border.TotalY;
            foreach (var child in Children)
            {
                int x, y;

                if (child.Box.Width > availWid) x = Border.Left - _controlPosition.X;
                else x = Border.Left + (availWid - child.Box.Width) / 2;

                if (child.Box.Height > availHei) y = Border.Top - _controlPosition.Y;
                else y = Border.Top + (availHei - child.Box.Height) / 2;

                child.ResizeSafe(new Box(x, y, child.Box.Width, child.Box.Height));
            }
        }
    }
}
