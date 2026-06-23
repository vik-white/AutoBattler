using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class CharacterUpgradeHierarchy : MonoBehaviour
    {
        public TMP_Text Name;
        public TMP_Text Level;
        public TMP_Text LevelUpPrice;
        public Button LevelUpButton;
        public Button CloseButton;
        public Button PreviousLevelButton;
        public Button NextLevelButton;
        public Image Image;
        public Image ClassIcon;
        public StarsHierarchy Stars;
        public StatsInfoHierarchy StatsInfo;
    }
}
