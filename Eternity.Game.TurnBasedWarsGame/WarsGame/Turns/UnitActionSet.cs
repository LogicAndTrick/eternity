using System.Collections.Generic;
using System.Linq;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Turns.Actions;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns
{
    /// <summary>
    /// The main tile interaction set: moving a unit
    /// </summary>
    public class UnitActionSet : ITileInteractionSet
    {
        private static readonly List<IUnitActionGenerator> RegisteredActions;

        static UnitActionSet()
        {
            RegisteredActions = new List<IUnitActionGenerator>
                                    {
                                        new Fire(),
                                        new Join(),
                                        new Load(),
                                        new Unload(),
                                        new BuildUnit(),
                                        new Capture(),
                                        new Resupply(),
                                        new Wait()

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
        private List<IUnitAction> Actions { get; set; }
        public MoveSet CurrentMoveSet { get; private set; }

        private readonly Battle _battle;
        private IUnitAction _currentAction;
        private bool _committing;

        public UnitActionSet(Unit unit)
        {
            Unit = unit;
            _battle = unit.Tile.Parent.Battle;
            _committing = false;
            CurrentMoveSet = new MoveSet(unit);
            Actions = new List<IUnitAction>();

            _battle.GameBoard.SelectUnit(Unit);

            SelectAction(new MoveAction());
        }

        public void TileMouseUp(EternityEvent e, Tile tile)
        {
            //if (_committing) return;
            //
        }

        public void TileHovered(Tile tile)
        {
            if (_committing) return;
            if (_currentAction != null && _currentAction.IsValidTile(tile, this))
            {
                _currentAction.UpdateMoveSet(tile, this);
                _battle.GameBoard.CalculateArrowOverlays(CurrentMoveSet);
            }
        }

        public void TileMouseDown(EternityEvent e, Tile tile)
        {
            if (_committing) return;
            if (_currentAction != null)
            {
                if (e.Button == MouseButton.Left && _currentAction.IsValidTile(tile, this)) ActionConfirmed();
                else Cancel();
            }
        }

        private void ActionConfirmed()
        {
            if (_currentAction.IsCommittingAction(this))
            {
                Actions.Add(_currentAction);
                Commit();
            }
            else if (RegisteredActions.Any(x => x.IsValidFor(this)))
            {
                Actions.Add(_currentAction);
                ShowMenu();
            }
            else
            {
                Cancel();
            }
        }

        private void ShowMenu()
        {
            var valid = RegisteredActions.Where(x => x.IsValidFor(this))
                .SelectMany(x => x.GetActions(this)).ToList();
            if (!valid.Any())
            {
                Cancel(); 
                return;
            }
            var target = GetTarget();
            _battle.GameBoard.ShowDialog(
                target.MoveTile,
                valid.Select(x => ActionDialog.Action(x.GetName(), () => SelectAction(x))).ToArray());
        }

        private Move GetTarget()
        {
            var ms = CurrentMoveSet;
            return ms.LastOrDefault(x => x.MoveType == MoveType.Move)
                         ?? Move.CreateMove(ms.Unit.Tile, ms.Unit);
        }

        private void SelectAction(IUnitAction action)
        {
            _battle.GameBoard.HideDialog();
            _currentAction = action;
            Unit.Tile.Parent.Tiles.ForEach(x => x.CanAttack = x.CanMoveTo = false);
            if (action.IsInstantAction(this))
            {
                ActionConfirmed();
                return;
            }
            var ms = _currentAction.GetValidTiles(this);
            ms.ForEach(x =>
                           {
                               x.Tile.CanMoveTo = x.MoveType == MoveType.Move;
                               x.Tile.CanAttack = x.MoveType == MoveType.Attack;
                           });
            _battle.GameBoard.UpdateTileHighlights();
            _battle.GameBoard.CalculateArrowOverlays(CurrentMoveSet);
        }

        private void Cancel()
        {
            if (!Actions.Any())
            {
                _battle.EndUnitAction();
            }
            else
            {
                _currentAction.Cancel(this);
                var act = Actions.Last();
                Actions.RemoveAt(Actions.Count - 1);
                SelectAction(act);
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
            if (Actions.Any())
            {
                var action = Actions[0];
                Actions.RemoveAt(0);
                action.Execute(this, Commit);
            }
            else
            {
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
