using UnityEngine;

namespace vikwhite
{
    public class SectorPlayer
    {
        private const float CompleteDistance = 0.02f;

        private readonly ISectorService _sector;
        private Vector3 _position;
        private Vector3 _targetPosition;
        private string _movingLocation;
        private float _moveSpeed = 4f;
        private bool _hasPosition;

        public Vector3 Position => _position;
        public bool IsMoving { get; private set; }
        public event System.Action Changed;

        public SectorPlayer(ISectorService sector)
        {
            _sector = sector;
            _sector.Changed += OnSectorChanged;
        }

        public void SetMoveSpeed(float value)
        {
            _moveSpeed = Mathf.Max(0.01f, value);
        }

        public void PlaceAt(Vector3 position)
        {
            _position = position;
            _targetPosition = position;
            _hasPosition = true;
            IsMoving = false;
            _movingLocation = string.Empty;
            Changed?.Invoke();
        }

        public void MoveToNextLocation()
        {
            if (IsMoving) return;
            if (!_sector.TryGetNextLocation(out var locationID, out var position)) return;

            _movingLocation = locationID;
            var completed = MoveTo(position);

            if (completed)
                CompleteMove();
            else
                Changed?.Invoke();
        }

        private bool MoveTo(Vector3 position)
        {
            if (IsMoving) return false;
            if (!_hasPosition)
            {
                PlaceAt(position);
                return true;
            }

            _targetPosition = position;
            if (Vector3.Distance(_position, _targetPosition) <= CompleteDistance)
            {
                _position = _targetPosition;
                IsMoving = false;
                return true;
            }

            IsMoving = true;
            return false;
        }

        public bool Update(float deltaTime)
        {
            if (!IsMoving) return false;

            _position = Vector3.MoveTowards(_position, _targetPosition, _moveSpeed * deltaTime);
            if (Vector3.Distance(_position, _targetPosition) > CompleteDistance) return false;

            _position = _targetPosition;
            IsMoving = false;
            CompleteMove();
            return true;
        }

        private void CompleteMove()
        {
            if (string.IsNullOrEmpty(_movingLocation)) return;

            var completedLocation = _movingLocation;
            _movingLocation = string.Empty;
            _sector.SetCurrentLocation(completedLocation);
        }

        private void OnSectorChanged()
        {
            Changed?.Invoke();
        }
    }
}
