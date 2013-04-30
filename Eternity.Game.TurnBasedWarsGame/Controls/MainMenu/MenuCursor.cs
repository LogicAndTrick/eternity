using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics.Sprites;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuCursor : Control, IOverlayControl
    {
        private class MenuCursorCorner : SpriteControl
        {
            public MenuCursorCorner(string group, string name) : base(group, name)
            {
            }
        }

        private readonly MenuCursorCorner _nw;
        private readonly MenuCursorCorner _ne;
        private readonly MenuCursorCorner _sw;
        private readonly MenuCursorCorner _se;

        public MenuCursor()
        {
            _nw = new MenuCursorCorner("MainMenu", "MenuCursorNW");
            _ne = new MenuCursorCorner("MainMenu", "MenuCursorNE");
            _sw = new MenuCursorCorner("MainMenu", "MenuCursorSW");
            _se = new MenuCursorCorner("MainMenu", "MenuCursorSE");

            Add(_nw);
            Add(_ne);
            Add(_sw);
            Add(_se);

            AnimationCallback(0);
            StartAnimation();
        }

        public override void OnSizeChanged()
        {
            AnimationCallback(Math.Max(0, _loop - (WaitFrames - 3)));
        }

        private void StartAnimation()
        {
            _loop = 0;
            _add = 1;
            AddAnimation(new Animation<int>(0, 30, NextAnimationValue, AnimationCallback));
        }

        private int _loop;
        private int _add;

        private const int WaitFrames = 20;

        private int NextAnimationValue(int val)
        {
            _loop += _add;
            if (_loop <= 0 || _loop >= WaitFrames) _add = -_add;
            return Math.Max(0, _loop - (WaitFrames - 3));
        }

        private void AnimationCallback(int val)
        {
            Padding = Insets.All(val);
            var ib = InnerBox;
            _nw.ResizeSafe(new Box(ib.X, ib.Y, _nw.Sprite.Width, _nw.Sprite.Height));
            _ne.ResizeSafe(new Box(ib.X + ib.Width - _ne.Sprite.Width, ib.Y, _ne.Sprite.Width, _ne.Sprite.Height));
            _sw.ResizeSafe(new Box(ib.X, ib.Y + ib.Height - _sw.Sprite.Height, _sw.Sprite.Width, _sw.Sprite.Height));
            _se.ResizeSafe(new Box(ib.X + ib.Width - _se.Sprite.Width, ib.Y + ib.Height - _se.Sprite.Height, _se.Sprite.Width, _se.Sprite.Height));
        }

        public bool InterceptEvent(EternityEvent e)
        {
            return false;
        }

        public bool ListenEvent(EternityEvent e)
        {
            return false;
        }
    }
}
