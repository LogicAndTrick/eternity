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
        private Point _controlPosition;
        private int _scrollX;
        private int _scrollY;

        public ScrollingMapPanel() : base(null)
        {
            Clip = true;
            Margin = Insets.All(40);
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

        public override Size GetPreferredSize()
        {
            if (Children.Any())
            {
                var ti = TotalInsets;
                return Children[0].GetPreferredSize() + new Size(ti.TotalX, ti.TotalY);
            }
            return base.GetPreferredSize();
        }

        public override void OnMouseMove(EternityEvent ee)
        {
            const int scrollZone = 60;

            _scrollX = _scrollY = 0;

            if (!Children.Any()) return;

            if (ee.X >= 0 && ee.X <= scrollZone) _scrollX = -1; // Scroll LEFT
            else if (ee.X >= Box.Width - scrollZone && ee.X <= Box.Width) _scrollX = 1; // Scroll RIGHT

            if (ee.Y >= 0 && ee.Y <= scrollZone) _scrollY = -1; // Scroll UP
            else if (ee.Y >= Box.Height - scrollZone && ee.Y <= Box.Height) _scrollY = 1; // Scroll DOWN
        }

        private Point ScrollCalculate(Point pt)
        {
            if (!Children.Any()) return pt;
            if (_scrollX == 0 && _scrollY == 0) return pt;

            var ib = InnerBox;

            var x = _scrollX * 3;
            var y = _scrollY * 3;
            if (_scrollX < 0 && pt.X <= 0) x = 0;
            if (_scrollY < 0 && pt.Y <= 0) y = 0;

            var child = Children.First();
            if (_scrollX > 0 && pt.X >= child.Box.Width - ib.Width) x = 0;
            if (_scrollY > 0 && pt.Y >= child.Box.Height - ib.Height) y = 0;

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
            var ib = InnerBox;
            foreach (var child in Children)
            {
                int x, y;

                if (child.Box.Width > ib.Width) x = ib.X - _controlPosition.X;
                else x = ib.X + (ib.Width - child.Box.Width) / 2;

                if (child.Box.Height > ib.Height) y = ib.Y - _controlPosition.Y;
                else y = ib.Y + (ib.Height - child.Box.Height) / 2;

                child.ResizeSafe(new Box(x, y, child.Box.Width, child.Box.Height));
            }
        }
    }
}
