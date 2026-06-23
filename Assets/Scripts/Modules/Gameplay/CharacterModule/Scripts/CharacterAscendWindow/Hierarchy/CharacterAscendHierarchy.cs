using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class CharacterAscendHierarchy : MonoBehaviour
    {
        public TMP_Text Name;
        public TMP_Text ShardPrice;
        public TMP_Text RedeemAmount;
        public RectTransform CloseButton;
        public RectTransform AscendButton;
        public Button RedeemButton;
        public RectTransform PreviousStarButton;
        public RectTransform NextStarButton;
        public Image Image;
        public Image ClassIcon;
        public Image ShardBar;
        public Image ShardBarIcon;
        public Image RedeemShardIcon;
        public Image HeroShardIcon;
        public StarsHierarchy Stars;
        public StatsInfoHierarchy StatsInfo;
    }
}
