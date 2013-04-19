using System;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Wait
{
    public class WaitActionRunner : IUnitActionRunner
    {
        public void Execute(Action callback)
        {
            callback();
        }
    }
}