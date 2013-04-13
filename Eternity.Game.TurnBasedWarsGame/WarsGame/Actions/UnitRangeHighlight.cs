using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions
{
    /// <summary>
    /// A secondary interaction set: showing a unit's attack range
    /// </summary>
    public class UnitRangeHighlight : ITileInteractionSet
    {
        public Unit Unit { get; private set; }

        private readonly Battle _battle;

        public UnitRangeHighlight(Unit unit)
        {
            Unit = unit;
            _battle = unit.Tile.Parent.Battle;
            var states = MoveSet.AllPossibleAttackPositions(Unit);
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
            Unit.Tile.Parent.Battle.GameBoard.DeselectUnit(Unit);
        }
    }
}
