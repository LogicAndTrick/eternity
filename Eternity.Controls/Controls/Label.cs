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

        private TextRenderer _renderer;
        private string _text;
        private Color _color;

        public Label(string text, Color color = default(Color))
        {
            _text = text;
            _color = color;
            UpdateRenderer();
        }

        public override DataStructures.Primitives.Size GetPreferredSize()
        {
            return _renderer.GetPreferredSize(_text);
        }

        private void UpdateRenderer()
        {
            if (_renderer != null) _renderer.Dispose();
            _renderer = new TextRenderer(_text, Box.Width, Box.Height);
            using (var brush = new SolidBrush(Color))
            {
                _renderer.DrawString(Text, null, brush);
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
