using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Messaging;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapEdit
{
    public class ArmyList : CenterPanel
    {
        public ArmyList() : base(new LayoutControl(new HorizontalStackLayout(5)))
        {
            foreach (var army in RuleSet.GetAllArmyRules())
            {
                var name = army.Name;
                var button = new Button(() => Mediator.Message(MapEditMessages.ChangeArmy, name), new CardLayout());
                button.Add(new SpriteControl("MapEdit", army.Name));
                Child.Add(button);
            }
            BackgroundColour = Color.FromArgb(128, Color.Black);
            Padding = Insets.All(5);
        }
    }
}