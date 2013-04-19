using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Fire;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Join;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Load;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Move;
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
        private static readonly List<IUnitActionGenerator> RegisteredActions;

        static UnitActionSet()
        {
            RegisteredActions = new List<IUnitActionGenerator>
                                    {
                                        new FireActionGenerator(),
                                        new JoinActionGenerator(),
                                        new LoadActionGenerator(),
                                        //new UnloadGenerator(),
                                        //new BuildUnitGenerator(),
                                        //new CaptureGenerator(),
                                        //new ResupplyGenerator(),
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
 

        public Unit Unit { get; private set; }
        private ContextQueue ContextQueue { get; set; }

        private readonly Battle _battle;
        private IUnitAction _currentAction;
        private bool _committing;

        private readonly bool _disabled;

        public UnitActionSet(Unit unit)
        {
            Unit = unit;
            _battle = unit.Tile.Parent.Battle;
            _committing = false;
            ContextQueue = new ContextQueue();

            var noAction = new NoAction();
            var cs = new ContextState(UnitActionType.None, Unit, Unit.Tile, noAction, noAction);
            ContextQueue.Enqueue(cs);

            _battle.GameBoard.SelectUnit(Unit);

            SelectAction(new MoveAction(ContextQueue.Last()));

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
                _currentAction.UpdateMoveSet(tile);
                _battle.GameBoard.CalculateArrowOverlays(_currentAction.GetMoveSet());
            }
        }

        public void TileMouseDown(EternityEvent e, Tile tile)
        {
            if (_committing || _disabled) return;
            if (_currentAction != null)
            {
                if (e.Button == MouseButton.Left && _currentAction.IsValidTile(tile)) ActionConfirmed();
                else Cancel();
            }
        }

        private void ActionConfirmed()
        {
            if (_currentAction.IsCommittingAction())
            {
                // Actions.Add(_currentAction);
                _currentAction.CreateContextStates().ToList().ForEach(ContextQueue.Enqueue);
                Commit();
            }
            else if (RegisteredActions.Any(x => x.IsValidFor(ContextQueue)))
            {
                // Actions.Add(_currentAction);
                _currentAction.CreateContextStates().ToList().ForEach(ContextQueue.Enqueue);
                ShowMenu();
            }
            else
            {
                Cancel();
            }
        }

        private void ShowMenu()
        {
            var valid = RegisteredActions
                .Where(x => x.IsValidFor(ContextQueue))
                .SelectMany(x => x.GetActions(ContextQueue))
                .ToList();
            if (!valid.Any())
            {
                Cancel(); 
                return;
            }
            var target = GetTarget();
            var dialog = _battle.GameBoard.ShowDialog(
                target,
                valid.Select(x => MenuDialog.Action(x.GetName(), () => SelectAction(x))).ToArray());
            dialog.CancelAction = Cancel;
        }

        private Tile GetTarget()
        {
            return ContextQueue.Select(x => x.Tile).LastOrDefault() ?? Unit.Tile;
        }

        private void SelectAction(IUnitAction action)
        {
            _battle.GameBoard.HideDialog();
            _currentAction = action;
            Unit.Tile.Parent.Tiles.ForEach(x => x.CanAttack = x.CanMoveTo = false);
            if (action.IsInstantAction())
            {
                ActionConfirmed();
                return;
            }
            var ms = _currentAction.GetValidTiles();
            ms.ForEach(x =>
                           {
                               x.Tile.CanMoveTo = x.MoveType == MoveType.Move;
                               x.Tile.CanAttack = x.MoveType == MoveType.Attack;
                           });
            _battle.GameBoard.UpdateTileHighlights();
            _battle.GameBoard.CalculateArrowOverlays(_currentAction.GetMoveSet());
        }

        private void Cancel()
        {
            if (ContextQueue.All(x => x.Type == UnitActionType.None))
            {
                _battle.EndUnitAction();
            }
            else
            {
                _currentAction.Cancel();
                var act = ContextQueue.Pop();
                SelectAction(act.Action);
            }
        }

        private void Commit()
        {
            _committing = true;
            _battle.GameBoard.DeselectUnit(Unit);
            CommitCallback();
        }

        private void CommitCallback()
        {
            _battle.GameBoard.UpdateHealthOverlays();
            if (ContextQueue.Any())
            {
                var state = ContextQueue.Dequeue();
                state.ActionRunner.Execute(Commit);
            }
            else
            {
                Unit.HasMoved = true;
                _battle.EndUnitAction();
            }
        }

        public void Complete()
        {
            _battle.Map.Tiles.ForEach(x => x.CanAttack = x.CanMoveTo = false);
            _battle.GameBoard.DeselectUnit(Unit);
        }
    }
}
