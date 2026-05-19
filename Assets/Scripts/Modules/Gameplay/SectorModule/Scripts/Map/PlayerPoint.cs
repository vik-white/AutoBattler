using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    public void Move(Vector3 position)
    {
        transform.position = position;
        Rotate(position - transform.position);
        _animator.SetBool(Animator.StringToHash("Running"), true);
    }

    public void Stop()
    {
        _animator.SetBool(Animator.StringToHash("Running"), false);
    }

    private void Rotate(Vector3 direction)
    {
        direction.y = 0;
        if (direction.sqrMagnitude <= 0.001f) return;
        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }
}
