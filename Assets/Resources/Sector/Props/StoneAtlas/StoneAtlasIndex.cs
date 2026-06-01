using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class StoneAtlasIndex : MonoBehaviour
{
    private const int AtlasColumns = 4;
    private const int AtlasRows = 2;
    private const int MaxIndex = AtlasColumns * AtlasRows - 1;

    private static readonly int AtlasTilingId = Shader.PropertyToID("_AtlasTiling");
    private static readonly int AtlasOffsetId = Shader.PropertyToID("_AtlasOffset");

    [Tooltip("0 = Stone_1 (верх-лево), 7 = Stone_8 (низ-право). Сетка атласа 4x2.")]
    [SerializeField, Range(0, MaxIndex)]
    private int _index;

    [SerializeField]
    private Renderer _renderer;

    private MaterialPropertyBlock _block;

    public int Index
    {
        get => _index;
        set
        {
            _index = Mathf.Clamp(value, 0, MaxIndex);
            Apply();
        }
    }

    private void Reset()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
            _renderer = GetComponentInChildren<Renderer>();
    }

    private void OnEnable() => Apply();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();
        }

        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            Apply();
        };
    }
#endif

    private void Apply()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();
            if (_renderer == null) return;
        }

        _block ??= new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_block);

        var clamped = Mathf.Clamp(_index, 0, MaxIndex);
        var col = clamped % AtlasColumns;
        var row = clamped / AtlasColumns;

        const float tileX = 1f / AtlasColumns;
        const float tileY = 1f / AtlasRows;

        var tiling = new Vector4(tileX, tileY, 0f, 0f);
        var offset = new Vector4(col * tileX, (AtlasRows - 1 - row) * tileY, 0f, 0f);

        _block.SetVector(AtlasTilingId, tiling);
        _block.SetVector(AtlasOffsetId, offset);

        _renderer.SetPropertyBlock(_block);
    }
}
