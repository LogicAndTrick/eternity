using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Fire
{
    public class FireActionRunner : IUnitActionRunner
    {
        private readonly Unit _attacker;
        private readonly Unit _defender;

        public FireActionRunner(Unit attacker, Unit defender)
        {
            _attacker = attacker;
            _defender = defender;
        }

        public void Execute(Action callback)
        {
            _attacker.AttackUnit(_defender);
            callback();
        }
    }
}