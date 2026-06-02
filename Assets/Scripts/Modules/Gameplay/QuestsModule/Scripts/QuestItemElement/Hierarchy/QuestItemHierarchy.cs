using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class QuestItemHierarchy : MonoBehaviour
    {
        public TMP_Text Description;
        public TMP_Text Progress;
        public Slider ProgressBar;
        public RectTransform RewardsContainer;
        public Button ClaimButton;
        public GameObject ClaimedLabel;
    }
}
