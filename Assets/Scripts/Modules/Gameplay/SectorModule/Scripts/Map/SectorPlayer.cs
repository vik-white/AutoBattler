using UnityEngine;
using UnityEngine.Events;

namespace vikwhite
{
    public class SectorPlayer
    {
        private const float CompleteDistance = 0.02f;
        private const float MoveSpeed = 4f;

        private Vector3 _position;
        private Vector3 _direction = Vector3.forward;
        private BezierPath _path;
        private float _pathDistance;
        private float _pathLength;

        public bool IsMoving { get; private set; }
        public event UnityAction<Vector3, Vector3> OnMove;
        public event UnityAction OnStop;
        public Vector3 Position => _position;

        public void Move(BezierPath path)
        {
            _path = path;
            _pathDistance = 0f;
            _pathLength = GetPathTravelLength(path);
            ApplySample(GetPathSample(0f));
            if (_pathLength <= CompleteDistance) StopAt(GetPathSample(_pathLength));
            IsMoving = true;
            OnMove?.Invoke(_position, _direction);
        }
        
        public void Update(float deltaTime)
        {
            if (!IsMoving) return;
            UpdatePathMovement(deltaTime);
        }

        private void UpdatePathMovement(float deltaTime)
        {
            _pathDistance = Mathf.Min(_pathDistance + MoveSpeed * deltaTime, _pathLength);

            if (_pathLength - _pathDistance <= CompleteDistance)
            {
                StopAt(GetPathSample(_pathLength));
                return;
            }

            ApplySample(GetPathSample(_pathDistance));
            OnMove?.Invoke(_position, _direction);
        }

        private void StopAt(BezierPathSample sample)
        {
            ApplySample(sample);
            IsMoving = false;
            ClearPath();
            OnMove?.Invoke(_position, _direction);
            OnStop?.Invoke();
        }

        private void ClearPath()
        {
            _path = null;
            _pathDistance = 0f;
            _pathLength = 0f;
        }

        private BezierPathSample GetPathSample(float distance) => _path.GetSampleAtDistance(distance);

        private void ApplySample(BezierPathSample sample)
        {
            _position = sample.Position;
            Vector3 direction = sample.Tangent;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f) _direction = direction.normalized;
        }

        private static float GetPathTravelLength(BezierPath path)
        {
            if (!path.Closed) return path.GetLength();
            float finalPointTime = path.PointCount <= 1 ? 0f : (path.PointCount - 1f) / path.SegmentCount;
            return path.GetDistanceAt(finalPointTime);
        }

    }
}
