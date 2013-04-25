using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Fire;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.MoveAction
{
    /// <summary>
    /// Moves a unit, but could potentially move it nowhere. Might also include an attack.
    /// </summary>
    public class MoveAction : IUnitAction
    {
        private readonly MoveSet _set;
        private readonly ContextState _state;

        public MoveAction(ContextState state)
        {
            _state = state;
            _set = new MoveSet(state.Unit);
            UpdateMoveSet(state.Tile);
        }

        public bool IsValidTile(Tile tile)
        {
            return tile.CanAttack || tile.CanMoveTo;
        }

        public List<ValidTile> GetValidTiles()
        {
            var ms = MoveSet.AllPossibleMoves(_state.Unit, _state.Tile);
            return ms.Select(x => new ValidTile {MoveType = x.MoveType, Tile = x.MoveTile}).ToList();
        }

        public void UpdateMoveSet(Tile tile)
        {
            _set.TryMovePathTo(Move.CreateMove(tile, _state.Unit));
            if (_set.Unit.CanAttackIndirectly(true))
            {
                _state.Tile.Parent.Battle.GameBoard.SetRangeCursor(
                    _set.Last(x => x.MoveType == MoveType.Move).MoveTile,
                    _state.Unit.PrimaryWeapon.WeaponRules.MaxRange);
            }
        }

        public MoveSet GetMoveSet()
        {
            return _set;
        }

        public void ClearEffects()
        {
            _state.Tile.Parent.Battle.GameBoard.ClearRangeCursor();
        }

        public void Cancel()
        {

        }

        public string GetName()
        {
            return "Move";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            var move = _set.LastOrDefault(x => x.MoveType == MoveType.Move);
            var tile = move == null ? _state.Tile : move.MoveTile;
            var target = _set.LastOrDefault(x => x.MoveType == MoveType.Attack);

            yield return new ContextState(UnitActionType.Move, _state.Unit, tile, this, new MoveActionRunner(_set));

            if (target != null)
            {
                yield return new ContextState(UnitActionType.Fire, _state.Unit, tile, this, new FireActionRunner(_state.Unit, target.UnitToAttack));
            }
        }

        public bool IsInstantAction()
        {
            return false;
        }

        public bool IsCommittingAction()
        {
            return _set.Any() && _set.Last().MoveType == MoveType.Attack;
        }
    }
}