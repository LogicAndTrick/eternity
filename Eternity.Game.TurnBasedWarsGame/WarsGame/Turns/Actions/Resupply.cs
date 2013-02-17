using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls;
using Eternity.Controls.Effects;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Tiles;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Turns.Actions
{
    /// <summary>
    /// APCs can resupply units with fuel and ammo
    /// </summary>
    public class Resupply : IUnitAction, IUnitActionGenerator
    {
        private class PopupEffect : IEffect
        {
            private long _startTime;
            private bool _finished;
            public long Time { get; set; }
            public GameBoard Board { get; set; }
            public List<Unit> UnitsToSupply { get; set; }

            public void Update(FrameInfo info, IInputState state)
            {
                _finished = info.TotalMilliseconds > _startTime + Time;
            }

            public void Render(IRenderContext context)
            {
                foreach (var unit in UnitsToSupply.Where(x => x.Tile != null))
                {
                    var tile = Board.GetTileControl(unit.Tile.Location);
                    if (tile == null) continue;
                    SpritePool.DrawSprite(context, "Overlays", "SupplyRight",
                                          tile.Box.Offset((int)(tile.Box.Width*0.75), -tile.Box.Height/2),
                                          new SpriteDrawingOptions { DockX = SpriteDrawingOptions.Dock.Left});
                }
            }

            public bool IsFinished()
            {
                return _finished;
            }

            public void Start(long currentTimeInMilliseconds)
            {
                _startTime = currentTimeInMilliseconds;
            }
        }

        public UnitActionType ActionType
        {
            get { return UnitActionType.Resupply; }
        }

        public void Execute(UnitActionSet set, Action callback)
        {
            var target = set.CurrentMoveSet.Last();
            var units = target.MoveTile.GetAdjacentTiles()
                .Where(x => x != null && x.Unit != null && x.Unit.Army == set.Unit.Army && x.Unit.CanBeResupplied())
                .Select(x => x.Unit)
                .ToList();
            units.ForEach(x => x.Resupply());
            var board = set.Unit.Tile.Parent.Battle.GameBoard;
            board.AddEffect(new PopupEffect { Board = board, Time = 500, UnitsToSupply = units });
            board.Delay(500, callback);
        }

        public bool IsValidFor(UnitActionSet set)
        {
            var target = set.CurrentMoveSet.LastOrDefault();
            return target != null &&
                   target.MoveTile.GetAdjacentTiles()
                       .Any(x => x != null && x.Unit != null && x.Unit.Army == set.Unit.Army && x.Unit.CanBeResupplied());
        }

        public IEnumerable<IUnitAction> GetActions(UnitActionSet set)
        {
            yield return new Resupply();
        }

        public bool IsValidTile(Tile tile, UnitActionSet set)
        {
            return true;
        }

        public List<ValidTile> GetValidTiles(UnitActionSet set)
        {
            throw new InvalidOperationException("The resupply action is instant and doesn't have a set of valid tiles.");
        }

        public void UpdateMoveSet(Tile tile, UnitActionSet set)
        {
            throw new InvalidOperationException("The resupply action is instant and cannot update the move set.");
        }

        public void Cancel(UnitActionSet set)
        {
            // 
        }

        public string GetName()
        {
            return "Resupply";
        }

        public bool IsInstantAction(UnitActionSet set)
        {
            return true;
        }

        public bool IsCommittingAction(UnitActionSet set)
        {
            return true;
        }
    }
}