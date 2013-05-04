using System;
using System.Collections.Generic;
using System.Drawing;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;
using Eternity.Input;
using Point = Eternity.DataStructures.Primitives.Point;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Controls.Controls
{
    public class VerticalScrollPanel : LayoutControl
    {
        private class VerticalScrollLayout : ILayout
        {
            private readonly VerticalScrollPanel _panel;

            public VerticalScrollLayout(VerticalScrollPanel panel)
            {
                _panel = panel;
            }

            public Size GetPreferredSize(Control parent, List<Control> children, Dictionary<Control, object> constraints)
            {
                return _panel._preferred;
            }

            public void DoLayout(Control parent, List<Control> children, Dictionary<Control, object> constraints)
            {
                foreach (var child in children)
                {
                    child.ResizeSafe(new Box(new Point(0, -_panel._offset), _panel._preferred));
                }
            }
        }

        private readonly Size _maximumSize;
        protected LayoutControl Child { get; private set; }

        private Size _preferred;
        private int _offset;

        public VerticalScrollPanel(Size maximumSize, LayoutControl child) : base(null)
        {
            Clip = true;
            SetLayout(new VerticalScrollLayout(this));
            _maximumSize = maximumSize;
            Child = child;
            CalculatePreferred();
            Add(Child);
        }

        public override void OnChildSizeChanged()
        {
            CalculatePreferred();
        }

        private void CalculatePreferred()
        {
            var ps = Child.GetPreferredSize();
            _preferred = new Size(_maximumSize.Width, ps.Height);
        }

        public override Size GetPreferredSize()
        {
            CalculatePreferred();
            return new Size(Math.Min(_preferred.Width, _maximumSize.Width), Math.Min(_preferred.Height, _maximumSize.Height));
        }

        public override void OnMouseWheel(EternityEvent e)
        {
            var maxOffset = _preferred.Height - Box.Height;
            var newOffset = -e.Delta * 40;
            var currentOffset = _offset;
            _offset = Math.Max(0, Math.Min(_offset + newOffset, maxOffset));
            DoLayout();
            if (_offset != currentOffset)
            {
                var ee = new EternityEvent {Type = EventType.MouseMove, LastX = e.X, LastY = e.Y - newOffset, X = e.X, Y = e.Y};
                EventBubbleDown(ee);
            }
        }

        public override void OnAfterRender(IRenderContext context)
        {
            if (_preferred.Height <= Box.Height) return;

            var ratio = (Box.Height - 4) / (float) _preferred.Height;
            var h = (int)(Box.Height * ratio);
            var y = (int)(_offset * ratio) + 2;
            var x = Box.Width - 6;
            const int w = 4;

            context.DisableTextures();
            context.StartQuads();

            context.SetColour(Color.FromArgb(128, Color.Gray));
            context.Point2(x, y);
            context.Point2(x + w, y);
            context.Point2(x + w, y + h);
            context.Point2(x, y + h);

            context.SetColour(Color.White);
            context.End();
            context.EnableTextures();
        }
    }
}
