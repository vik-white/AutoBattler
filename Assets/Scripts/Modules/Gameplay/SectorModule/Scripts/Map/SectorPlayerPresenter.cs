using UnityEngine;

namespace vikwhite
{
    public interface ISectorPlayerPresenter
    {
        void Initialize();
        void Release();
    }

    public class SectorPlayerPresenter : ISectorPlayerPresenter
    {
        private readonly ISectorPlayerModel _model;
        private readonly ICameraService _camera;
        private global::PlayerPoint _view;

        public SectorPlayerPresenter(ISectorPlayerModel model, ICameraService camera)
        {
            _model = model;
            _camera = camera;
        }

        public void Initialize()
        {
            Release();

            _view = Object.FindAnyObjectByType<global::PlayerPoint>(FindObjectsInactive.Include);
            if (_view == null)
            {
                Debug.LogWarning("PlayerPoint was not found on sector scene.");
                return;
            }

            _model.SetMoveSpeed(_view.Speed);
            _camera.Follow(_view.transform);
            _model.Changed += OnModelChanged;
            if (_model.HasPosition)
                OnModelChanged();
        }

        public void Release()
        {
            _model.Changed -= OnModelChanged;
            _camera.Release();
            _view = null;
        }

        private void OnModelChanged()
        {
            if (_view == null) return;

            _view.ApplyState(_model.Position, _model.IsMoving);
            _camera.Center();
        }
    }
}
