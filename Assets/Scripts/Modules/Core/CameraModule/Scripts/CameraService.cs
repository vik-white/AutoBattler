using UnityEngine;

namespace vikwhite
{
    public interface ICameraService
    {
        void Initialize(Vector3 position, Quaternion rotation, float fov, Transform parent = null);
    }

    public class CameraService : ICameraService
    {
        private Camera _camera = Camera.main;
        
        public void Initialize(Vector3 position, Quaternion rotation, float fov, Transform parent = null)
        {
            _camera.transform.SetParent(parent);
            _camera.transform.localPosition = position;
            _camera.transform.localRotation = rotation;
            _camera.fieldOfView = fov;
        }
    }
}
