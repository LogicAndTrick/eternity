using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Size = Eternity.DataStructures.Primitives.Size;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuControl : MenuScreen
    {
        public MenuControl() : base(new VerticalStackLayout(2))
        {
            Padding = Insets.All(2);
            BackgroundColour = Color.White;
            var header = new MenuHeaderControl();
            var list = new LayoutControl(new VerticalStackLayout(2));
            for (var i = 0; i < 5; i++)
            {
                list.Add(new MenuItem(this) { PreferredSize = new Size(250, 64) });
            }
            var scroll = new VerticalScrollPanel(new Size(250, 350), list);

            Add(header);
            Add(scroll);
        }
    }
}
