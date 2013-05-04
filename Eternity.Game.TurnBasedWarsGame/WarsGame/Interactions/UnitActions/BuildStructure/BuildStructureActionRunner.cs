using System;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Structures;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.BuildStructure
{
    public class BuildStructureActionRunner : IUnitActionRunner
    {
        private readonly Unit _builder;
        private readonly Tile _tile;

        public BuildStructureActionRunner(Unit builder, Tile tile)
        {
            _builder = builder;
            _tile = tile;
        }

        public void Execute(Battle battle, GameBoard gameboard, Action<ExecutionState> callback)
        {
            if (_tile.Structure == null)
            {
                _tile.Structure = new Structure(_tile)
                                      {
                                          IsUnderConstruction = true
                                      };
            }
            var points = _tile.Structure.CapturePoints - _builder.HealthOutOfTen;
            if (points <= 0)
            {
                _tile.Type = _builder.GetBuildingType(_tile.Type);
                _tile.Structure.Capture(_builder.Army);
                gameboard.RevealFogOfWar(battle, _tile.Structure.Tile, _tile.Structure);
            }
            else
            {
                _tile.Structure.CapturePoints = points;
            }
            callback(ExecutionState.Empty);
        }
    }
}