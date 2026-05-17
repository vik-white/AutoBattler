using UnityEngine;
using UnityEngine.UI;
using System;

namespace vikwhite
{
    public class BattleWindowHierarchy : MonoBehaviour
    {
        public Text FPS;
        public Button LobbyButton;
        public RectTransform AbilityContainer;

        public event Action Updated;

        private void Update()
        {
            Updated?.Invoke();
        }
    }
}
