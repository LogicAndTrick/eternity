using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.COs;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Turns;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Input;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame
{
    public class Battle
    {
        public Map Map { get; private set; }
        public List<Army> Armies { get; private set; }
        public int Turn { get; private set; }
        public int Day { get; private set; }
        public GameBoard GameBoard { get; set; }

        private ITileInteractionSet _interaction;

        public Battle(ResourceDefinition mapDefinition)
        {
            RuleSet.SetCurrentRuleSet(mapDefinition.GetData("RuleSet"));
            Armies = new List<Army>();
            foreach (var army in mapDefinition.GetData("Armies").Split(' '))
            {
                Armies.Add(new Army(new CO(), army));
            }
            Map = new Map(this, mapDefinition);
            Turn = 0;
            Day = 1;
            _interaction = null;
        }

        public Army GetArmy(string army)
        {
            return Armies.FirstOrDefault(x => x.ArmyRules.Name == army);
        }

        public bool IsGameOver()
        {
            // An army has lost if:
            // it has created units, but now they are all dead
            if (Armies.Count(x => !x.Units.Any() && x.DeadUnits.Any()) < 2) return true;
            // the map is using HQs, and the army has no HQ
            return false;
        }

        public void EndTurn()
        {
            
        }

        public void EndUnitAction()
        {
            if (_interaction == null) return;
            _interaction.Complete();
            _interaction = null;
        }

        public void TileMouseDown(EternityEvent e, Tile tile)
        {
            if (_interaction != null)
            {
                _interaction.TileMouseDown(e, tile);
            }
            else if (tile.Unit != null)
            {
                if (e.Button == MouseButton.Left)_interaction = new UnitActionSet(tile.Unit);
                else if (e.Button == MouseButton.Right) _interaction = new UnitRangeHighlight(tile.Unit);
            }
        }

        public void TileHovered(Tile tile)
        {
            if (_interaction != null) _interaction.TileHovered(tile);
        }

        public void TileMouseUp(EternityEvent e, Tile tile)
        {
            if (_interaction != null) _interaction.TileMouseUp(e, tile);
        }
    }
}
