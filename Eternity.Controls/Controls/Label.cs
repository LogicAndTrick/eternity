using System.Drawing;
using Eternity.Graphics;

namespace Eternity.Controls.Controls
{
    public class Label : Control
    {
        public string Text
        {
            get { return _text; }
            set {
                _text = value;
                UpdateRenderer();
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateRenderer();
            }
        }

        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                UpdateRenderer();
            }
        }

        private TextRenderer _renderer;
        private string _text;
        private Color _color;
        private Font _font;

        public Label(string text, Color color = default(Color), Font font = null)
        {
            _text = text;
            _color = color;
            _font = font;
            UpdateRenderer();
        }

        public override DataStructures.Primitives.Size GetPreferredSize()
        {
            return _renderer.GetPreferredSize(_text, Font);
        }

        private void UpdateRenderer()
        {
            if (_renderer != null) _renderer.Dispose();
            _renderer = new TextRenderer(_text, (int) Box.Width, (int) Box.Height);
            using (var brush = new SolidBrush(Color))
            {
                _renderer.DrawString(Text, _font, brush);
            }
        }

        public override void OnSizeChanged()
        {
            UpdateRenderer();
        }

        public override void OnRender(IRenderContext context)
        {
            context.SetColour(Color.White);
            _renderer.Render(context);
        }

        public override void OnDispose()
        {
            _renderer.Dispose();
        }
    }
}
