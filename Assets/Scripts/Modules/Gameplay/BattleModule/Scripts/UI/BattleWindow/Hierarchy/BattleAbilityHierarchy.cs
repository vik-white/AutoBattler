using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class BattleAbilityHierarchy : MonoBehaviour
    {
        public Button Button;
        public RectTransform HealthBar;
        public RectTransform AbilityBar;
        public Image Fade;
        public Image Icon;
        public TMP_Text Title;

        public event Action Updated;
        
        private void Update()
        {
            Updated?.Invoke();
        }
    }
}
