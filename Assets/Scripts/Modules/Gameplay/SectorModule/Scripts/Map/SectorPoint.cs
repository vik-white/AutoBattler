using TMPro;
using UnityEngine;

namespace vikwhite
{
    public class SectorPoint : MonoBehaviour
    {
        public int Index;
        [SerializeField] private TMP_Text locationTitle;

        public void Initialize(string title)
        {
            locationTitle.text = title;
        }
    }
}
