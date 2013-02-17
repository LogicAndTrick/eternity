using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;

namespace Eternity.Graphics.Sprites
{
    public class SpriteReference : ISpriteProvider
    {
        private readonly ISpriteProvider _provider;

        public string Group { get { return _provider.Group; } }
        public string Name { get; private set; }
        public int Width { get { return _provider.GetWidth(Name); } }
        public int Height { get { return _provider.GetHeight(Name); } }

        public SpriteReference(ISpriteProvider provider, string name)
        {
            _provider = provider;
            Name = name;
        }

        public SpriteReference(string group, string name)
        {
            _provider = SpriteManager.GetProvider(group, name);
            Name = name;
        }

        public bool HasSprite(string name)
        {
            return Name == name;
        }

        public int GetWidth(string name)
        {
            return Width;
        }

        public int GetHeight(string name)
        {
            return Height;
        }

        public void DrawSprite(IRenderContext context, string name, Box box, SpriteDrawingOptions options)
        {
            _provider.DrawSprite(context, Name, box, options);
        }
    }
}
