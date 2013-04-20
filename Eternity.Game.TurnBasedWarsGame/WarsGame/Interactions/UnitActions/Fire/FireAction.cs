using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Fire
{
    /// <summary>
    /// Units attack other units. The fire is often combined into the move and this action isn't used.
    /// </summary>
    public class FireAction : IUnitAction
    {
        private readonly ContextState _state;
        private Tile _attackTile;

        public FireAction(ContextState state)
        {
            _state = state;
            _attackTile = null;
        }

        public bool IsValidTile(Tile tile)
        {
            return tile.CanAttack;
        }

        public List<ValidTile> GetValidTiles()
        {
            var tiles = _state.Unit.GetAttackableTiles(_state.Tile, false);
            return tiles.Select(x => new ValidTile {MoveType = MoveType.Attack, Tile = x}).ToList();
        }

        public void UpdateMoveSet(Tile tile)
        {
            _attackTile = tile;
        }

        public MoveSet GetMoveSet()
        {
            return _attackTile == null
                       ? null
                       : new MoveSet(_state.Unit, new[] {Move.CreateAttack(_attackTile, _state.Unit)});
        }

        public void Cancel()
        {
            // 
        }

        public string GetName()
        {
            return "Fire";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Fire, _state.Unit, _state.Tile, this, new FireActionRunner(_state.Unit, _attackTile.Unit));
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