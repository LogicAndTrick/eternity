using System;
using System.Collections.Generic;
using System.Linq;
using Eternity.Controls.Animations;
using Eternity.Controls.Easings;
using Eternity.Controls.Effects;
using Eternity.DataStructures.Primitives;
using Eternity.Game;
using Eternity.Graphics;
using Eternity.Input;

namespace Eternity.Controls
{
    public class Control : IRenderable, IUpdatable, IDisposable
    {
        public Control Parent { get; protected set; }
        protected List<Control> Children;
        protected List<Control> Overlays;
        protected AnimationQueue AnimationQueue;
        protected EffectQueue EffectQueue;
        protected bool Clip;

        private Box _box;
        public Box Box
        {
            get { return _box; }
            set
            {
                ResizeSafe(value);
                OnSizeChanged();
                if (Parent != null)
                {
                    Parent.OnChildSizeChanged();
                }
            }
        }

        public Size ActualSize {get { return new Size(_box.Width, _box.Height); }}
        public int NumChildren { get { return Children.Count; } }
        public Size PreferredSize { get; set; }

        public bool OnFocusPath { get; private set; }

        public Control()
        {
            Parent = null;
            Children = new List<Control>();
            Overlays = new List<Control>();
            _box = new Box(0, 0, 100, 100);
            PreferredSize = new Size(100, 100);
            AnimationQueue = new AnimationQueue();
            EffectQueue = new EffectQueue();
            Clip = false;
        }

        public virtual Size GetPreferredSize()
        {
            return PreferredSize;
        }

        public void SetUp(IRenderContext context)
        {
            OnSetUp(context);
            Children.ForEach(x => x.SetUp(context));
            Overlays.ForEach(x => x.SetUp(context));
        }

        // ADDING AND REMOVING

        protected void AddInternal(Control child)
        {
            if (child.Parent != null)
            {
                if (child.Parent == this) return;
                child.Parent.Remove(child);
            }
            child.Parent = this;
            Children.Add(child);
        }

        protected void AddOverlayInternal(Control child)
        {
            if (child.Parent != null)
            {
                if (child.Parent == this) return;
                child.Parent.Remove(child);
            }
            child.Parent = this;
            Overlays.Add(child);
            child.Box = Box;
        }

        public void ResizeSafe(Box box)
        {
            _box = box;
            var overlayBox = new Box(0, 0, box.Width, box.Height);
            Overlays.ForEach(x =>
                                 {
                                     x.ResizeSafe(overlayBox);
                                     x.OnSizeChanged();
                                 });
            OnSizeChanged();
        }

        public void Add(Control child)
        {
            AddInternal(child);
            OnAdd(child);
        }

        public void AddOverlay(Control child)
        {
            AddOverlayInternal(child);
            OnAdd(child);
        }

        public void Remove(Func<Control, bool> test)
        {
            foreach (var child in GetAllChildren().Where(test).ToList())
            {
                Remove(child);
            }
        }

        public void Remove(Control child)
        {
            if (child.Parent == null || child.Parent != this) return;
            child.Parent = null;
            Children.Remove(child);
            Overlays.Remove(child);
            OnRemove(child);
            child.Dispose();
        }

        public void AddAnimation(params IAnimation[] animation)
        {
            foreach (var anim in animation)
            {
                AnimationQueue.AddAnimation(anim);
            }
        }

        public void AddAnimationSequential(params IAnimation[] animation)
        {
            foreach (var anim in animation)
            {
                AnimationQueue.AddSequential(anim);
            }
        }

        public void StopAnimations()
        {
            AnimationQueue.StopAnimations();
        }

        public void StopSequentialAnimations()
        {
            AnimationQueue.StopSequential();
        }

        public void AddEffect(params IEffect[] effect)
        {
            foreach (var eff in effect)
            {
                EffectQueue.AddEffect(eff);
            }
        }

        public void Delay(long milliseconds, Action callback)
        {
            AddAnimation(new Animation<double>(0, 1, milliseconds, new LinearEasing(), null, x => callback()));
        }

        // TREE TRAVERSAL / CHILD LOCATING

        // private IEnumerable<Control> GetEventAcceptingChildren()
        // {
        //     return Overlays.OfType<IOverlayControl>().Any(x => x.IsModal)
        //                ? Overlays.Where(x => x is IOverlayControl && ((IOverlayControl) x).IsModal).Take(1)
        //                : Children;
        // }

        protected IEnumerable<Control> GetAllChildren()
        {
            return Children.Union(Overlays);
        }

        public Control GetChildAt(int x, int y)
        {
            var p = new Point(x, y);
            return Children.LastOrDefault(control => control.Box.Contains(p));
        }

        public List<Control> GetChildrenOverlapping(Line line)
        {
            Point p1, p2;
            return Children.Where(control => line.Intersects(control.Box, out p1, out p2)).ToList();
        }

        public Control GetFocusedChild()
        {
            return GetAllChildren().FirstOrDefault(x => x.OnFocusPath);
        }

        public Point GetLocationInTree()
        {
            var pt = new Point(0, 0);
            var ctrl = this;
            while (ctrl != null)
            {
                pt = new Point(pt.X + ctrl.Box.X, pt.Y + ctrl.Box.Y);
                ctrl = ctrl.Parent;
            }
            return pt;
        }

        // CONTROL FOCUSING

        public void Unfocus()
        {
            foreach (var control in GetAllChildren().Where(control => control.OnFocusPath))
            {
                control.Unfocus();
            }
            OnFocusPath = false;
            OnControlFocusChanged();
        }

