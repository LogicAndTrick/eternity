using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Fire;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.MoveAction;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.TakeOff
{
    public class TakeOffAction : IUnitAction
    {
        private readonly MoveSet _set;
        private readonly Unit _unitToTakeOff;
        private readonly ContextState _state;

        public TakeOffAction(ContextState state, Unit unitToTakeOff)
        {
            _state = state;
            _unitToTakeOff = unitToTakeOff;
            //_unitToTakeOff.Tile = state.Tile;
            _set = new MoveSet(unitToTakeOff, state.Tile);
            _set.TryMovePathTo(Move.CreateMove(state.Tile, unitToTakeOff));
        }

        public bool IsValidTile(Tile tile)
        {
            return tile != _state.Tile && (tile.CanAttack || tile.CanMoveTo);
        }

        public List<ValidTile> GetValidTiles()
        {
            var ms = MoveSet.AllPossibleMoves(_unitToTakeOff, _state.Tile);
            return ms.Select(x => new ValidTile { MoveType = x.MoveType, Tile = x.MoveTile }).ToList();
        }

        public void UpdateMoveSet(Battle battle, GameBoard gameboard, Tile tile)
        {
            _set.TryMovePathTo(Move.CreateMove(tile, _unitToTakeOff));
        }

        public MoveSet GetMoveSet()
        {
            return _set;
        }

        public void ClearEffects(Battle battle, GameBoard gameboard)
        {
            
        }

        public void Cancel()
        {
            _unitToTakeOff.Tile = null;
        }

        public string GetName()
        {
            return "Take Off";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            var move = _set.LastOrDefault(x => x.MoveType == MoveType.Move);
            var tile = move == null ? _state.Tile : move.MoveTile;
            var target = _set.LastOrDefault(x => x.MoveType == MoveType.Attack);

            yield return new ContextState(UnitActionType.TakeOff, _unitToTakeOff, _state.Tile, this, new TakeOffActionRunner(_state.Unit, _unitToTakeOff, tile));
            
            yield return new ContextState(UnitActionType.Move, _unitToTakeOff, tile, this, new MoveActionRunner(_set));

            if (target != null)
            {
                yield return new ContextState(UnitActionType.Fire, _unitToTakeOff, tile, this, new FireActionRunner(_unitToTakeOff, target.UnitToAttack));
            }
        }

        public bool IsInstantAction()
        {
            return false;
        }

        public bool IsCommittingAction()
        {
            return true;
        }
    }
}
