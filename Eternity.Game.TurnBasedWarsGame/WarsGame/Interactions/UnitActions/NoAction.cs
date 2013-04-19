using System;
using System.Collections.Generic;
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

        public void UpdateMoveSet(Tile tile)
        {
            throw new NotImplementedException();
        }

        public MoveSet GetMoveSet()
        {
            throw new NotImplementedException();
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

        public void Execute(Action callback)
        {
            callback();
        }
    }
}