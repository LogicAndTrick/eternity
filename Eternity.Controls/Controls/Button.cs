using System;
using System.Drawing;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Controls.Controls
{
    public class Button : Control
    {
        public Action Callback { get; set; }

        public string Text
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        public Color Color
        {
            get { return _label.Color; }
            set { _label.Color = value; }
        }

        private readonly Label _label;

        private bool _drawBorder = false;

        public Button(Action callback, string text, Color color = default(Color))
        {
            Callback = callback;
            _label = new Label(text, color);
            Add(_label);
            Box = new Box(Point.Zero, _label.GetPreferredSize().Add(8, 4));
        }

        public override void OnSizeChanged()
        {
            _label.Box = new Box(new Point(2, 2), Box.Size);
        }

        public override void OnMouseEnter(Input.EternityEvent e)
        {
            _drawBorder = true;
        }

        public override void OnMouseLeave(Input.EternityEvent e)
        {
            _drawBorder = false;
        }

        public override void OnMouseDown(Input.EternityEvent e)
        {
            if (Callback != null) Callback();
        }

        public override void OnRender(IRenderContext context)
        {
            context.DisableTextures();

            context.StartQuads();
            context.SetColour(Color.Teal);

            context.Point2(0, 0);
            context.Point2(Box.Width, 0);
            context.Point2(Box.Width, Box.Height);
            context.Point2(0, Box.Height);

            context.End();

            if (_drawBorder)
            {
                context.StartLines();
                context.SetColour(Color.White);

                context.Point2(0, 0);
                context.Point2(Box.Width, 0);
                context.Point2(Box.Width, 0);
                context.Point2(Box.Width, Box.Height);
                context.Point2(Box.Width, Box.Height);
                context.Point2(0, Box.Height);
                context.Point2(0, Box.Height);
                context.Point2(0, 0);

                context.End();
            }

            context.EnableTextures();

            context.SetColour(Color.Transparent);
        }
    }
}
