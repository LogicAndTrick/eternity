using System.Collections.Generic;
using System.Linq;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Armies;
using Eternity.Game.TurnBasedWarsGame.WarsGame.COs;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Rules;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Turns;
using Eternity.Input;
using Eternity.Resources;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame
{
    public class Battle
    {
        public Map Map { get; private set; }
        public List<Army> Armies { get; private set; }
        public Turn CurrentTurn { get; private set; }
        public int Day { get; private set; }
        public GameBoard GameBoard { get; set; }

        public TileInfoChromeControl TileInfoChromeControl { get; set; }

        private ITileInteraction _interaction;

        public Battle(ResourceDefinition mapDefinition)
        {
            RuleSet.SetCurrentRuleSet(mapDefinition.GetData("RuleSet"));
            Armies = new List<Army>();
            foreach (var army in mapDefinition.GetData("Armies").Split(' '))
            {
                Armies.Add(new Army(new CO(), army));
            }
            Map = new Map(this, mapDefinition);
            Day = 1;
            CurrentTurn = new Turn(this, 1, Armies.First());
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
            EndUnitAction();
            CurrentTurn.Army.Units.ForEach(x => x.HasMoved = false);
            CurrentTurn = CurrentTurn.CreateNextTurn();
            GameBoard.HideDialog();
            GameBoard.UpdateHealthOverlays();
            GameBoard.SetFogOfWar();
            NewTurnOverlay();
        }

        private void NewTurnOverlay()
        {
            var eo = new NewTurnEffectOverlay(CurrentTurn);
            GameBoard.AddOverlay(eo);
        }

        public void EndUnitAction()
        {
            if (_interaction == null) return;
            _interaction.Complete();
            _interaction = null;
            GameBoard.UpdateHealthOverlays();
        }

        public void TileMouseDown(EternityEvent e, Tile tile)
        {
            if (_interaction != null)
            {
                _interaction.TileMouseDown(e, tile);
            }
            else if (tile.Fog)
            {
                // Can't interact with fogged tiles
            }
            else if (tile.Unit != null)
            {
                if (e.Button == MouseButton.Left) _interaction = new UnitActionSet(tile.Unit);
                else if (e.Button == MouseButton.Right) _interaction = new UnitRangeHighlight(tile.Unit);
            }
            else if (e.Button == MouseButton.Right)
            {
                var point = GameBoard.GetTileControl(tile.Location).GetLocationInTree() - GameBoard.GetLocationInTree();
                ToggleMenu(point + new Point(e.X, e.Y));
            }
        }

        public void TileHovered(Tile tile)
        {
            if (TileInfoChromeControl != null) TileInfoChromeControl.SetTile(tile);
            if (_interaction != null) _interaction.TileHovered(tile);
        }

        public void TileMouseUp(EternityEvent e, Tile tile)
        {
            if (_interaction != null) _interaction.TileMouseUp(e, tile);
        }

        public void KeyDown(EternityEvent e)
        {
            if (e.Key == Key.Escape)
            {
                ToggleMenu(null);
            }
        }

        private void ToggleMenu(Point p)
        {
            if (GameBoard.HasDialog())
            {
                GameBoard.HideDialog();
            }
            else
            {
                var actions = new[] { MenuDialog.Action("End Turn", EndTurn) };
                var dialog = p == null ? GameBoard.ShowDialog(actions) : GameBoard.ShowDialog(new Box(p, Size.Zero), actions);
                dialog.CancelAction = GameBoard.HideDialog;
            }
        }
    }
}
