using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Actions.Actions
{
    /// <summary>
    /// The most important unit action. Always executes on every turn, even if the unit's tile isn't changing.
    /// The move also executes any attack, join, or load action as these all involve moving the unit.
    /// </summary>
    public class MoveAction : IUnitAction, IUnitActionGenerator
    {
        public UnitActionType ActionType
        {
            get { return UnitActionType.Move; }
        }

        public void Execute(UnitActionSet set, Action callback)
        {
            var attackMove = set.CurrentMoveSet.FirstOrDefault(x => x.MoveType == MoveType.Attack);
            var target = set.CurrentMoveSet.LastOrDefault(x => x.MoveType == MoveType.Move)
                         ?? Move.CreateMove(set.Unit.Tile, set.Unit);
            var startTile = set.Unit.Tile;
            var endTile = target.MoveTile;
            var board = set.Unit.Tile.Parent.Battle.GameBoard;
            if (startTile != endTile && startTile.Structure != null) startTile.Structure.ResetCapturePoints();
            board.AnimatePath(set.Unit, set.CurrentMoveSet.GetMovementMoves(),
                              () =>
                                  {
                                      var etUnit = endTile.Unit;
                                      startTile.Unit = null;
                                      endTile.Unit = set.Unit;
                                      if (attackMove != null)
                                      {
                                          // Attack
                                          attackMove.UnitToMove.AttackUnit(attackMove.UnitToAttack);
                                      }
                                      else if (set.Unit.CanJoinWith(etUnit))
                                      {
                                          // Join
                                          set.Unit.JoinWith(etUnit);
                                      }
                                      else if (etUnit != null && etUnit.CanLoadWith(set.Unit))
                                      {
                                          // Load
                                          etUnit.LoadWith(set.Unit);
                                          endTile.Unit = etUnit;
                                      }
                                      callback();
                                  });
        }

        public bool IsValidFor(UnitActionSet set)
        {
            return false; // Move should always be first
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            throw new InvalidOperationException("Move is not a valid generator"); // Move is never generated
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return tile.CanAttack || tile.CanMoveTo;
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            var ms = MoveSet.AllPossibleMoves(set.Unit, set.Unit.Tile);
            return ms.Select(x => new ValidTile {MoveType = x.MoveType, Tile = x.MoveTile}).ToList();
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            set.CurrentMoveSet.TryMovePathTo(Move.CreateMove(tile, set.Unit));
        }

        public void Cancel(UnitActionSet set)
        {
            // 
        }

        public string GetName()
        {
            return "Move";
        }

        public bool IsInstantAction(UnitActionSet set)
        {
            return false;
        }

        public bool IsCommittingAction(UnitActionSet set)
        {
            return set.CurrentMoveSet.Any() && set.CurrentMoveSet.Last().MoveType == MoveType.Attack;
        }
    }
}