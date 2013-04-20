using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Structures;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Capture
{
    public class CaptureActionRunner : IUnitActionRunner
    {
        private readonly Unit _capturer;
        private readonly Structure _structure;

        public CaptureActionRunner(Unit capturer, Structure structure)
        {
            _capturer = capturer;
            _structure = structure;
        }

        public void Execute(Action callback)
        {
            var points = _structure.CapturePoints - _capturer.HealthOutOfTen;
            if (points <= 0) _structure.Capture(_capturer.Army);
            else _structure.CapturePoints = points;
            callback();
        }
    }
}