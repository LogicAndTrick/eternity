using System;
using Eternity.Controls.Layouts;

namespace Eternity.Controls.Controls
{
    public class Button : LayoutControl
    {
        public Action Callback { get; set; }

        public Button(Action callback, ILayout layout) : base(layout)
        {
            Callback = callback;
        }

        public override void OnMouseDown(Input.EternityEvent e)
        {
            if (Callback != null) Callback();
        }
    }
}