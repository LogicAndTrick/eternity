using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls.Animations;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;
using Eternity.Input;
using Eternity.Resources;

namespace Eternity.Controls
{
    public static class SpritePool
    {
        private static readonly Dictionary<string, ISpriteProvider> Providers;
        private static readonly AnimationQueue Animations;

        static SpritePool()
        {
            Providers = new Dictionary<string, ISpriteProvider>();
            Animations = new AnimationQueue();
        }

        private static void AddProvider(IRenderContext context, string group, string name)
        {
            var resource = ResourceManager.GetResourceDefinitions(new Dictionary<string, string>
                                                                   {
                                                                       {"Group", group},
                                                                       {"Name", name}
                                                                   }).FirstOrDefault();
            if (resource == null) return;

            var key = group + "___" + name;

            if (resource.DefinitionType == "AnimatedSprite")
            {
                var sprite = new SpriteAnimated(resource, context);
                Animations.AddAnimation(new Animation<SpriteAnimated>(sprite, sprite.FrameSpeed, x => x, x => x.NextFrame()));
                Providers.Add(key, sprite);
            }
            else
            {
                var sprite = new SpriteReference(group, name);
                Providers.Add(key, sprite);
            }
        }

        public static void Update(FrameInfo info, IInputState state)
        {
            Animations.Update(info, state);
            Animations.RemoveCompletedAnimations();
        }

        public static void DrawSprite(IRenderContext context, string group, string name, Box box, SpriteDrawingOptions options)
        {
            var key = group + "___" + name;
            if (!Providers.ContainsKey(key)) AddProvider(context, group, name);
            if (Providers.ContainsKey(key)) Providers[key].DrawSprite(context, name, box, options);
        }

        public static void DrawSprite(IRenderContext context, SpriteReference sprite, Box box, SpriteDrawingOptions options)
        {
            var key = sprite.Group + "___" + sprite.Name;
            if (!Providers.ContainsKey(key)) AddProvider(context, sprite.Group, sprite.Name);
            if (Providers.ContainsKey(key)) Providers[key].DrawSprite(context, sprite.Name, box, options);
        }
    }
}