        public void TakeFocus()
        {
            if (OnFocusPath)
            {
                var focus = GetAllChildren().FirstOrDefault(x => x.OnFocusPath);
                if (focus != null)
                {
                    focus.Unfocus();
                    OnControlFocusChanged();
                }
                return;
            }

            if (Parent == null)
            {
                OnFocusPath = true;
                OnControlFocusChanged();
                return;
            }

            // Walk up the tree until we have the closest control on the focus path
            var path = new List<Control> { this };
            var elem = Parent;
            while (elem.Parent != null && !elem.OnFocusPath)
            {
                path.Add(elem);
                elem = elem.Parent;
            }
            // elem is now either unfocused tree root or is on the focus path
            elem.TakeFocus(); // This will unfocus any children of the element
            path.Reverse(); // Focus the elements from the top of the tree down to this element
            foreach (var e in path)
            {
                e.OnFocusPath = true;
                e.OnControlFocusChanged();
            }
        }

        // EVENTS

        protected void EventBubbleDown(EternityEvent ee)
        {
            // Check intercepting overlays
            var overlays = Overlays.OfType<IOverlayControl>().ToList();
            var intercept = overlays.FirstOrDefault(x => x.InterceptEvent(ee)) as Control;
            if (intercept != null)
            {
                intercept.EventBubbleDown(ee.Clone());
                return;
            }
            // Check listening overlays
            overlays.Where(x => x.ListenEvent(ee)).OfType<Control>().ToList().ForEach(x => x.EventBubbleDown(ee.Clone()));
            
            var bubbleChildren = new List<Control>();
            var childAt = GetChildAt(ee.X, ee.Y);
            var childFocus = GetFocusedChild();
            switch (ee.Type)
            {
                case EventType.MouseDown:
                    OnMouseDown(ee);
                    bubbleChildren.Add(childAt);
                    break;
                case EventType.MouseUp:
                    OnMouseUp(ee);
                    bubbleChildren.Add(childAt);
                    break;
                case EventType.MouseMove:
                    OnMouseMove(ee);
                    var line = new Line(new Point(ee.LastX, ee.LastY), new Point(ee.X, ee.Y));
                    foreach (var control in Children)
                    {
                        Point p1, p2;
                        if (!line.Intersects(control.Box, out p1, out p2)) continue;
                        if (p1 != null) control.OnMouseEnter(ee);
                        if (p2 != null) control.OnMouseLeave(ee);
                        bubbleChildren.Add(control);
                    }
                    break;
                case EventType.MouseWheel:
                    OnMouseWheel(ee);
                    bubbleChildren.Add(childAt);
                    break;
                case EventType.KeyDown:
                    OnKeyDown(ee);
                    bubbleChildren.Add(childFocus);
                    break;
                case EventType.KeyUp:
                    OnKeyUp(ee);
                    bubbleChildren.Add(childFocus);
                    break;
                case EventType.FocusChanged:
                    OnWindowFocusChanged(ee);
                    bubbleChildren.AddRange(GetAllChildren());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            bubbleChildren = bubbleChildren.Where(b => b != null).ToList();
            foreach (var b in bubbleChildren)
            {
                var ce = ee.Clone();
                ce.Translate(b.Box.X, b.Box.Y);
                b.EventBubbleDown(ce);
            }
        }

        // UPDATE AND RENDER

        public void Render(IRenderContext context)
        {
            if (Clip) context.SetScissor(Box.X, Box.Y, Box.Width, Box.Height);
            OnRender(context);
            foreach (var control in Children)
            {
                context.Translate(control.Box.X, control.Box.Y);
                control.Render(context);
                context.Translate(-control.Box.X, -control.Box.Y);
            }
            OnAfterRender(context);
            Overlays.ForEach(x => x.Render(context));
            EffectQueue.Render(context);
            OnAfterRenderOverlays(context);
            if (Clip) context.RemoveScissor();
        }

        public void Update(FrameInfo info, IInputState state)
        {
            // Events
            var events = new List<EternityEvent>(state.GetEvents());
            state.FlushEventData();
            foreach (var e in events)
            {
                EventBubbleDown(e);
            }

            OnUpdate(info, state);

            AnimationQueue.Update(info, state);
            EffectQueue.Update(info, state);
            OnAnimate();
            AnimationQueue.RemoveCompletedAnimations();
            EffectQueue.RemoveCompletedEffects();

            GetAllChildren().ToList().ForEach(x => x.Update(info, state));
        }

        // VIRTUAL METHODS

        public virtual void OnSetUp(IRenderContext context)
        {
            // Virtual
        }

        protected virtual void OnAdd(Control control)
        {
            // Virtual
        }

        protected virtual void OnRemove(Control control)
        {
            // Virtual
        }

        public virtual void OnRender(IRenderContext context)
        {
            // Virtual
        }

        public virtual void OnAfterRender(IRenderContext context)
        {
            // Virtual
        }

        public virtual void OnAfterRenderOverlays(IRenderContext context)
        {
            // Virtual
        }

        public virtual void OnUpdate(FrameInfo info, IInputState state)
        {
            // Virtual
        }

        public virtual void OnAnimate()
        {
            // Virtual
        }

        public virtual void OnMouseDown(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnMouseUp(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnMouseMove(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnMouseWheel(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnKeyDown(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnKeyUp(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnMouseEnter(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnMouseLeave(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnWindowFocusChanged(EternityEvent e)
        {
            // Virtual
        }

        public virtual void OnControlFocusChanged()
        {
            // Virtual
        }

        public virtual void OnWindowSizeChanged()
        {
            // Virtual
        }

        public virtual void OnSizeChanged()
        {
            // Virtual
        }

        public virtual void OnChildSizeChanged()
        {
            // Virtual
        }

        public void Dispose()
        {
            OnDispose();
            Children.ForEach(x => x.Dispose());
        }

        public virtual void OnDispose()
        {
            // Virtual
        }
    }
}
