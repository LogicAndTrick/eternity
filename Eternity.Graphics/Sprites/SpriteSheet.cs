using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics.Textures;
using Eternity.Resources;

namespace Eternity.Graphics.Sprites
{
    public class SpriteSheet : ISpriteProvider
    {
        private readonly ITexture _texture;
        private readonly string _name;
        private readonly string _group;
        private readonly int _spriteWidth;
        private readonly int _spriteHeight;
        private readonly int _sheetWidth;
        private readonly int _sheetHeight;
        private readonly string[] _spriteOrder;
        private readonly double _texWidth;
        private readonly double _texHeight;

        public string Group { get { return _group; } }

        public SpriteSheet(ResourceDefinition def, IRenderContext context)
        {
            var rs = def.GetData("Resource");
            if (String.IsNullOrWhiteSpace(rs)) throw new Exception("SpriteSheet with no resource defined.");
            var resource = ResourceManager.GetResource(rs);
            if (resource == null) throw new Exception("SpriteSheet with missing resource.");
            _name = def.GetData("Name");
            _texture = context.CreateTextureFromFile(resource, _name);
            _spriteWidth = int.Parse(def.GetData("SpriteWidth"));
            _spriteHeight = int.Parse(def.GetData("SpriteHeight"));
            _sheetWidth = int.Parse(def.GetData("SheetWidth"));
            _sheetHeight = int.Parse(def.GetData("SheetHeight"));
            _spriteOrder = def.GetData("SpriteOrder").Split(' ');
            _texWidth = 1.0 / _sheetWidth;
            _texHeight = 1.0 / _sheetHeight;
            _group = def.GetData("Group");
        }

        public SpriteSheet(ITexture texture, int spriteWidth, int spriteHeight, int sheetWidth, int sheetHeight, string[] spriteOrder, string group)
        {
            _texture = texture;
            _spriteWidth = spriteWidth;
            _spriteHeight = spriteHeight;
            _sheetWidth = sheetWidth;
            _sheetHeight = sheetHeight;
            _spriteOrder = spriteOrder;
            _texWidth = 1.0 / _sheetWidth;
            _texHeight = 1.0 / _sheetHeight;
            _group = group;
        }

        public Point GetLocationOfSprite(string name)
        {
            var idx = Array.IndexOf(_spriteOrder, name);
            if (idx < 0) return null;
            if (idx > _sheetWidth * _sheetHeight) return null;
            var x = idx % _sheetWidth;
            var y = idx / _sheetWidth;
            return new Point(x, y);
        }

        public bool HasSprite(string name)
        {
            return _spriteOrder.Contains(name);
        }

        public int GetWidth(string name)
        {
            return _spriteWidth;
        }

        public int GetHeight(string name)
        {
            return _spriteHeight;
        }

        public void DrawSprite(IRenderContext context, string name, Box box, SpriteDrawingOptions options)
        {
            var pt = GetLocationOfSprite(name);
            if (pt == null) return;

            if (options == null) options = new SpriteDrawingOptions();

            var tx = pt.X * (_texWidth);
            var ty = pt.Y * (_texHeight);

            var tex = options.CalculateTextureBox(new[] {tx, ty, tx + _texWidth, ty + _texHeight});
            var pos = options.CalculatePositionBox(box, _spriteWidth, _spriteHeight);

            context.SetTexture(_texture);
            context.StartQuads();

            context.TexturePoint(tex[0], tex[1]);
            context.Point2(pos.X, pos.Y);

            context.TexturePoint(tex[0], tex[3]);
            context.Point2(pos.X, pos.Y + pos.Height);

            context.TexturePoint(tex[2], tex[3]);
            context.Point2(pos.X + pos.Width, pos.Y + pos.Height);

            context.TexturePoint(tex[2], tex[1]);
            context.Point2(pos.X + pos.Width, pos.Y);

            context.End();
        }
    }
}
