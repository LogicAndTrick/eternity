using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Easings;
using Eternity.Controls.Effects;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class MenuControl : LayoutControl
    {
        public MenuControl()
            : base(new VerticalStackLayout(new Insets(24, 4, 4, 4), 0))
        {
            for (var i = 0; i < 5; i++)
            {
                Add(new MenuItem());
            }
        }

        public override void OnMouseDown(Input.EternityEvent e)
        {
            Parent.Parent.AddEffect(new CardSwipeEffect(Parent, null, 300, new BackEasing()));
            //AddAnimationSequential(new Animation<int>(() => Box.Width, () => Box.Width + 100, 1000, new EasingOut(new BounceEasing()), ChangeWidth));
            base.OnMouseDown(e);
        }

        private void ChangeWidth(int newWidth)
        {
            var diff = (newWidth - Box.Width) / 2;
            Box = new Box(Box.X - diff, Box.Y, Box.Width + diff * 2, Box.Height);
            OnChildSizeChanged();
        }

        public override void OnRender(IRenderContext context)
        {
            var borderWidth = 2;
            var HeaderHeight = 20;

            context.DisableTextures();
            context.StartQuads();

            context.SetColour(Color.White);
            context.Point2(0, 0);
            context.Point2(Box.Width, 0);
            context.Point2(Box.Width, Box.Height);
            context.Point2(0, Box.Height);

            context.SetColour(Color.Red);
            context.Point2(borderWidth, borderWidth);
            context.Point2(Box.Width - borderWidth, borderWidth);
            context.Point2(Box.Width - borderWidth, Box.Height - borderWidth);
            context.Point2(borderWidth, Box.Height - borderWidth);

            borderWidth *= 2;

            context.SetColour(Color.White);
            context.Point2(borderWidth, borderWidth + HeaderHeight);
            context.Point2(Box.Width - borderWidth, borderWidth + HeaderHeight);
            context.Point2(Box.Width - borderWidth, Box.Height - borderWidth);
            context.Point2(borderWidth, Box.Height - borderWidth);

            context.SetColour(Color.White);
            context.End();
            context.EnableTextures();
        }
    }
}
