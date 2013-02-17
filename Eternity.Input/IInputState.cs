using System.Collections.Generic;

namespace Eternity.Input
{
    public interface IInputState
    {
        bool IsFocused();

        bool IsMouseButtonDown(MouseButton button);
        bool IsMouseButtonUp(MouseButton button);

        IEnumerable<MouseButton> GetDownMouseButtons();

        int GetMouseClickCount(MouseButton button);
        void FlushMouseClickData(MouseButton button);
        void FlushMouseClickData();

        int GetMouseWheelOffset();
        void FlushMouseWheelData();

        int GetMouseX();
        int GetMouseY();

        int GetMouseOffsetX();
        int GetMouseOffsetY();
        void FlushMouseOffsetData();

        bool IsKeyDown(Key key);
        bool IsKeyUp(Key key);
        int GetKeyPresses(Key key);

        IEnumerable<Key> GetDownKeys();

        IEnumerable<EternityEvent> GetEvents();

        void FlushKeyPressData(Key key);
        void FlushKeyPressData();

        void FlushEventData();

        void FlushAllData();
    }
}
