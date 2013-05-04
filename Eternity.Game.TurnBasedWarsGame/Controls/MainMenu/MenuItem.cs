using System;
using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuItem : MenuElement
    {
        public MenuItem(MenuScreen screen) : base(screen, new HorizontalStackLayout(2))
        {
            Padding = Insets.All(2);
            Add(new SpriteControl("MainMenu", "CampaignIcon"));
            Focuses = true;
            ChangesScreen = true;
        }

        public override void OnMouseEnter(Input.EternityEvent e)
        {
            BackgroundColour = Color.LightGray;
            base.OnMouseEnter(e);
        }

        public override void OnMouseLeave(Input.EternityEvent e)
        {
            BackgroundColour = Color.Empty;
            base.OnMouseLeave(e);
        }

        protected override MenuScreen GetScreen()
        {
            return new MenuControl();
        }
    }
}
