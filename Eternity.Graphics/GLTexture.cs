using System;
using Eternity.Graphics.Textures;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Eternity.Graphics
{
    public class GLTexture : ITexture
    {
        public string Name { get; protected set; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public decimal ScaleX { get; set; }
        public decimal ScaleY { get; set; }
        public decimal OffsetX { get; set; }
        public decimal OffsetY { get; set; }

        public int TextureReference { get; protected set; }

        public GLTexture(int textureReference, string name, int width, int height)
        {
            TextureReference = textureReference;
            Name = name;
            Width = width;
            Height = height;
            ScaleX = ScaleY = 1;
            OffsetX = OffsetY = 0;
        }

        public void Dispose()
        {
            if (GraphicsContext.CurrentContext != null)
            {
                GL.DeleteTexture(TextureReference);
            }
        }
    }
}
