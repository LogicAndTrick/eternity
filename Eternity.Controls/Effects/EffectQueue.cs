using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Game;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Controls.Effects
{
    public class EffectQueue : IUpdatable
    {
        private List<IEffect> _new; 
        private List<IEffect> _effects;

        public EffectQueue()
        {
            _new = new List<IEffect>();
            _effects = new List<IEffect>();
        }

        public bool IsEmpty()
        {
            return !_effects.Any(x => !x.IsFinished());
        }

        public void AddEffect(IEffect effect)
        {
            _new.Add(effect);
        }

        public void Update(FrameInfo info, IInputState state)
        {
            // Start new effects
            foreach (var effect in _new)
            {
                effect.Start(info.TotalMilliseconds);
                _effects.Add(effect);
            }
            _new.Clear();

            // Update running effects
            foreach (var effect in _effects)
            {
                effect.Update(info, state);
            }
        }

        public void Render(IRenderContext context)
        {
            _effects.ForEach(x => x.Render(context));
        }

        public void RemoveCompletedEffects()
        {
            _effects.RemoveAll(x => x.IsFinished());
        }
    }
}
