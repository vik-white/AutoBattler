using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public interface ICameraService
    {
        void Initialize(Vector3 position, Quaternion rotation, float fov, Transform parent = null);
        void DetachFromParent();
        void MoveTo(Vector3 position, float duration);
    }

    public class CameraService : ICameraService
    {
        private Camera _camera;
        private Scene _originScene;

        public CameraService()
        {
            UpdateCameraReference();
        }
        
        public void Initialize(Vector3 position, Quaternion rotation, float fov, Transform parent = null)
        {
            var camera = UpdateCameraReference();
            camera.transform.SetParent(parent);
            camera.transform.localPosition = position;
            camera.transform.localRotation = rotation;
            camera.fieldOfView = fov;
        }
        
        public void DetachFromParent()
        {
            var camera = UpdateCameraReference();
            camera.transform.SetParent(null, true);
            if (_originScene.IsValid() && _originScene.isLoaded &&
                camera.gameObject.scene.handle != _originScene.handle)
                SceneManager.MoveGameObjectToScene(camera.gameObject, _originScene);
        }

        public void MoveTo(Vector3 position, float duration)
        {
            var camera = UpdateCameraReference();
            DOTween.To(
                    () => camera.transform.localPosition,
                    value => camera.transform.localPosition = value,
                    position,
                    duration)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true)
                .SetTarget(camera);
        }

        private Camera UpdateCameraReference()
        {
            if (_camera == null) _camera = Camera.main;
            if (_camera != null && !_originScene.IsValid()) _originScene = _camera.gameObject.scene;
            return _camera;
        }
    }
}
