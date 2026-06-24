using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class CharacterSkillsWindowHierarchy : MonoBehaviour
    {
        public TMP_Text Name;
        public TMP_Text SkillName;
        public TMP_Text SkillDescription;
        public TMP_Text SkillUpgradePrice;
        public TMP_Text BooksAmount;
        public Button StatsButton;
        public Button UpgradeButton;
        public Button CloseButton;
        public Button RedeemButton;
        public Image Image;
        public Image ClassIcon;
        public Image BookClassIcon;
        public SkillItemHierarchy[] SkillItems;
    }
}
