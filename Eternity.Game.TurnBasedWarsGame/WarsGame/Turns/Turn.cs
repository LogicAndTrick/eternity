using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns
{
    public class Turn
    {
        public Battle Battle { get; private set; }
        public int Day { get; private set; }
        public Army Army { get; private set; }

        public Turn(Battle battle, int day, Army army)
        {
            Battle = battle;
            Day = day;
            Army = army;
        }
        
        public Turn CreateNextTurn()
        {
            var idx = Battle.Armies.IndexOf(Army);
            var last = idx == Battle.Armies.Count - 1;
            var army = Battle.Armies[last ? 0 : idx + 1]; // Loop back to the first army if it's currently at the last one
            var day = last ? Day + 1 : Day; // Increment the day count if we've looped back to the first army
            return new Turn(Battle, day, army);
        }
    }
}
