using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Hide
{
    /// <summary>
    /// Some units can hide themselves (subs, stealth)
    /// </summary>
    public class HideAction : IUnitAction
    {
        private readonly ContextState _state;

        public HideAction(ContextState state)
        {
            _state = state;
        }

        public bool IsValidTile(Tile tile)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new InvalidOperationException("The hide action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Battle battle, GameBoard gameboard, Tile tile)
        {
            throw new InvalidOperationException("The hide action is instant and cannot update the move set.");
        }

        public MoveSet GetMoveSet()
        {
            return null;
        }

        public void ClearEffects(Battle battle, GameBoard gameboard)
        {
            
        }

        public void Cancel()
        {
            // 
        }

        public string GetName()
        {
            return _state.Unit.UnitRules.HideAction;
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Hide, _state.Unit, _state.Tile, this, new HideActionRunner(_state.Unit));
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