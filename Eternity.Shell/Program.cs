using System;

namespace Eternity.Shell
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Type t;
            // t = typeof(Game.IGameMode);
            // t = typeof(Game.TurnBasedWarsGame.TurnBasedWarsMode);

            var ew = new EternityWindow();
            ew.Run(200, 60);
        }
    }
}
