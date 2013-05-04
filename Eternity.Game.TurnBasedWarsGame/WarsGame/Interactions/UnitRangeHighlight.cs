using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions
{
    /// <summary>
    /// A secondary interaction set: showing a unit's attack range
    /// </summary>
    public class UnitRangeHighlight : ITileInteraction
    {
        public Unit Unit { get; private set; }

        private readonly Battle _battle;

        public UnitRangeHighlight(Battle battle, Unit unit)
        {
            Unit = unit;
            _battle = battle;
            var states = MoveSet.AllPossibleAttackPositions(Unit, Unit.Tile, true);
            states.ForEach(x =>
                               {
                                   x.MoveTile.CanMoveTo = x.MoveType == MoveType.Move;
                                   x.MoveTile.CanAttack = x.MoveType == MoveType.Attack;
                               });
            _battle.GameBoard.UpdateTileHighlights();
            _battle.GameBoard.SelectUnit(Unit);
        }

        public void TileMouseUp(EternityEvent e, Tile tile)
        {
            _battle.EndUnitAction();
        }

        public void TileHovered(Tile tile)
        {
            //
        }

        public void TileMouseDown(EternityEvent e, Tile tile)
        {
            //
        }

        public void Complete()
        {
            Unit.Tile.Parent.Tiles.ForEach(x => x.CanAttack = x.CanMoveTo = false);
            _battle.GameBoard.DeselectUnit(Unit);
        }
    }
}
