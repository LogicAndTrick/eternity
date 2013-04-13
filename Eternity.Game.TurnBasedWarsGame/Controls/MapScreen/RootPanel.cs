using Eternity.Controls;
using Eternity.Controls.Layouts;
using Eternity.Game.TurnBasedWarsGame.WarsGame;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class RootPanel : LayoutControl
    {
        public Battle Battle { get; private set; }

        public RootPanel(Battle battle) : base(new CardLayout())
        {
            Battle = battle;
        }

        public override void OnKeyDown(Input.EternityEvent e)
        {
            Battle.KeyDown(e);
        }
    }
}