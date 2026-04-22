using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class CardHierarchy : UIScrollAndDrag
    {
        public TMP_Text Health;
        public TMP_Text Damage;
        public Image Character;
        public Image Race;
        public Image Rarity;
        public Image TypeIcon;
        public Button Button;
        public TMP_Text Level;
        public TMP_Text Experience;
        public ProgressBar LevelBar;
        public GameObject LevelBarContainer;
        public GameObject Lock;
    }
}