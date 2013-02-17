using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles
{
    public class TilePath : List<Tile>
    {
        public Unit Unit { get; private set; }

        public TilePath(Unit unit)
        {
            Unit = unit;
        }
    }
}
