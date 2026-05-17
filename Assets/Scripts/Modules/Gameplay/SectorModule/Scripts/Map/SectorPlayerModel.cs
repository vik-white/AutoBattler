using UnityEngine;

namespace vikwhite
{
    public interface ISectorPlayerModel
    {
        Vector3 Position { get; }
        bool IsMoving { get; }
        void SetMoveSpeed(float value);
        void PlaceAt(Vector3 position);
        bool MoveTo(Vector3 position);
        bool Update(float deltaTime);
    }

    public class SectorPlayerModel : ISectorPlayerModel
    {
        private const float CompleteDistance = 0.02f;

        private Vector3 _position;
        private Vector3 _targetPosition;
        private float _moveSpeed = 4f;
        private bool _hasPosition;

        public Vector3 Position => _position;
        public bool IsMoving { get; private set; }

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
        }

        public bool MoveTo(Vector3 position)
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
            return true;
        }
    }
}
