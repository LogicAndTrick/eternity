using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Controls;
using Eternity.Controls.Effects;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class EffectOverlay : Control, IOverlayControl
    {
        protected bool Disposed = false;

        public bool InterceptEvent(EternityEvent e)
        {
            return true;
        }

        public bool ListenEvent(EternityEvent e)
        {
            return true;
        }

        public override void OnMouseDown(EternityEvent e)
        {
            Hide();
        }

        protected void Hide()
        {
            if (Disposed) return;
            Disposed = true;
            StopAnimations();
            Parent.Remove(this);
        }
    }
}
