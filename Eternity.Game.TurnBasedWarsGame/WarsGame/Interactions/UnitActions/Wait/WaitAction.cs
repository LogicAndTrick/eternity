using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Wait
{
    /// <summary>
    /// Wait ends a turn without any further action.
    /// </summary>
    public class WaitAction : IUnitAction
    {
        private readonly ContextState _state;

        public WaitAction(ContextState state)
        {
            _state = state;
        }

        public bool IsValidTile(Tile tile)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles()
        {
            return new List<ValidTile>();
        }

        public void UpdateMoveSet(Tile tile)
        {
            //
        }

        public MoveSet GetMoveSet()
        {
            return null;
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Wait";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Wait, _state.Unit, _state.Tile, this, new WaitActionRunner());
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