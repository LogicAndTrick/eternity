using System;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Graphics;
using Eternity.Graphics.Textures;

namespace Eternity.Game.TurnBasedWarsGame.Controls
{
    class ScrollingBackgroundImage : Control
    {
        public ITexture Texture { get; private set; }

        private readonly Animation<double> _animateX;
        private readonly Animation<double> _animateY;

        private double _backgroundOffsetX;
        private double _backgroundOffsetY;

        public ScrollingBackgroundImage(ITexture texture, int scrollSpeedX, int scrollSpeedY, double scrollAmountX, double scrollAmountY)
        {
            Texture = texture;
            _backgroundOffsetX = _backgroundOffsetY = 0;

            _animateX = new Animation<double>(0, scrollSpeedX, x => x + scrollAmountX);
            _animateY = new Animation<double>(0, scrollSpeedY, x => x + scrollAmountY);
            AddAnimation(_animateX, _animateY);
        }

        public override void OnAnimate()
        {
            _backgroundOffsetX = _animateX.CurrentValue;
            _backgroundOffsetY = _animateY.CurrentValue;
        }

        public override void OnRender(IRenderContext context)
        {
            context.SetTexture(Texture);
            context.StartQuads();

            var tx = Box.Width / (double) Texture.Width;
            var ty = Box.Height / (double) Texture.Height;

            var offx = Math.Round(_backgroundOffsetX) / Texture.Width;
            var offy = Math.Round(_backgroundOffsetY) / Texture.Height;
            tx += offx;
            ty += offy;

            context.TexturePoint(offx, offy);
            context.Point2(0, 0);

            context.TexturePoint(offx, ty);
            context.Point2(0, Box.Height);

            context.TexturePoint(tx, ty);
            context.Point2(Box.Width, Box.Height);

            context.TexturePoint(tx, offy);
            context.Point2(Box.Width, 0);

            context.End();
        }
    }
}
