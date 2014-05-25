using System;
using System.Drawing;
using Eternity.Graphics.Textures;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Graphics
{
    /// <summary>
    /// Uses System.Drawing for 2d text rendering.
    /// Borrowed from the OpenTK text rendering example.
    /// </summary>
    public class TextRenderer : IDisposable, IRenderable
    {
        public static Font DefaultFont { get; set; }

        static TextRenderer()
        {
            DefaultFont = SystemFonts.DefaultFont;
        }

        private Bitmap _bmp;
        private System.Drawing.Graphics _gfx;
        private ITexture _texture;

        private Rectangle _dirtyRegion;
        private bool _disposed;

        public int Width { get { return _texture.Width; } }
        public int Height { get { return _texture.Height; } }

        public TextRenderer(int width, int height)
        {
            Init(width, height);
        }

        public TextRenderer(string text, Font font = null)
        {
            var size = GetPreferredSize(text, font);
            Init((int) size.Width, (int) size.Height);
            //DrawString(text, font);
        }

        public Size GetPreferredSize(string text, Font font = null)
        {
            using (var b = new Bitmap(1, 1))
            {
                using (var g = System.Drawing.Graphics.FromImage(b))
                {
                    var size = g.MeasureString(text, font ?? DefaultFont);
                    return new Size((int) size.Width, (int) size.Height);
                }
            }
        }

        public TextRenderer(string text, int width, int height, Font font = null)
        {
            Init(width, height);
            //DrawString(text, font);
        }

        private void Init(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentOutOfRangeException("height");
            if (GraphicsContext.CurrentContext == null) throw new InvalidOperationException("No GraphicsContext is current on the calling thread.");

            _bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            _gfx = System.Drawing.Graphics.FromImage(_bmp);
            _gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            var texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            _texture = new GLTexture(texture, ToString() + DateTime.Now.Ticks, width, height);
        }

        /// <summary>
        /// Clears the backing store to the specified color.
        /// </summary>
        /// <param name="color">A <see cref="System.Drawing.Color"/>.</param>
        public void Clear(Color color)
        {
            _gfx.Clear(color);
            _dirtyRegion = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
        }

        /// <summary>
        /// Draws the specified string to the backing store.
        /// </summary>
        /// <param name="text">The <see cref="System.String"/> to draw.</param>
        /// <param name="font">The <see cref="System.Drawing.Font"/> that will be used.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush"/> that will be used.</param>
        /// <param name="point">The location of the text on the backing store, in 2d pixel coordinates.
        /// The origin (0, 0) lies at the top-left corner of the backing store.</param>
        public void DrawString(string text, Font font = null, Brush brush = null, PointF point = default(PointF))
        {
            _gfx.DrawString(text, font ?? DefaultFont, brush ?? Brushes.White, point);

            var size = _gfx.MeasureString(text, font ?? DefaultFont);
            _dirtyRegion = Rectangle.Round(RectangleF.Union(_dirtyRegion, new RectangleF(point, size)));
            _dirtyRegion = Rectangle.Intersect(_dirtyRegion, new Rectangle(0, 0, _bmp.Width, _bmp.Height));
            _dirtyRegion = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
        }

        public void Render(IRenderContext context)
        {
            UploadBitmap();
            context.SetTexture(_texture);

            context.StartQuads();

            context.TexturePoint(0, 0);
            context.Point2(0, 0);

            context.TexturePoint(1, 0);
            context.Point2(_texture.Width, 0);

            context.TexturePoint(1, 1);
            context.Point2(_texture.Width, _texture.Height);

            context.TexturePoint(0, 1);
            context.Point2(0, _texture.Height);

            context.End();
        }

        // Uploads the dirty regions of the backing store to the OpenGL texture.
        private void UploadBitmap()
        {
            if (_dirtyRegion == RectangleF.Empty) return;

            var data = _bmp.LockBits(_dirtyRegion,
                                     System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                     System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, _texture.TextureReference);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                             _dirtyRegion.X, _dirtyRegion.Y, _dirtyRegion.Width, _dirtyRegion.Height,
                             PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            _bmp.UnlockBits(data);

            _dirtyRegion = Rectangle.Empty;
        }

        private void Dispose(bool manual)
        {
            if (_disposed) return;

            if (manual)
            {
                _bmp.Dispose();
                _gfx.Dispose();
                _texture.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TextRenderer()
        {
            Console.WriteLine("[Warning] Resource leaked: {0}.", typeof(TextRenderer));
        }
    }
}
