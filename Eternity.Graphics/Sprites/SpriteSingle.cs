using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics.Textures;
using Eternity.Resources;

namespace Eternity.Graphics.Sprites
{
    public class SpriteSingle : ISpriteProvider
    {
        private readonly ITexture _texture;
        private readonly string _group;
        private readonly string _name;

        public string Group { get { return _group; } }

        public SpriteSingle(ResourceDefinition def, IRenderContext context)
        {
            var rs = def.GetData("Resource");
            if (String.IsNullOrWhiteSpace(rs)) throw new Exception("Sprite with no resource defined.");
            var resource = ResourceManager.GetResource(rs);
            if (resource == null) throw new Exception("Sprite with missing resource.");
            _name = def.GetData("Name");
            _texture = context.CreateTextureFromFile(resource, _name);
            _group = def.GetData("Group");
        }

        public SpriteSingle(ITexture texture, string name, string group)
        {
            _texture = texture;
            _name = name;
            _group = group;
        }

        public bool HasSprite(string name)
        {
            return _name == name;
        }

        public int GetWidth(string name)
        {
            return _texture.Width;
        }

        public int GetHeight(string name)
        {
            return _texture.Height;
        }

        public void DrawSprite(IRenderContext context, string name, Box box, SpriteDrawingOptions options)
        {
            if (options == null) options = new SpriteDrawingOptions();

            var tex = options.CalculateTextureBox(new[] { 0d, 0d, 1d, 1d });
            var pos = options.CalculatePositionBox(box, _texture.Width, _texture.Height);

            context.SetTexture(_texture);
            context.SetColour(options.Colour);
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
            context.SetColour(Color.White);
        }
    }
}
