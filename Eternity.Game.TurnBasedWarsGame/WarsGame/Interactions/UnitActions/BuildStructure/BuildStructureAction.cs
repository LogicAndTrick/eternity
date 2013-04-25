using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.BuildStructure
{
    /// <summary>
    /// APCs can build temporary ports and airports
    /// </summary>
    public class BuildStructureAction : IUnitAction
    {
        private readonly ContextState _state;

        public BuildStructureAction(ContextState state)
        {
            _state = state;
        }

        public bool IsValidTile(Tile tile)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new InvalidOperationException("The build action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile)
        {
            throw new InvalidOperationException("The build action is instant and cannot update the move set.");
        }

        public MoveSet GetMoveSet()
        {
            return null;
        }

        public void ClearEffects()
        {
            
        }

        public void Cancel()
        {
            // 
        }

        public string GetName()
        {
            return "Build";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Capture, _state.Unit, _state.Tile, this, new BuildStructureActionRunner(_state.Unit, _state.Tile));
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