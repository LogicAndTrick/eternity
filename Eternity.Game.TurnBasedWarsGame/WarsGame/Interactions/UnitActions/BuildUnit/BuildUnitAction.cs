using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.BuildUnit
{
    /// <summary>
    /// Some units can build other units internally (carriers)
    /// </summary>
    public class BuildUnitAction : IUnitAction
    {
        private readonly ContextState _state;
        private readonly UnitType _buildType;

        public BuildUnitAction(ContextState state, UnitType buildType)
        {
            _state = state;
            _buildType = buildType;
        }

        public bool IsValidTile(Tile tile)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new InvalidOperationException("The build action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Battle battle, GameBoard gameboard, Tile tile)
        {
            throw new InvalidOperationException("The build action is instant and cannot update the move set.");
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
            return "Build";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Build, _state.Unit, _state.Tile, this, new BuildUnitActionRunner(_state.Unit, _buildType));
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