using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    public class ContextState
    {
        public UnitActionType Type { get; private set; }
        public Unit Unit { get; private set; }
        public Tile Tile { get; private set; }
        public IUnitAction Action { get; private set; }
        public IUnitActionRunner ActionRunner { get; private set; }

        public ContextState(UnitActionType type, Unit unit, Tile tile, IUnitAction action, IUnitActionRunner actionRunner)
        {
            Type = type;
            Unit = unit;
            Tile = tile;
            Action = action;
            ActionRunner = actionRunner;
        }
    }
}