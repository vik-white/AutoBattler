using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speed = 4f;

    public float Speed => _speed;
    
    public void ApplyState(Vector3 position, bool isMoving)
    {
        Rotate(position - transform.position);
        transform.position = position;
        _animator.SetBool(Animator.StringToHash("Running"), isMoving);
    }

    private void Rotate(Vector3 direction)
    {
        direction.y = 0;
        if (direction.sqrMagnitude <= 0.001f) return;
        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }
}
