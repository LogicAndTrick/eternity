using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions
{
    public class NoAction : IUnitAction, IUnitActionRunner
    {
        public bool IsValidTile(Tile tile)
        {
            throw new NotImplementedException();
        }

        public List<ValidTile> GetValidTiles()
        {
            throw new NotImplementedException();
        }

        public void UpdateMoveSet(Battle battle, GameBoard gameboard, Tile tile)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IEnumerable<ContextState> CreateContextStates()
        {
            throw new NotImplementedException();
        }

        public bool IsInstantAction()
        {
            throw new NotImplementedException();
        }

        public bool IsCommittingAction()
        {
            throw new NotImplementedException();
        }

        public void Execute(Battle battle, GameBoard gameboard, Action<ExecutionState> callback)
        {
            callback(ExecutionState.Empty);
        }
    }
}