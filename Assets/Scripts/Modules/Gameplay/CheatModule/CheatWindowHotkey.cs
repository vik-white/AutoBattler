using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface ICheatWindowHotkey : IUpdatable { }

    public class CheatWindowHotkey : ICheatWindowHotkey
    {
        private readonly ICheatWindow _cheatWindow;

        public CheatWindowHotkey(ICheatWindow cheatWindow)
        {
            _cheatWindow = cheatWindow;
        }

        public void Update()
        {
            if (Keyboard.current?.cKey.wasPressedThisFrame == true)
                _cheatWindow.ToggleWindow();
        }
    }
}
