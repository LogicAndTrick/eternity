using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls;
using System.Drawing;
using Eternity.Controls.Layouts;
using Eternity.DataStructures.Primitives;
using Eternity.Graphics;

namespace Eternity.Game.TurnBasedWarsGame.Controls.MapScreen
{
    public class ActionDialog : Control, IOverlayControl, IDisposable
    {

        public class ActionDialogAction
        {
            public string Text { get; set; }
            public Action Callback { get; set; }
        }

        private class ActionDialogButton : Control, IDisposable
        {
            private int Index { get; set; }
            private ActionDialogAction ButtonAction { get; set; }
            private ActionDialog ParentDialog { get; set; }
            private readonly TextRenderer _renderer;

            private bool _drawBorder = false;

            public ActionDialogButton(int index, ActionDialogAction action, ActionDialog parent)
            {
                Index = index;
                ButtonAction = action;
                ParentDialog = parent;
                _drawBorder = false;
                _renderer = new TextRenderer(action.Text);
                Box = new Box(0, 0, _renderer.Width + 8, _renderer.Height + 4);
            }

            public override void OnMouseEnter(Input.EternityEvent e)
            {
                _drawBorder = true;
            }

            public override void OnMouseLeave(Input.EternityEvent e)
            {
                _drawBorder = false;
            }

            public override void OnMouseDown(Input.EternityEvent e)
            {
                ButtonAction.Callback();
            }

            public override void OnRender(IRenderContext context)
            {
                context.DisableTextures();

                context.StartQuads();
                context.SetColour(Color.Teal);

                context.Point2(0, 0);
                context.Point2(Box.Width, 0);
                context.Point2(Box.Width, Box.Height);
                context.Point2(0, Box.Height);

                context.End();

                if (_drawBorder)
                {
                    context.StartLines();
                    context.SetColour(Color.White);

                    context.Point2(0, 0);
                    context.Point2(Box.Width, 0);
                    context.Point2(Box.Width, 0);
                    context.Point2(Box.Width, Box.Height);
                    context.Point2(Box.Width, Box.Height);
                    context.Point2(0, Box.Height);
                    context.Point2(0, Box.Height);
                    context.Point2(0, 0);

                    context.End();
                }

                context.EnableTextures();

                context.SetColour(Color.White);

                context.Translate(2, 2);
                _renderer.Render(context);
                context.Translate(-2, -2);

                context.SetColour(Color.Transparent);
            }

            public void Dispose()
            {
                _renderer.Dispose();
            }
        }

        public static ActionDialogAction Action(string text, Action callback)
        {
            return new ActionDialogAction {Text = text, Callback = callback};
        }

        public bool IsModal
        {
            get { return true; }
        }

        public Action CancelAction { get; private set; }

        private readonly List<ActionDialogButton> _buttons; 

        public ActionDialog(Box parent, Box target,
            params ActionDialogAction[] actions)
        {
            _buttons = new List<ActionDialogButton>();
            var container = new LayoutControl(new VerticalStackLayout(new Insets(1, 1, 1, 1), 1));
            var maxWidth = 0;
            var maxHeight = 0;
            for (var i = 0; i < actions.Length; i++)
            {
                var button = new ActionDialogButton(i, actions[i], this);
                maxWidth = Math.Max(button.Box.Width, maxWidth);
                maxHeight = Math.Max(button.Box.Height, maxHeight);
                _buttons.Add(button);
            }

            var x = target.X + target.Width;
            if (x + maxWidth > parent.Width) x = target.X - maxWidth;
            var y = target.Y;

            var box = new Box(0, 0, maxWidth, maxHeight);
            foreach (var button in _buttons)
            {
                button.Box = box;
                container.Add(button);
            }

            container.Box = new Box(x, y, maxWidth, (maxHeight + 2) * _buttons.Count);
            Add(container);

            var cancel = actions.FirstOrDefault(a => a.Text == "Cancel");
            CancelAction = cancel == null ? null : cancel.Callback;

            container.DoLayout();
        }

        public override void OnMouseDown(Input.EternityEvent e)
        {
            if (CancelAction != null && GetChildAt(e.X, e.Y) == null)
            {
                CancelAction();
            }
        }

        public override void OnRender(IRenderContext context)
        {
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

        public void Dispose()
        {
            _buttons.ForEach(x => x.Dispose());
        }
    }
}
