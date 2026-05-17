using UnityEngine;

namespace vikwhite
{
    public interface ICameraService
    {
        void SetTarget(Transform target);
        void ClearTarget();
    }

    public class CameraService : ICameraService, IUpdatable
    {
        private Camera _camera = Camera.main;
        private Vector3 _battlePosition = new (-51.6f, 47.5f, - 29.7f );
        private Transform _target;

        public void SetTarget(Transform target)
        {
            _target = target;
            FollowTarget();
        }

        public void ClearTarget()
        {
            _target = null;
            _camera.transform.position = _battlePosition;
        }

        public void Update()
        {
            if (_target != null) FollowTarget();
            else _camera.transform.position = _battlePosition;
        }

        private void FollowTarget()
        {
            _camera.transform.position = _target.position - _camera.transform.forward * GetCameraDistance();
        }

        private float GetCameraDistance()
        {
            var targetOffset = _target.position - _camera.transform.position;
            var cameraDistance = Mathf.Abs(Vector3.Dot(targetOffset, _camera.transform.forward));
            if (cameraDistance <= 0.01f) cameraDistance = Vector3.Distance(_camera.transform.position, _target.position);
            return cameraDistance;
        }
    }
}
