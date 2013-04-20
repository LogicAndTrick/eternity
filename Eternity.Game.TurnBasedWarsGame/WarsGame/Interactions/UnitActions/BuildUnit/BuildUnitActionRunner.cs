using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.BuildUnit
{
    public class BuildUnitActionRunner: IUnitActionRunner
    {
        private readonly Unit _builder;
        private readonly UnitType _buildType;

        public BuildUnitActionRunner(Unit builder, UnitType buildType)
        {
            _builder = builder;
            _buildType = buildType;
        }

        public void Execute(Action callback)
        {
            var unit = new Unit(_builder.Army, _buildType, _builder.Army.GetUnitStyle(_buildType));
            _builder.UnitMaterial--;
            _builder.Army.Units.Add(unit);
            _builder.LoadWith(unit);
            callback();
        }
    }
}