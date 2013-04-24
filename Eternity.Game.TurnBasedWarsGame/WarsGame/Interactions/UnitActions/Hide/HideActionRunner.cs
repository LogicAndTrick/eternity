using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Hide
{
    public class HideActionRunner: IUnitActionRunner
    {
        private readonly Unit _unit;

        public HideActionRunner(Unit unit)
        {
            _unit = unit;
        }

        public void Execute(Action<ExecutionState> callback)
        {
            _unit.IsHidden = true;
            // Todo animations
            callback(ExecutionState.Empty);
        }
    }
}