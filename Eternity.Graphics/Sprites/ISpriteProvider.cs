using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Graphics.Sprites
{
    public interface ISpriteProvider
    {
        string Group { get; }
        bool HasSprite(string name);
        int GetWidth(string name);
        int GetHeight(string name);
        void DrawSprite(IRenderContext context, string name, Box box, SpriteDrawingOptions options);
    }
}
