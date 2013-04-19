using System;
using System.Collections.Generic;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Move
{
    public class MoveActionGenerator : IUnitActionGenerator
    {
        public bool IsValidFor(ContextQueue queue)
        {
            return false; // Move should always be first
        }

        public IEnumerable<IUnitAction> GetActions(ContextQueue queue)
        {
            throw new InvalidOperationException("Move is not a valid generator"); // Move is never generated
        }
    }
}