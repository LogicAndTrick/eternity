using Eternity.Game.TurnBasedWarsGame.WarsGame.Structures;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns.Actions
{
    public enum UnitActionType
    {
        Cancel, Move, Wait, Fire, Join, Capture,
        Load, Unload, Resupply, Repair,
        Launch, Explode, Dive, Rise, Hide,
        Appear, CO, TakeOff, Build
    }
}