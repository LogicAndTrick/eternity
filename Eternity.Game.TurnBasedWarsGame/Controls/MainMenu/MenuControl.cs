using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Controls;
using Eternity.Controls.Easings;
using Eternity.Controls.Effects;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuControl : LayoutControl
    {
        public MenuControl() : base(new VerticalStackLayout(Insets.All(2), 2))
        {
            BackgroundColour = Color.White;
            var header = new MenuHeaderControl();
            var list = new LayoutControl(new VerticalStackLayout(Insets.All(0), 2));
            for (var i = 0; i < 5; i++)
            {
                list.Add(new MenuItem { PreferredSize = new Size(250, 64) });
            }
            var scroll = new VerticalScrollPanel(new Size(250, 350), list);

            Add(header);
            Add(scroll);
        }

        public override void OnMouseDown(Input.EternityEvent e)
        {
            Parent.Parent.AddEffect(new CardSwipeEffect(Parent, null, 300, new BackEasing()));
            base.OnMouseDown(e);
        }
    }
}
