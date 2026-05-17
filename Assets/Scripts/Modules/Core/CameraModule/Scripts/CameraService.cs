using UnityEngine;

namespace vikwhite
{
    public interface ICameraService
    {
        void Follow(Transform target);
        void Center();
        void Release();
    }

    public class CameraService : ICameraService, IUpdatable
    {
        private Camera _camera;
        private Transform _target;
        private float _cameraDistance;

        public void Follow(Transform target)
        {
            _target = target;
            _cameraDistance = 0;
        }

        public void Center()
        {
            if (_target == null) return;

            if (_camera == null)
                _camera = Camera.main;
            if (_camera == null) return;

            if (_cameraDistance <= 0)
                UpdateCameraDistance();

            _camera.transform.position = _target.position - _camera.transform.forward * _cameraDistance;
        }

        public void Release()
        {
            _target = null;
            _camera = null;
            _cameraDistance = 0;
        }

        public void Update()
        {
            Center();
        }

        private void UpdateCameraDistance()
        {
            if (_camera == null || _target == null) return;

            var targetOffset = _target.position - _camera.transform.position;
            _cameraDistance = Mathf.Abs(Vector3.Dot(targetOffset, _camera.transform.forward));
            if (_cameraDistance <= 0.01f)
                _cameraDistance = Vector3.Distance(_camera.transform.position, _target.position);
        }
    }
}
