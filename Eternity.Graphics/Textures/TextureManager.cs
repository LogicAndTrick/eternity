using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Resources;

namespace Eternity.Graphics.Textures
{
    public static class TextureManager
    {
        private class TextureWrapper
        {
            public ITexture Texture { get; private set; }
            public string Group { get; private set; }

            public TextureWrapper(ResourceDefinition def, IRenderContext context)
            {
                var rs = def.GetData("Resource");
                if (String.IsNullOrWhiteSpace(rs)) throw new Exception("Texture with no resource defined.");
                var resource = ResourceManager.GetResource(rs);
                if (resource == null) throw new Exception("Texture with missing resource.");
                Texture = context.CreateTextureFromFile(resource, def.GetData("Name"));
                Group = def.GetData("Group");
            }

            public TextureWrapper(ITexture texture, string grp)
            {
                Texture = texture;
                Group = grp;
            }

        }

        private static readonly List<TextureWrapper> Textures;

        static TextureManager()
        {
            Textures = new List<TextureWrapper>();
        }

        public static void RegisterTexture(ITexture texture, string group)
        {
            Textures.Add(new TextureWrapper(texture, group));
        }

        public static void RegisterResourceGroup(IRenderContext context, string group)
        {
            RegisterResources(context, ResourceManager.GetResourceDefinitionGroup(group));
        }

        public static void RegisterResources(IRenderContext context, IEnumerable<ResourceDefinition> definitions)
        {
            foreach (var rd in definitions)
            {
                if (Textures.Any(x => x.Group == rd.GetData("Group") && x.Texture.Name == rd.GetData("Name")))
                {
                    continue; // We already have that texture, go away
                }
                switch (rd.DefinitionType)
                {
                    case "Texture":
                        Textures.Add(new TextureWrapper(rd, context));
                        break;
                }
            }
        }

        public static void DeregisterTexture(ITexture texture)
        {
            Textures.RemoveAll(x => x.Texture == texture);
        }

        public static void DeregisterResourceGroup(string group)
        {
            Textures.RemoveAll(x => x.Group == group);
        }

        public static ITexture GetTexture(string group, string name)
        {
            var t = Textures.FirstOrDefault(x => x.Group == group && x.Texture.Name == name);
            return t == null ? null : t.Texture;
        }
    }
}
