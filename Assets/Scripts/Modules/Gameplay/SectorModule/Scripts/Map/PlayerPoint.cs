using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    private const float GroundRaycastHeight = 50f;
    private const float GroundRaycastDistance = 200f;
    private static readonly int RunningHash = Animator.StringToHash("Running");

    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _characterRoot;

    private readonly RaycastHit[] _groundHits = new RaycastHit[8];
    private Quaternion _characterRotationOffset = Quaternion.identity;
    private bool _isInitialized;

    private void Awake()
    {
        Initialize();
    }
    
    public void Move(Vector3 position, Vector3 direction)
    {
        Initialize();
        transform.position = SnapToGround(position);
        RotateCharacter(direction);
        if (_animator != null) _animator.SetBool(RunningHash, true);
    }

    public void Stop()
    {
        if (_animator != null) _animator.SetBool(RunningHash, false);
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        if (_characterRoot == null && _animator != null)
        {
            _characterRoot = _animator.transform;
        }

        if (_characterRoot != null)
        {
            _characterRotationOffset = _characterRoot.localRotation;
        }

        _isInitialized = true;
    }

    private void RotateCharacter(Vector3 direction)
    {
        if (_characterRoot == null) return;

        direction.y = 0f;
        if (direction.sqrMagnitude <= 0.001f) return;
        _characterRoot.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up) * _characterRotationOffset;
    }

    private Vector3 SnapToGround(Vector3 position)
    {
        if (TrySnapToTerrain(position, out Vector3 terrainPosition))
        {
            return terrainPosition;
        }

        if (TrySnapToCollider(position, out Vector3 colliderPosition))
        {
            return colliderPosition;
        }

        return position;
    }

    private static bool TrySnapToTerrain(Vector3 position, out Vector3 snappedPosition)
    {
        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            if (terrain == null || terrain.terrainData == null) continue;

            Vector3 terrainPosition = terrain.GetPosition();
            Vector3 terrainSize = terrain.terrainData.size;
            bool insideX = position.x >= terrainPosition.x
                           && position.x <= terrainPosition.x + terrainSize.x;
            bool insideZ = position.z >= terrainPosition.z
                           && position.z <= terrainPosition.z + terrainSize.z;

            if (!insideX || !insideZ) continue;

            float normalizedX = Mathf.InverseLerp(terrainPosition.x, terrainPosition.x + terrainSize.x, position.x);
            float normalizedZ = Mathf.InverseLerp(terrainPosition.z, terrainPosition.z + terrainSize.z, position.z);
            float terrainHeight = terrain.terrainData.GetInterpolatedHeight(normalizedX, normalizedZ) + terrainPosition.y;

            snappedPosition = new Vector3(position.x, terrainHeight, position.z);
            return true;
        }

        snappedPosition = position;
        return false;
    }

    private bool TrySnapToCollider(Vector3 position, out Vector3 snappedPosition)
    {
        Vector3 origin = new Vector3(position.x, position.y + GroundRaycastHeight, position.z);
        int hitCount = Physics.RaycastNonAlloc(
            origin,
            Vector3.down,
            _groundHits,
            GroundRaycastDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        float closestDistance = float.PositiveInfinity;
        snappedPosition = position;
        bool hasGround = false;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = _groundHits[i];
            if (hit.collider == null || hit.transform.IsChildOf(transform)) continue;
            if (hit.distance >= closestDistance) continue;

            closestDistance = hit.distance;
            snappedPosition = hit.point;
            hasGround = true;
        }

        return hasGround;
    }
}
