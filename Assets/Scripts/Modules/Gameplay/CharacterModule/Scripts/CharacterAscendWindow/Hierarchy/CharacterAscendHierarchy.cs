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
        public TMP_Text Might;
        public Button CloseButton;
        public Button AscendButton;
        public Button SummonButton;
        public Button PreviousStarButton;
        public Button NextStarButton;
        public Button RedeemButton;
        public Image Image;
        public Image ClassIcon;
        public ProgressBar ShardBar;
        public Image ShardBarIcon;
        public Image RedeemShardIcon;
        public Image HeroShardIcon;
        public StarsHierarchy Stars;
        public StatsInfoHierarchy StatsInfo;
    }
}
