using System;
using UnityEngine;

namespace vikwhite
{
    public interface ISectorPlayerModel
    {
        Vector3 Position { get; }
        bool HasPosition { get; }
        bool IsMoving { get; }
        event Action Changed;
        event Action MovementCompleted;
        void SetMoveSpeed(float value);
        void PlaceAt(Vector3 position);
        void MoveTo(Vector3 position);
    }

    public class SectorPlayerModel : ISectorPlayerModel, IUpdatable
    {
        private const float CompleteDistance = 0.02f;

        private Vector3 _position;
        private Vector3 _targetPosition;
        private float _moveSpeed = 4f;

        public Vector3 Position => _position;
        public bool HasPosition { get; private set; }
        public bool IsMoving { get; private set; }
        public event Action Changed;
        public event Action MovementCompleted;

        public void SetMoveSpeed(float value)
        {
            _moveSpeed = Mathf.Max(0.01f, value);
        }

        public void PlaceAt(Vector3 position)
        {
            _position = position;
            _targetPosition = position;
            HasPosition = true;
            IsMoving = false;
            Changed?.Invoke();
        }

        public void MoveTo(Vector3 position)
        {
            if (IsMoving) return;
            if (!HasPosition)
            {
                PlaceAt(position);
                MovementCompleted?.Invoke();
                return;
            }

            _targetPosition = position;
            if (Vector3.Distance(_position, _targetPosition) <= CompleteDistance)
            {
                _position = _targetPosition;
                Changed?.Invoke();
                MovementCompleted?.Invoke();
                return;
            }

            IsMoving = true;
            Changed?.Invoke();
        }

        public void Update()
        {
            if (!IsMoving) return;

            _position = Vector3.MoveTowards(_position, _targetPosition, _moveSpeed * Time.deltaTime);
            if (Vector3.Distance(_position, _targetPosition) > CompleteDistance)
            {
                Changed?.Invoke();
                return;
            }

            _position = _targetPosition;
            IsMoving = false;
            Changed?.Invoke();
            MovementCompleted?.Invoke();
        }
    }
}
