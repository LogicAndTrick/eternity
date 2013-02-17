using System;

namespace Eternity.Graphics.Textures
{
    public interface ITexture : IDisposable
    {
        string Name { get; }

        int Width { get; }
        int Height { get; }

        decimal ScaleX { get; set; }
        decimal ScaleY { get; set; }

        decimal OffsetX { get; set; }
        decimal OffsetY { get; set; }

        int TextureReference { get; }
    }
}
