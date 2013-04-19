using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Join
{
    public class JoinActionRunner : IUnitActionRunner
    {
        private readonly Unit _baseUnit;
        private readonly Unit _joinUnit;

        public JoinActionRunner(Unit baseUnit, Unit joinUnit)
        {
            _baseUnit = baseUnit;
            _joinUnit = joinUnit;
        }

        public void Execute(Action callback)
        {
            _baseUnit.JoinWith(_joinUnit);
            callback();
        }
    }
}