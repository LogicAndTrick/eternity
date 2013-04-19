using System.Collections.Generic;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    public class ContextQueue : List<ContextState>
    {
        public void Enqueue(ContextState state)
        {
            Add(state);
        }

        public ContextState Dequeue()
        {
            var first = this[0];
            RemoveAt(0);
            return first;
        }

        public ContextState Pop()
        {
            var last = this[Count - 1];
            RemoveAt(Count - 1);
            return last;
        }
    }
}