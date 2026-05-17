using TMPro;
using UnityEngine;

namespace vikwhite
{
    public class SectorPoint : MonoBehaviour
    {
        public int Index;
        private string locationID;
        [SerializeField] private TMP_Text locationTitle;

        public string LocationID => locationID;
        public Vector3 Position => transform.position;
        public bool HasLocation => !string.IsNullOrEmpty(locationID);

        public void Initialize(string id, string title)
        {
            locationID = id;
            if (locationTitle != null)
                locationTitle.text = title;

            gameObject.SetActive(true);
        }

        public void Clear()
        {
            locationID = string.Empty;
            if (locationTitle != null)
                locationTitle.text = string.Empty;

            gameObject.SetActive(false);
        }
    }
}
