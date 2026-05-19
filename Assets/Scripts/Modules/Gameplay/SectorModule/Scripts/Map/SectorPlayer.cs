using UnityEngine;
using UnityEngine.Events;

namespace vikwhite
{
    public class SectorPlayer
    {
        private const float CompleteDistance = 0.02f;
        private const float MoveSpeed = 4f;

        private Vector3 _position;
        private Vector3 _targetPosition;

        public bool IsMoving { get; private set; }
        public event UnityAction<Vector3> OnMove;
        public event UnityAction OnStop;
        public Vector3 Position => _position;

        public SectorPlayer(Vector3 position)
        {
            _position = position;
            _targetPosition = position;
            IsMoving = false;
        }

        public void Move(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            IsMoving = true;
        }
        
        public void Update(float deltaTime)
        {
            if (!IsMoving) return;
            _position = Vector3.MoveTowards(_position, _targetPosition, MoveSpeed * deltaTime);
            if (Vector3.Distance(_position, _targetPosition) <= CompleteDistance)
            {
                _position = _targetPosition;
                IsMoving = false;
                OnStop?.Invoke();
            }
            else
            {
                OnMove?.Invoke(_position);
            }
        }
    }
}