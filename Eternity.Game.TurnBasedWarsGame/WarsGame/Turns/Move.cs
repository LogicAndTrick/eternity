using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns
{
    public class Move
    {
        public static Move CreateMove(Tile moveTile, Unit moveUnit)
        {
            return new Move
            {
                MoveType = MoveType.Move,
                MoveTile = moveTile,
                UnitToMove = moveUnit,
                UnitToAttack = null
            };
        }

        public static Move CreateAttack(Tile moveTile, Unit moveUnit)
        {
            return new Move
            {
                MoveType = MoveType.Attack,
                MoveTile = moveTile,
                UnitToMove = moveUnit,
                UnitToAttack = moveTile.Unit
            };
        }

        public static Move CreateUnload(Tile moveTile, Tile loaderTile, Unit moveUnit)
        {
            return new Move
            {
                MoveType = MoveType.Unload,
                MoveTile = moveTile,
                UnitToMove = moveUnit,
                UnitToAttack = null,
                LoaderTile = loaderTile
            };
        }

        public MoveType MoveType { get; private set; }
        public Tile MoveTile { get; private set; }
        public Tile LoaderTile { get; private set; }
        public Unit UnitToMove { get; private set; }
        public Unit UnitToAttack { get; private set; }

        public List<Move> GetMovableTiles(int currentCost)
        {
            var list = new List<Move>();
            if (MoveType != MoveType.Attack)
            {
                list.AddRange(UnitToMove.GetMovableTiles(MoveTile, currentCost).Select(x => CreateMove(x, UnitToMove)));
                list.AddRange(UnitToMove.GetAttackableTiles(MoveTile, currentCost).Select(x => CreateAttack(x, UnitToMove)));
            }
            return list;
        }

        public int GetMovementCost()
        {
            return UnitToMove.GetMovementCost(MoveTile.Type);
        }

        public bool Equals(Move other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.MoveTile, MoveTile);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Move)) return false;
            return Equals((Move) obj);
        }

        public override int GetHashCode()
        {
            return (MoveTile != null ? MoveTile.GetHashCode() : 0);
        }

        public static bool operator ==(Move left, Move right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !Equals(left, right);
        }
    }
}
