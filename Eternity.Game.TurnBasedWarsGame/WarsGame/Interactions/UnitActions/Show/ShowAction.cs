using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Show
{
    /// <summary>
    /// Hidden units can show themselves (subs, stealth)
    /// </summary>
    public class ShowAction : IUnitAction
    {
        private readonly ContextState _state;

        public ShowAction(ContextState state)
        {
            _state = state;
        }

        public bool IsValidTile(Tile tile)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new InvalidOperationException("The show action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile)
        {
            throw new InvalidOperationException("The show action is instant and cannot update the move set.");
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
            return _state.Unit.UnitRules.ShowAction;
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Show, _state.Unit, _state.Tile, this, new ShowActionRunner(_state.Unit));
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