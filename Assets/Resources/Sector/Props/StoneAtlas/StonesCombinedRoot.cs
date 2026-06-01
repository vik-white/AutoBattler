using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer))]
public sealed class StonesCombinedRoot : MonoBehaviour
{
    private static readonly int AtlasTilingId = Shader.PropertyToID("_AtlasTiling");
    private static readonly int AtlasOffsetId = Shader.PropertyToID("_AtlasOffset");

    [Tooltip("Скольких камней объединили на этом меше.")]
    public int sourceCount;

    [Tooltip("GameObject'ы, которые были отключены инструментом при сборке.\n" +
             "Используется для корректного Restore: возвращаем только их, " +
             "а не всё подряд, что выключено в сцене.")]
    public List<GameObject> disabledOriginals = new List<GameObject>();

#if UNITY_EDITOR
    [Tooltip("Asset-путь объединённого меша (Editor-only).")]
    public string meshAssetPath;
#endif

    private MaterialPropertyBlock _block;
    private Renderer _cachedRenderer;

    private void OnEnable()
    {
        // UV уже запечены в меш в нужные регионы атласа — здесь
        // выставляем «единичные» tiling/offset, чтобы шейдер не сдвигал
        // координаты повторно. MaterialPropertyBlock не сериализуется,
        // поэтому при каждом старте Play Mode (и любом OnEnable) ставим заново.
        ApplyIdentityAtlas();
    }

    private void ApplyIdentityAtlas()
    {
        if (_cachedRenderer == null)
            _cachedRenderer = GetComponent<Renderer>();
        if (_cachedRenderer == null) return;

        _block ??= new MaterialPropertyBlock();
        _cachedRenderer.GetPropertyBlock(_block);
        _block.SetVector(AtlasTilingId, new Vector4(1f, 1f, 0f, 0f));
        _block.SetVector(AtlasOffsetId, new Vector4(0f, 0f, 0f, 0f));
        _cachedRenderer.SetPropertyBlock(_block);
    }
}
