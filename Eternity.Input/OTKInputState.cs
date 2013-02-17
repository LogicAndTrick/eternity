using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Input;

namespace Eternity.Input
{
    public class OTKInputState : IInputState
    {
        private readonly GameWindow _window;
        private readonly MouseDevice _mouse;
        private readonly KeyboardDevice _keyboard;

        private readonly List<Key> _downKeys;
        private readonly List<Key> _pressedKeys;
        private readonly List<MouseButton> _downButtons;
        private readonly List<MouseButton> _clickedButtons;
        private readonly List<EternityEvent> _events; 

        private int _currentX;
        private int _currentY;
        private int _lastX;
        private int _lastY;

        private int _wheelDelta;

        private bool _focused;

        public OTKInputState(GameWindow window)
        {
            _downKeys = new List<Key>();
            _pressedKeys = new List<Key>();
            _downButtons = new List<MouseButton>();
            _clickedButtons = new List<MouseButton>();
            _events = new List<EternityEvent>();

            _currentX = 0;
            _currentY = 0;
            _lastX = 0;
            _lastY = 0;

            _wheelDelta = 0;

            _focused = window.Focused;

            _window = window;
            _mouse = window.Mouse;
            _keyboard = window.Keyboard;

            _mouse.ButtonDown += (sender, e) => MouseDown(e);
            _mouse.ButtonUp += (sender, e) => MouseUp(e);
            _mouse.Move += (sender, e) => MouseMove(e);
            _mouse.WheelChanged += (sender, e) => MouseWheel(e);

            _keyboard.KeyDown += (sender, e) => KeyDown(e);
            _keyboard.KeyUp += (sender, e) => KeyUp(e);

            _window.FocusedChanged += (sender, e) => FocusChanged();
            _window.MouseLeave += (sender, e) => MouseLeave();
        }

        private static MouseButton GetMouseButton(MouseButtonEventArgs args)
        {
            return (MouseButton)Enum.Parse(typeof(MouseButton), args.Button.ToString());
        }

        private static Key GetKey(KeyboardKeyEventArgs args)
        {
            return (Key)Enum.Parse(typeof(Key), args.Key.ToString());
        }

        private void MouseDown(MouseButtonEventArgs args)
        {
            var mb = GetMouseButton(args);
            _events.Add(new EternityEvent { Type = EventType.MouseDown, Button = mb, X = args.X, Y = args.Y });
            if (_downButtons.Contains(mb)) return;
            _downButtons.Add(mb);
        }

        private void MouseUp(MouseButtonEventArgs args)
        {
            var mb = GetMouseButton(args);
            _events.Add(new EternityEvent { Type = EventType.MouseUp, Button = mb, X = args.X, Y = args.Y });
            _downButtons.Remove(mb);
            _clickedButtons.Add(mb);
        }

        private void MouseMove(MouseMoveEventArgs args)
        {
            if (args.X == _currentX && args.Y == _currentY) return;
            _events.Add(new EternityEvent { Type = EventType.MouseMove, X = args.X, Y = args.Y, LastX = _currentX, LastY = _currentY });
            _currentX = args.X;
            _currentY = args.Y;
        }

        private void MouseWheel(MouseWheelEventArgs args)
        {
            _events.Add(new EternityEvent { Type = EventType.MouseWheel, Delta = args.Delta, X = args.X, Y = args.Y });
            _wheelDelta += args.Delta;
        }

        private void KeyDown(KeyboardKeyEventArgs args)
        {
            var kk = GetKey(args);
            _events.Add(new EternityEvent { Type = EventType.KeyDown, Key = kk, X = _currentX, Y = _currentY });
            _downKeys.Add(kk);
        }

        private void KeyUp(KeyboardKeyEventArgs args)
        {
            var kk = GetKey(args);
            _events.Add(new EternityEvent { Type = EventType.KeyUp, Key = kk, X = _currentX, Y = _currentY });
            _downKeys.Remove(kk);
            _pressedKeys.Add(kk);
        }

        private void FocusChanged()
        {
            _events.Add(new EternityEvent { Type = EventType.FocusChanged, Focused = _window.Focused });
            _focused = _window.Focused;
        }

        private void MouseLeave()
        {
            foreach (var mb in _downButtons)
            {
                _events.Add(new EternityEvent { Type = EventType.MouseUp, Button = mb, X = _currentX, Y = _currentY });
            }
            _downButtons.Clear();
        }

        public bool IsFocused()
        {
            return _focused;
        }

        public bool IsMouseButtonDown(MouseButton button)
        {
            return _downButtons.Contains(button);
        }

        public bool IsMouseButtonUp(MouseButton button)
        {
            return !_downButtons.Contains(button);
        }

        public IEnumerable<MouseButton> GetDownMouseButtons()
        {
            return _downButtons.Distinct();
        }

        public int GetMouseClickCount(MouseButton button)
        {
            return _clickedButtons.Count(b => b == button);
        }

        public void FlushMouseClickData(MouseButton button)
        {
            _clickedButtons.RemoveAll(b => b == button);
        }

        public void FlushMouseClickData()
        {
            _clickedButtons.Clear();
        }

        public int GetMouseWheelOffset()
        {
            return _wheelDelta;
        }

        public void FlushMouseWheelData()
        {
            _wheelDelta = 0;
        }

        public int GetMouseX()
        {
            return _currentX;
        }

        public int GetMouseY()
        {
            return _currentY;
        }

        public int GetMouseOffsetX()
        {
            return _currentX - _lastX;
        }

        public int GetMouseOffsetY()
        {
            return _currentY - _lastY;
        }

        public void FlushMouseOffsetData()
        {
            _lastX = _currentX;
            _lastY = _currentY;
        }

        public bool IsKeyDown(Key key)
        {
            return _downKeys.Contains(key);
        }

        public bool IsKeyUp(Key key)
        {
            return !_downKeys.Contains(key);
        }

        public int GetKeyPresses(Key key)
        {
            return _pressedKeys.Count(k => k == key);
        }

        public IEnumerable<Key> GetDownKeys()
        {
            return _downKeys.Distinct();
        }

        public IEnumerable<EternityEvent> GetEvents()
        {
            return _events;
        }

        public void FlushKeyPressData(Key key)
        {
            _pressedKeys.RemoveAll(k => k == key);
        }

        public void FlushKeyPressData()
        {
            _pressedKeys.Clear();
        }

        public void FlushEventData()
        {
            _events.Clear();
        }

        public void FlushAllData()
        {
            FlushKeyPressData();
            FlushMouseClickData();
            FlushMouseOffsetData();
            FlushMouseWheelData();
            FlushEventData();
        }
    }
}
