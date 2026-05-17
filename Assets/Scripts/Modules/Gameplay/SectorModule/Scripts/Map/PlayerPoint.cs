using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    private static readonly int Running = Animator.StringToHash("Running");

    [SerializeField] private float moveSpeed = 4f;

    private Animator _animator;
    private Camera _camera;
    private Coroutine _moveRoutine;
    private float _cameraDistance;

    public bool IsMoving => _moveRoutine != null;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void PlaceAt(Transform target)
    {
        if (target == null) return;

        transform.position = target.position;
        UpdateCameraDistance();
        CenterCamera();
    }

    public void MoveTo(Transform target, System.Action onComplete)
    {
        if (target == null || IsMoving) return;

        _moveRoutine = StartCoroutine(MoveRoutine(target, onComplete));
    }

    private System.Collections.IEnumerator MoveRoutine(Transform target, System.Action onComplete)
    {
        SetRunning(true);

        while (Vector3.Distance(transform.position, target.position) > 0.02f)
        {
            var direction = target.position - transform.position;
            Rotate(direction);
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target.position;
        SetRunning(false);
        _moveRoutine = null;
        onComplete?.Invoke();
    }

    private void LateUpdate()
    {
        CenterCamera();
    }

    private void OnDisable()
    {
        SetRunning(false);
        _moveRoutine = null;
    }

    private void Rotate(Vector3 direction)
    {
        direction.y = 0;
        if (direction.sqrMagnitude <= 0.001f) return;

        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    private void SetRunning(bool value)
    {
        if (_animator != null)
            _animator.SetBool(Running, value);
    }

    private void UpdateCameraDistance()
    {
        if (_camera == null)
            _camera = Camera.main;
        if (_camera == null) return;

        var targetOffset = transform.position - _camera.transform.position;
        _cameraDistance = Mathf.Abs(Vector3.Dot(targetOffset, _camera.transform.forward));
        if (_cameraDistance <= 0.01f)
            _cameraDistance = Vector3.Distance(_camera.transform.position, transform.position);
    }

    private void CenterCamera()
    {
        if (_camera == null)
            _camera = Camera.main;
        if (_camera == null) return;
        if (_cameraDistance <= 0)
            UpdateCameraDistance();

        _camera.transform.position = transform.position - _camera.transform.forward * _cameraDistance;
    }
}
