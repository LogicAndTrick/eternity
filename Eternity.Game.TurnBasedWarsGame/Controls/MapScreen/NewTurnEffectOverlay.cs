using System.Drawing;
using Eternity.Controls;
using Eternity.Controls.Animations;
using Eternity.Controls.Controls;
using Eternity.Controls.Easings;
using Eternity.DataStructures.Primitives;
using Eternity.Game.TurnBasedWarsGame.WarsGame.Turns;
using Eternity.Graphics;
using Point = Eternity.DataStructures.Primitives.Point;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class NewTurnEffectOverlay : EffectOverlay
    {
        public Turn Turn { get; private set; }

        private Font _font;

        private Label _day;
        private Label _dayNumber;
        private Label _fight;

        public NewTurnEffectOverlay(Turn turn)
        {
            _font = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold);
            Turn = turn;
            _day = new Label("Day", Color.White, _font);
            _dayNumber = new Label(turn.Day.ToString(), Color.White, _font);
            _fight = new Label("Fight!", Color.White, _font);

            Add(_day);
            Add(_dayNumber);
            Add(_fight);

            var dps = _day.GetPreferredSize();
            var dnps = _dayNumber.GetPreferredSize();
            var fps = _fight.GetPreferredSize();
            _day.Box = new Box(new Point(-dps.Width, 0), dps);
            _dayNumber.Box = new Box(new Point(-dnps.Width, 0), dnps);
            _fight.Box = new Box(new Point(-fps.Width, 0), fps);

            CalculateLabels();

            var d = Animation<int>.Delay(0);
            d.Queue(_ => AddAnimation(_), () => _day.Box.X, () => 50, 200, new QuadEasing().Out(), x => UpdateBox(x, _day))
                .Queue(_ => AddAnimation(_), () => _day.Box.X, () => _day.Box.X + 20, 2000, new LinearEasing(), x => UpdateBox(x, _day))
                .Queue(_ => AddAnimation(_), () => _day.Box.X, () => _dayNumber.Box.X + Parent.Box.Width, 100, new LinearEasing(), x => UpdateBox(x, _day));
            AddAnimation(d);

            d = Animation<int>.Delay(200);
            d.Queue(_ => AddAnimation(_), () => _dayNumber.Box.X, () => 80, 200, new QuadEasing().Out(), x => UpdateBox(x, _dayNumber))
                .Queue(_ => AddAnimation(_), () => _dayNumber.Box.X, () => _dayNumber.Box.X + 20, 1800, new LinearEasing(), x => UpdateBox(x, _dayNumber))
                .Queue(_ => AddAnimation(_), () => _dayNumber.Box.X, () => _dayNumber.Box.X + Parent.Box.Width, 100, new LinearEasing(), x => UpdateBox(x, _dayNumber));
            AddAnimation(d);

            d = Animation<int>.Delay(1000);
            d.Queue(_ => AddAnimation(_), () => _fight.Box.X, () => 150, 200, new QuadEasing().Out(), x => UpdateBox(x, _fight))
                .Queue(_ => AddAnimation(_), () => _fight.Box.X, () => _fight.Box.X + 20, 1000, new LinearEasing(), x => UpdateBox(x, _fight))
                .Queue(_ => AddAnimation(_), () => _fight.Box.X, () => _dayNumber.Box.X + Parent.Box.Width, 100, new LinearEasing(), x => UpdateBox(x, _fight));
            AddAnimation(d);

            AddAnimation(Animation<int>.Delay(2400, x => Hide()));
        }

        private void UpdateBox(int x, Control label)
        {
            if (Disposed) return;
            label.Box = new Box(x, label.Box.Y, label.Box.Width, label.Box.Height);
        }

        private void CalculateLabels()
        {
            _day.Box = new Box(_day.Box.X, (int)(Box.Height * 0.3), _day.Box.Width, _day.Box.Height);
            _dayNumber.Box = new Box(_dayNumber.Box.X, (int)(Box.Height * 0.5), _dayNumber.Box.Width, _dayNumber.Box.Height);
            _fight.Box = new Box(_fight.Box.X, (int)(Box.Height * 0.7), _fight.Box.Width, _fight.Box.Height);
        }

        public override void OnRender(IRenderContext context)
        {
            context.SetScissor(Parent.Box.X, Parent.Box.Y, Parent.Box.Width, Parent.Box.Height);
        }

        public override void OnAfterRender(IRenderContext context)
        {
            context.RemoveScissor();
        }

        public override void OnSizeChanged()
        {
            CalculateLabels();
        }

        public override void OnDispose()
        {
            _font.Dispose();
        }
    }
}