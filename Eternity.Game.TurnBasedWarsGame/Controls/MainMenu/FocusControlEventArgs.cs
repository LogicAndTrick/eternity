using System;
using Eternity.Controls;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class FocusControlEventArgs : EventArgs
    {
        public Control Control { get; set; }

        public FocusControlEventArgs(Control control)
        {
            Control = control;
        }
    }
}