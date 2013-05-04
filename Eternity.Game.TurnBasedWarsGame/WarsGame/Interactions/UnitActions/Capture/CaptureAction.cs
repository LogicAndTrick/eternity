using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Capture
{
    /// <summary>
    /// Infantry can capture buildings
    /// </summary>
    public class CaptureAction : IUnitAction
    {
        private readonly ContextState _state;

        public CaptureAction(ContextState state)
        {
            _state = state;
        }

        public bool IsValidTile(Tile tile)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new InvalidOperationException("The capture action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Battle battle, GameBoard gameboard, Tile tile)
        {
            throw new InvalidOperationException("The capture action is instant and cannot update the move set.");
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
            return "Capture";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Capture, _state.Unit, _state.Tile, this, new CaptureActionRunner(_state.Unit, _state.Tile.Structure));
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