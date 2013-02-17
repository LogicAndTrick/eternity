﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eternity.Controls.Layouts;

namespace Eternity.Controls
{
    public class LayoutControl : Control
    {
        private ILayout _layout;
        private readonly Dictionary<Control, object> _constraints;
        private bool _setUp;

        public LayoutControl(ILayout layout)
        {
            _layout = layout;
            _constraints = new Dictionary<Control, object>();
            _setUp = false;
        }

        public void SetLayout(ILayout layout)
        {
            _layout = layout;
            if (_setUp) DoLayout();
        }

        public void Add(Control child, object constraints)
        {
            AddInternal(child);
            if (Children.Contains(child))
            {
                _constraints[child] = constraints;
            }
            OnAdd(child);
        }

        public void DoLayout()
        {
            if (_layout != null) _layout.DoLayout(this, Children, _constraints);
            OnDoLayout();
        }

        protected virtual void OnDoLayout()
        {
            // Virtual
        }

        public override void OnSetUp(Graphics.IRenderContext context)
        {
            DoLayout();
            _setUp = true;
        }

        protected override void OnAdd(Control control)
        {
            if (_setUp) DoLayout();
        }

        protected override void OnRemove(Control control)
        {
            if (_constraints.ContainsKey(control))
            {
                _constraints.Remove(control);
            }
            if (_setUp) DoLayout();
        }

        public override void OnChildSizeChanged()
        {
            if (_setUp) DoLayout();
        }
    }
}
