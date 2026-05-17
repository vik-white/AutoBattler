using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace vikwhite
{
    public class SectorPoint : MonoBehaviour
    {
        public int Index;
        private string locationID;
        private string locationName;
        [SerializeField] private TMP_Text locationTitle;

        private ISectorMapService _sectorMap;
        private Camera _camera;

        public string LocationID => locationID;
        public string LocationName => locationName;
        public bool HasLocation => !string.IsNullOrEmpty(locationID);

        public void Initialize(ISectorMapService sectorMap)
        {
            _sectorMap = sectorMap;
        }

        public void SetLocation(string id, string title)
        {
            locationID = id;
            locationName = title;
            if (locationTitle != null)
                locationTitle.text = locationName;
            gameObject.SetActive(true);
        }

        public void ClearLocation()
        {
            locationID = string.Empty;
            locationName = string.Empty;
            if (locationTitle != null)
                locationTitle.text = string.Empty;
            gameObject.SetActive(false);
        }

        private void OnMouseDown()
        {
            if (!HasLocation) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            _sectorMap?.SelectLocation(this);
        }

        private void LateUpdate()
        {
            if (locationTitle == null) return;

            if (_camera == null)
                _camera = Camera.main;
            if (_camera == null) return;

            locationTitle.transform.LookAt(
                locationTitle.transform.position + _camera.transform.rotation * Vector3.forward,
                _camera.transform.rotation * Vector3.up);
        }
    }
}
