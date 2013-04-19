using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions.Actions
{
    /// <summary>
    /// Units attack other units. The fire is often combined into the move and this action isn't used.
    /// </summary>
    public class Fire : IUnitAction
    {
        public UnitActionType ActionType { get { return UnitActionType.Fire; } }

        public void Execute(UnitActionSet set, Action callback)
        {
            // The move action ALWAYS executes (even if moved zero tiles), and 
            // the move action takes care of any attacks as well.
            callback();
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return tile.CanAttack;
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            var tiles = set.CurrentMoveSet.GetPossibleAttackMoves();
            return tiles.Select(x => new ValidTile {MoveType = MoveType.Attack, Tile = x.MoveTile}).ToList();
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            set.CurrentMoveSet.RemoveAll(x => x.MoveType == MoveType.Attack);
            set.CurrentMoveSet.TryMovePathTo(Move.CreateAttack(tile, set.Unit));
        }

        public void Cancel(UnitActionSet set)
        {
            // 
        }

        public string GetName()
        {
            return "Fire";
        }

        public bool IsInstantAction(UnitActionSet set)
        {
            return false;
        }

        public bool IsCommittingAction(UnitActionSet set)
        {
            return true;
        }
    }

    public class FireGenerator : IUnitActionGenerator
    {
        public UnitActionType ActionType { get { return UnitActionType.Fire; } }

        public bool IsValidFor(UnitActionSet set)
        {
            var target = set.CurrentMoveSet.Any()
                             ? set.CurrentMoveSet.Last().MoveTile
                             : set.Unit.Tile;
            return set.CurrentMoveSet.All(x => x.MoveType != MoveType.Attack)
                && set.Unit.GetAttackableTiles(target, set.CurrentMoveSet.GetMovementCost()).Any();
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            yield return new Fire();
        }
    }
}