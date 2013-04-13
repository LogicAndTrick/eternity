using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls;
using System.Drawing;
using Eternity.Controls.Controls;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    /// <summary>
    /// The menu dialog is a modal dialog that supports cancellation.
    /// It renders a rectangle over the entire control and displays the menu within that rectangle.
    /// </summary>
    public class MenuDialog : Control, IOverlayControl
    {

        public class MenuDialogAction
        {
            public string Text { get; set; }
            public Action Callback { get; set; }
        }

        public static MenuDialogAction Action(string text, Action callback)
        {
            return new MenuDialogAction {Text = text, Callback = callback};
        }

        public Action CancelAction { get; set; }

        public MenuDialog(Box parent, Box target, params MenuDialogAction[] actions)
        {
            // Create the menu and buttons
            var container = new LayoutControl(new VerticalStackLayout(new Insets(1, 1, 1, 1), 1));

            var buttons = actions.Select(action => new Button(action.Callback, action.Text, Color.White)).ToList();

            var maxWidth = buttons.Max(b => b.Box.Width);
            var maxHeight = buttons.Max(b => b.Box.Height);

            // Set the position
            var x = target.X + target.Width;
            if (x + maxWidth > parent.Width) x = target.X - maxWidth;
            var y = target.Y;

            // Set the size
            var box = new Box(0, 0, maxWidth, maxHeight);
            foreach (var button in buttons)
            {
                button.Box = box;
                container.Add(button);
            }

            container.Box = new Box(x, y, maxWidth, (maxHeight + 2) * buttons.Count);
            Add(container);

            var cancel = actions.FirstOrDefault(a => a.Text == "Cancel");
            CancelAction = cancel == null ? null : cancel.Callback;

            container.DoLayout();
        }

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
            if (CancelAction != null && GetChildAt(e.X, e.Y) == null)
            {
                // We've clicked on the background, run the cancel action
                CancelAction();
            }
        }

        public override void OnRender(IRenderContext context)
        {
            // Render the modal background
            context.DisableTextures();
            context.StartQuads();
            context.SetColour(Color.FromArgb(64, 64, 64, 64));
            context.Point2(0, 0);
            context.Point2(Box.Width, 0);
            context.Point2(Box.Width, Box.Height);
            context.Point2(0, Box.Height);
            context.SetColour(Color.Transparent);
            context.End();
            context.EnableTextures();
        }
    }
}
