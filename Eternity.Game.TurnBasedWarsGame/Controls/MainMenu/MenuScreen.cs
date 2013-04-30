using System;
using Eternity.Controls;
using Eternity.Controls.Layouts;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public abstract class MenuScreen : LayoutControl
    {
        public event EventHandler<ChangeScreenEventArgs> ChangeScreen;
        public event EventHandler<FocusControlEventArgs> FocusControl;

        public virtual void OnChangeScreen(MenuScreen screen)
        {
            if (ChangeScreen != null)
            {
                ChangeScreen(this, new ChangeScreenEventArgs(screen));
            }
        }

        public virtual void OnFocusControl(Control control)
        {
            if (FocusControl != null)
            {
                FocusControl(this, new FocusControlEventArgs(control));
            }
        }

        protected MenuScreen(ILayout layout) : base(layout)
        {
        }
    }
}