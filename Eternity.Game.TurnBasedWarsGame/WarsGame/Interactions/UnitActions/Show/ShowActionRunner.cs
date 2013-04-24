using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Show
{
    public class ShowActionRunner: IUnitActionRunner
    {
        private readonly Unit _unit;

        public ShowActionRunner(Unit unit)
        {
            _unit = unit;
        }

        public void Execute(Action<ExecutionState> callback)
        {
            _unit.IsHidden = false;
            // Todo animations
            callback(ExecutionState.Empty);
        }
    }
}