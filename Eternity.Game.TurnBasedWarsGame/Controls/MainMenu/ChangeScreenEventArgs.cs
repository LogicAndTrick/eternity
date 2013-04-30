using System;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MainMenu
{
    public class ChangeScreenEventArgs : EventArgs
    {
        public MenuScreen Screen { get; set; }

        public ChangeScreenEventArgs(MenuScreen screen)
        {
            Screen = screen;
        }
    }
}