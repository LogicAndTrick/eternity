﻿using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.BuildUnit;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Capture;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Fire;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Join;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Load;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Resupply;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Unload;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Wait;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions
{
    /// <summary>
    /// The main tile interaction set: moving a unit
    /// </summary>
    public class UnitActionSet : ITileInteraction
    {
        private static readonly List<IUnitActionGenerator> ActionGenerators;

        static UnitActionSet()
        {
            ActionGenerators = new List<IUnitActionGenerator>
                                    {
                                        new FireActionGenerator(),
                                        new JoinActionGenerator(),
                                        new LoadActionGenerator(),
                                        new UnloadActionGenerator(),
                                        new BuildUnitActionGenerator(),
                                        new CaptureActionGenerator(),
                                        new ResupplyActionGenerator(),
                                        new WaitActionGenerator()

                                        // TODO

                                        // Dive (sub)
                                        // Rise (sub)

                                        // AW 4
                                        // Takeoff (carrier w/ planes)
                                        // CO (on a base)
                                        // Build (APC)

                                        // AW 2/3/4
                                        // Launch (silo)
                                        
                                        // AW3
                                        // Repair (black boat)
                                        // Explode (black bomb)
                                        // Hide (stealth)
                                        // Appear (stealth)
                                    };
        }

        private readonly Battle _battle;
        private readonly Unit _unit;
        private readonly ContextQueue _contextQueue;

        private IUnitAction _currentAction;
        private bool _committing;

        private readonly bool _disabled;

        public UnitActionSet(Unit unit)
        {
            _unit = unit;
            _battle = unit.Tile.Parent.Battle;
            _committing = false;
            _contextQueue = new ContextQueue();

            // Push the base state into the queue
            var noAction = new NoAction();
            var cs = new ContextState(UnitActionType.None, _unit, _unit.Tile, noAction, noAction);
            _contextQueue.Enqueue(cs);

            _battle.GameBoard.SelectUnit(_unit);

            // Always start with the move action
            SelectAction(new MoveAction.MoveAction(_contextQueue.Last()));

            // Allow the UAS on moved units, but don't allow any movements if it's disabled
            _disabled = unit.HasMoved || unit.Army != _battle.CurrentTurn.Army;
        }

        public void TileMouseUp(EternityEvent e, Tile tile)
        {
            if (_disabled) _battle.EndUnitAction();
        }

        public void TileHovered(Tile tile)
        {
            if (_committing || _disabled) return;
            if (_currentAction != null && _currentAction.IsValidTile(tile))
            {
                // Update the current action
                _currentAction.UpdateMoveSet(tile);
                CalculateArrowOverlays();
            }
        }

        public void TileMouseDown(EternityEvent e, Tile tile)
        {
            if (_committing || _disabled) return;
            if (_currentAction != null)
            {
                // Left click = confirm, Right click = cancel
                if (e.Button == MouseButton.Left && _currentAction.IsValidTile(tile)) ActionConfirmed();
                else Cancel();
            }
        }

        private void ActionConfirmed()
        {
            // Queue up all the context states
            var actions = _currentAction.CreateContextStates().ToList();
            actions.ForEach(_contextQueue.Enqueue);

            if (_currentAction.IsCommittingAction())
            {
                // If this action commits, run the action
                Commit();
            }
            else if (ActionGenerators.Any(x => x.IsValidFor(_contextQueue)))
            {
                // If it doesn't commit and actions are available, show the actions menu
                ShowMenu();
            }
            else
            {
                // If it doesn't commit and no actions are available, remove the context states and cancel
                actions.ForEach(x => _contextQueue.Remove(x));
                Cancel();
            }
        }

        private void ShowMenu()
        {
            // Grab the valid actions
            var valid = ActionGenerators
                .Where(x => x.IsValidFor(_contextQueue))
                .SelectMany(x => x.GetActions(_contextQueue))
                .ToList();
            if (!valid.Any())
            {
                // If no actions were found, cancel
                Cancel(); 
                return;
            }
            // Otherwise, show the actions menu
            var target = GetTarget();
            var dialog = _battle.GameBoard.ShowDialog(
                target,
                valid.Select(x => MenuDialog.Action(x.GetName(), () => SelectAction(x))).ToArray());
            dialog.CancelAction = Cancel;
        }

        private Tile GetTarget()
        {
            // The target is the last tile of the context queue.
            return _contextQueue.Select(x => x.Tile).LastOrDefault() ?? _unit.Tile;
        }

        private void CalculateArrowOverlays()
        {
            // Get all the move sets of the previous actions...
            var moves = _contextQueue.Select(x => x.Action.GetMoveSet())
                .Where(x => x != null)
                .SelectMany(x => x)
                .ToList();

            // ...plus the move set of the current action...
            if (_currentAction != null)
            {
                var ms = _currentAction.GetMoveSet();
                if (ms != null) moves.AddRange(ms);
            }

            // ...and render them all on the gameboard.
            var set = new MoveSet(_unit, moves);
            _battle.GameBoard.CalculateArrowOverlays(set.Any() ? set : null);
        }

        private void SelectAction(IUnitAction action)
        {
            // An action has been chosen
            _battle.GameBoard.HideDialog();
            _currentAction = action;
            _unit.Tile.Parent.Tiles.ForEach(x => x.CanAttack = x.CanMoveTo = false);

            // Instant actions don't allow selection of a tile
            if (action.IsInstantAction())
            {
                ActionConfirmed();
                return;
            }

            // Non-instant actions let you select a tile
            var ms = _currentAction.GetValidTiles();
            ms.ForEach(x =>
                           {
                               x.Tile.CanMoveTo = x.MoveType == MoveType.Move;
                               x.Tile.CanAttack = x.MoveType == MoveType.Attack;
                           });
            _battle.GameBoard.UpdateTileHighlights();
            CalculateArrowOverlays();
        }

        private void Cancel()
        {
            if (_contextQueue.All(x => x.Type == UnitActionType.None))
            {
                // If the only remaining context is the base state, cancel the entire action
                _battle.EndUnitAction();
            }
            else
            {
                // Otherwise, pop the context queue of all states with the last action and carry on
                _currentAction.Cancel();
                var act = _contextQueue.Pop().Action;
                while (_contextQueue.Last().Action == act) _contextQueue.Pop();
                SelectAction(act);
            }
        }

        private void Commit()
        {
            // Start committing the action
            _committing = true;
            _battle.GameBoard.DeselectUnit(_unit);
            CommitCallback();
        }

        private void CommitCallback()
        {
            // Each action might change the unit overlays, so update each time
            _battle.GameBoard.UpdateHealthOverlays();

            if (_contextQueue.Any())
            {
                // While the queue has items, run each action
                var state = _contextQueue.Dequeue();
                state.ActionRunner.Execute(CommitCallback); // Recursively use this method as the callback
            }
            else
            {
                // The queue is empty, we're done here.
                _unit.HasMoved = true;
                _battle.EndUnitAction();
            }
        }

        public void Complete()
        {
            // The unit action is over and we need to clean up any mess we made
            _battle.Map.Tiles.ForEach(x => x.CanAttack = x.CanMoveTo = false);
            _battle.GameBoard.DeselectUnit(_unit);
        }
    }
}
