using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Load
{
    /// <summary>
    /// Some units can be loaded with others (carrier, APC, cruiser, etc)
    /// </summary>
    public class LoadAction : IUnitAction
    {
        private readonly ContextState _state;

        public LoadAction(ContextState state)
        {
            _state = state;
        }

        public bool IsValidTile(Tile tile)
        {
            return tile.CanMoveTo && tile.Unit != null && tile.Unit.CanLoadWith(_state.Unit);
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new InvalidOperationException("The load action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile)
        {
            throw new InvalidOperationException("The load action is instant and cannot update the move set.");
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
            return "Load";
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            yield return new ContextState(UnitActionType.Load, _state.Unit, _state.Tile, this, new LoadActionRunner(_state.Tile.Unit, _state.Unit));
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