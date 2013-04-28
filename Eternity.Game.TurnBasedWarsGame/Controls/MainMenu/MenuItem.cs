using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuItem : LayoutControl
    {
        public MenuItem() : base(new HorizontalStackLayout(Insets.All(2), 2))
        {
            Add(new SpriteControl("MainMenu", "CampaignIcon"));
        }

        public override void OnMouseEnter(Input.EternityEvent e)
        {
            BackgroundColour = Color.LightGray;
        }

        public override void OnMouseLeave(Input.EternityEvent e)
        {
            BackgroundColour = Color.Empty;
        }
    }
}
