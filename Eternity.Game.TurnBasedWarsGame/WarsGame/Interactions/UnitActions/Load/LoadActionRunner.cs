using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Load
{
    public class LoadActionRunner : IUnitActionRunner
    {
        private readonly Unit _parent;
        private readonly Unit _child;

        public LoadActionRunner(Unit parent, Unit child)
        {
            _parent = parent;
            _child = child;
        }

        public void Execute(Action callback)
        {
            _parent.LoadWith(_child);
            callback();
        }
    }
}