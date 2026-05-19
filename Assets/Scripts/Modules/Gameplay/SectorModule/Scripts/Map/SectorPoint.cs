using TMPro;
using UnityEngine;

namespace vikwhite
{
    public class SectorPoint : MonoBehaviour
    {
        public int Index;
        [SerializeField] private TMP_Text locationTitle;

        public Vector3 Position => transform.position;

        public void Initialize(string title)
        {
            locationTitle.text = title;
            gameObject.SetActive(true);
        }
    }
}
