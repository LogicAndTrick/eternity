using System;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Resupply
{
    public class ResupplyActionRunner : IUnitActionRunner
    {
        private readonly Unit _supplier;

        public ResupplyActionRunner(Unit supplier)
        {
            _supplier = supplier;
        }

        public void Execute(Action<ExecutionState> callback)
        {
            var units = _supplier.Tile.GetAdjacentTiles()
                .Where(x => x != null && x.Unit != null && x.Unit.Army == _supplier.Army && x.Unit.CanBeResupplied())
                .Select(x => x.Unit)
                .ToList();
            units.ForEach(x => x.Resupply());
            var board = _supplier.Tile.Parent.Battle.GameBoard;
            board.AddEffect(new PopupEffect
                                {
                                    Board = board,
                                    Time = 500,
                                    TileSprites = units.Where(x => x.Tile != null).ToDictionary(x => x.Tile, x => "SupplyRight")
                                });
            board.Delay(500, () => callback(ExecutionState.Empty));
        }
    }
}