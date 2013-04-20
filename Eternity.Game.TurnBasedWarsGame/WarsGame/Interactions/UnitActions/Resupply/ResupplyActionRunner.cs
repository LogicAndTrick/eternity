using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls;
using Eternity.Controls.Effects;
using Eternity.Game.TurnBasedWarsGame.Controls.MapScreen;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Units;
using Eternity.Graphics;
using Eternity.Graphics.Sprites;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Resupply
{
    public class ResupplyActionRunner : IUnitActionRunner
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
                                          tile.Box.Offset((int)(tile.Box.Width * 0.75), -tile.Box.Height / 2),
                                          new SpriteDrawingOptions { DockX = SpriteDrawingOptions.Dock.Left });
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

        private readonly Unit _supplier;

        public ResupplyActionRunner(Unit supplier)
        {
            _supplier = supplier;
        }

        public void Execute(Action callback)
        {
            var units = _supplier.Tile.GetAdjacentTiles()
                .Where(x => x != null && x.Unit != null && x.Unit.Army == _supplier.Army && x.Unit.CanBeResupplied())
                .Select(x => x.Unit)
                .ToList();
            units.ForEach(x => x.Resupply());
            var board = _supplier.Tile.Parent.Battle.GameBoard;
            board.AddEffect(new PopupEffect { Board = board, Time = 500, UnitsToSupply = units });
            board.Delay(500, callback);
        }
    }
}