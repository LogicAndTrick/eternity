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

namespace Eternity.Game.TurnBasedWarsGame.WarsGame.Interactions.UnitActions.Common
{
    internal class PopupEffect : IEffect
    {
        private long _startTime;
        private bool _finished;
        public long Time { get; set; }
        public GameBoard Board { get; set; }
        public Dictionary<Tile, string> TileSprites { get; set; }

        public void Update(FrameInfo info, IInputState state)
        {
            _finished = info.TotalMilliseconds > _startTime + Time;
        }

        public void Render(IRenderContext context)
        {
            foreach (var t in TileSprites)
            {
                var tile = Board.GetTileControl(t.Key.Location);
                if (tile == null) continue;
                SpritePool.DrawSprite(context, "Overlays", t.Value,
                                      tile.Box.Offset(tile.Box.Width * 0.75, -tile.Box.Height / 2),
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
}