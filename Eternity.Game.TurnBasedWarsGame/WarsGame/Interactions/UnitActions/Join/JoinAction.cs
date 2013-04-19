using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Join
{
    /// <summary>
    /// Join a unit with damaged unit of the same type.
    /// </summary>
    public class JoinAction : IUnitAction
    {
        private readonly ContextState _state;

        public JoinAction(ContextState state)
        {
            _state = state;
        }

        public bool IsValidTile(Tile tile)
        {
            return tile.CanMoveTo && _state.Unit.CanJoinWith(tile.Unit);
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new InvalidOperationException("The join action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile)
        {
            throw new InvalidOperationException("The join action is instant and cannot update the move set.");
        }

        public MoveSet GetMoveSet()
        {
            return null;
        }

        public void Cancel()
        {
            // 
        }

        public string GetName()
        {
            return "Join";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Join, _state.Unit, _state.Tile, this, new JoinActionRunner(_state.Tile.Unit, _state.Unit));
        }

        public bool IsInstantAction()
        {
            return true;
        }

        public bool IsCommittingAction()
        {
            return true;
        }
    }
}