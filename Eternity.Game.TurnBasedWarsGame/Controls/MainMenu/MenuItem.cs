using System.Drawing;
using Eternity.Controls;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;
using Eternity.Graphics.Textures;
using Eternity.Graphics.Sprites;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuItem : Control
    {
        private SpriteReference _campaignIcon;
        private bool _mouseIn;

        public override void OnSetUp(IRenderContext context)
        {
            _campaignIcon = new SpriteReference("MainMenu", "CampaignIcon");
        }

        public override void OnRender(IRenderContext context)
        {
            if (_mouseIn)
            {
                context.DisableTextures();
                context.StartQuads();
                context.SetColour(Color.LightGray);
                context.Point2(0, 0);
                context.Point2(0, Box.Height);
                context.Point2(Box.Width, Box.Height);
                context.Point2(Box.Width, 0);
                context.SetColour(Color.White);
                context.End();
                context.EnableTextures();
            }

            var oy = (Box.Height - _campaignIcon.Height) / 2;
            var ox = 5;

            SpriteManager.DrawSprite(context, "MainMenu", "CampaignIcon", new Box(ox, oy, _campaignIcon.Width, _campaignIcon.Height), null);
        }

        public override void OnMouseEnter(Input.EternityEvent e)
        {
            _mouseIn = true;
        }

        public override void OnMouseLeave(Input.EternityEvent e)
        {
            _mouseIn = false;
        }

        public override void OnMouseDown(Input.EternityEvent e)
        {
            //_mouseIn = !_mouseIn;
        }
    }
}
