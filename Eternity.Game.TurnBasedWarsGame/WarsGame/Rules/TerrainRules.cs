using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Rules
{
    public class TerrainRules
    {
        public TileType TileType { get; private set; }
        public int Defense { get; private set; }
        public int Vision { get; private set; }
        public bool Capturable { get; private set; }
        public bool Destroyable { get; private set; }
        private readonly Dictionary<UnitMoveType, int> _moveValues;

        public TerrainRules(ResourceDefinition def)
        {
            TileType tt;
            Enum.TryParse(def.GetData("Name"), out tt);
            TileType = tt;

            Defense = int.Parse(def.GetData("Defense", "0"));
            Vision = int.Parse(def.GetData("Vision", "0"));
            Capturable = bool.Parse(def.GetData("Capturable", "False"));
            Destroyable = bool.Parse(def.GetData("Destroyable", "False"));

            _moveValues = new Dictionary<UnitMoveType, int>();
            foreach (var type in Enum.GetValues(typeof(UnitMoveType)).OfType<UnitMoveType>())
            {
                var val = def.GetData(type.ToString());
                var mv = String.IsNullOrWhiteSpace(val) ? 0 : int.Parse(val);
                _moveValues.Add(type, mv);
            }
        }

        public bool CanMove(UnitMoveType mt)
        {
            return _moveValues[mt] > 0;
        }

        public int GetMoveCost(UnitMoveType mt)
        {
            return _moveValues[mt];
        }
    }
}
