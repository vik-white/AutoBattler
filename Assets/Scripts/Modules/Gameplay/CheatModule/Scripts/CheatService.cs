using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface ICheatService : IUpdatable { }

    public class CheatService : ICheatService
    {
        private readonly ICheatWindow _cheatWindow;

        public CheatService(ICheatWindow cheatWindow)
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
