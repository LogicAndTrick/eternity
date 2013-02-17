using System.Collections.Generic;
using System.Linq;
using Eternity.DataStructures.Primitives;
using Eternity.Resources;

namespace Eternity.Graphics.Sprites
{
    public static class SpriteManager
    {
        private static readonly List<ISpriteProvider> Providers;

        static SpriteManager()
        {
            Providers = new List<ISpriteProvider>();
        }

        public static void RegisterProvider(ISpriteProvider provider)
        {
            Providers.Add(provider);
        }

        public static void RegisterResourceGroup(IRenderContext context, string group)
        {
            RegisterResources(context, ResourceManager.GetResourceDefinitionGroup(group));
        }

        public static void RegisterResources(IRenderContext context, IEnumerable<ResourceDefinition> definitions)
        {
            foreach (var rd in definitions)
            {
                switch (rd.DefinitionType)
                {
                    case "SpriteSheet":
                        RegisterProvider(new SpriteSheet(rd, context));
                        foreach (var cd in rd.ChildrenDefinitions)
                        {
                            switch (cd.DefinitionType)
                            {
                                case "AnimatedSprite":
                                    RegisterProvider(new SpriteAnimated(cd, context));
                                    break;
                            }
                        }
                        break;
                    case "Sprite":
                        RegisterProvider(new SpriteSingle(rd, context));
                        break;
                }
            }
        }

        public static void DeregisterProvider(ISpriteProvider provider)
        {
            Providers.Remove(provider);
        }

        public static void DeregisterResourceGroup(string group)
        {
            Providers.RemoveAll(x => x.Group == group);
        }

        public static ISpriteProvider GetProvider(string group, string name)
        {
            return Providers.FirstOrDefault(x => x.Group == group && x.HasSprite(name));
        }

        public static void DrawSprite(IRenderContext context, string group, string name, Box box, SpriteDrawingOptions options)
        {
            var p = GetProvider(group, name);
            if (p != null) p.DrawSprite(context, name, box, options);
        }
    }
}
