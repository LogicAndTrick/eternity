using System;
using Eternity.DataStructures.Primitives;
using Eternity.Resources;

namespace Eternity.Graphics.Sprites
{
    public class SpriteAnimated : ISpriteProvider
    {
        public enum AnimationStyle
        {
            Loop,
            Reverse
        }
        public string Group { get; private set; }
        public int FrameSpeed { get; private set; }
        private readonly string[] _sequence;
        private readonly string _name;
        private int _currentFrame;
        private SpriteReference _currentSprite;
        private AnimationStyle _style;
        private bool _reversing;

        public SpriteAnimated(ResourceDefinition def, IRenderContext context)
        {
            if (!Enum.TryParse(def.GetData("AnimationStyle"), out _style)) _style = AnimationStyle.Loop;
            _name = def.GetData("Name");
            Group = def.GetData("Group");
            _sequence = def.GetData("Sequence").Split(' ');
            FrameSpeed = int.Parse(def.GetData("FrameSpeed"));
            _currentFrame = 0;
            _currentSprite = new SpriteReference(Group, _sequence[_currentFrame]);
            _reversing = false;
        }

        public SpriteAnimated(string[] sequence, string name, string group, int frameSpeed, AnimationStyle style)
        {
            _sequence = sequence;
            _name = name;
            FrameSpeed = frameSpeed;
            Group = group;
            _currentFrame = 0;
            _currentSprite = new SpriteReference(Group, _sequence[_currentFrame]);
            _style = style;
            _reversing = false;
        }

        public void NextFrame()
        {
            if (_style == AnimationStyle.Loop)
            {
                _currentFrame = (_currentFrame + 1) % _sequence.Length;
            }
            else if (_style == AnimationStyle.Reverse)
            {
                var nextFrame = _reversing ? _currentFrame - 1 : _currentFrame + 1;
                if (nextFrame < 0)
                {
                    _reversing = false;
                    _currentFrame += 1;
                }
                else if (nextFrame >= _sequence.Length)
                {
                    _reversing = true;
                    _currentFrame -= 1;
                }
                else
                {
                    _currentFrame = nextFrame;
                }
            }
            _currentSprite = new SpriteReference(Group, _sequence[_currentFrame]);
        }

        public bool HasSprite(string name)
        {
            return _name == name;
        }

        public int GetWidth(string name)
        {
            return _currentSprite.Width;
        }

        public int GetHeight(string name)
        {
            return _currentSprite.Height;
        }

        public void DrawSprite(IRenderContext context, string name, Box box, SpriteDrawingOptions options)
        {
            _currentSprite.DrawSprite(context, name, box, options );
        }
    }
}
