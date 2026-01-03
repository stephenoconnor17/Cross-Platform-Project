#if WINDOWS
using System.Collections.Concurrent;

namespace CROSSPLATFORM2DGAME {
    public class KeyHandler {
        private readonly ConcurrentDictionary<string, bool> _keyStates = new();

        // Example keys
        public bool Left => IsKeyDown("Left") || IsKeyDown("A");
        public bool Right => IsKeyDown("Right") || IsKeyDown("D");
        public bool Up => IsKeyDown("Up") || IsKeyDown("W");
        public bool Down => IsKeyDown("Down") || IsKeyDown("S");
        public bool Space => IsKeyDown("Space");

        public bool IsKeyDown(string key) {
            return _keyStates.TryGetValue(key, out var pressed) && pressed;
        }

        internal void SetKeyDown(string key) => _keyStates[key] = true;
        internal void SetKeyUp(string key) => _keyStates[key] = false;
    }
}
#endif