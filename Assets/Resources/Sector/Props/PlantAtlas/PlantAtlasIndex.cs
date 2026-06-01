using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Помечает Quad конкретного префаба Palm/Plant и подменяет ему меш на
/// уникальный 4-вершинный quad с UV, запечёнными под нужный сектор общего
/// атласа 4×4. Запускается и в редакторе ([ExecuteAlways]), и в рантайме.
///
/// Сами меши не сохраняются в ассеты — они генерируются в памяти и
/// шарятся между всеми инстансами одного индекса через статический кэш.
/// Это позволило не вшивать UV в built-in Quad (он один на всю сцену) и
/// при этом обойтись без отдельных .asset файлов на каждый растение/пальму.
///
/// MaterialPropertyBlock сознательно НЕ используется: ветровые параметры
/// уже зашиты в материал (palm/plant preset), а MPB сломал бы SRP Batcher
/// на Wind.shadergraph.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class PlantAtlasIndex : MonoBehaviour
{
    public const int AtlasColumns = 4;
    public const int AtlasRows    = 4;
    public const int MaxIndex     = AtlasColumns * AtlasRows - 1;

    /// <summary>Палмовый preset (Palm_1..5, Plant_9) или растительный (всё остальное).</summary>
    public enum WindPreset
    {
        Plant = 0,
        Palm  = 1,
    }

    [Tooltip("Порядковый номер в атласе Palm/Plant (0..15).\n" +
             "0..4 — Palm_1..5, 5..15 — Plant_1..11.")]
    [SerializeField, Range(0, MaxIndex)]
    private int _index;

    [Tooltip("Каким preset-ом материала помечен этот префаб. Только для инструментов.")]
    [SerializeField]
    private WindPreset _preset = WindPreset.Plant;

    [Tooltip("MeshFilter, которому подменяем меш. Если не задан — ищем на этом GameObject.")]
    [SerializeField]
    private MeshFilter _meshFilter;

    public int Index
    {
        get => _index;
        set { _index = Mathf.Clamp(value, 0, MaxIndex); ApplyMesh(); }
    }

    public WindPreset Preset
    {
        get => _preset;
        set => _preset = value;
    }

    public int AtlasColumn => _index % AtlasColumns;
    public int AtlasRow    => _index / AtlasColumns;

    // Один меш на индекс, шарится между всеми инстансами.
    private static readonly Dictionary<int, Mesh> Cache = new Dictionary<int, Mesh>(16);

    private void OnEnable() => ApplyMesh();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!isActiveAndEnabled) return;
        ApplyMesh();
    }
#endif

    private void ApplyMesh()
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter == null)
                _meshFilter = GetComponentInChildren<MeshFilter>();
            if (_meshFilter == null) return;
        }

        var clamped = Mathf.Clamp(_index, 0, MaxIndex);
        var mesh    = GetOrCreateMesh(clamped);
        if (_meshFilter.sharedMesh != mesh)
            _meshFilter.sharedMesh = mesh;
    }

    private static Mesh GetOrCreateMesh(int index)
    {
        if (Cache.TryGetValue(index, out var mesh) && mesh != null)
            return mesh;

        var col = index % AtlasColumns;
        var row = index / AtlasColumns;

        // row=0 — верх атласа. В UV-пространстве V растёт вверх, поэтому верхняя
        // строка атласа отображается в V = (Rows-1)/Rows .. 1.
        var uMin = (float)col / AtlasColumns;
        var uMax = (float)(col + 1) / AtlasColumns;
        var vMax = (float)(AtlasRows - row) / AtlasRows;
        var vMin = (float)(AtlasRows - row - 1) / AtlasRows;

        mesh = new Mesh
        {
            name = $"PlantAtlasQuad_{index}",
            hideFlags = HideFlags.HideAndDontSave,
        };
        mesh.vertices = new[]
        {
            new Vector3(-0.5f, -0.5f, 0f),
            new Vector3( 0.5f, -0.5f, 0f),
            new Vector3(-0.5f,  0.5f, 0f),
            new Vector3( 0.5f,  0.5f, 0f),
        };
        mesh.normals = new[]
        {
            new Vector3(0f, 0f, -1f),
            new Vector3(0f, 0f, -1f),
            new Vector3(0f, 0f, -1f),
            new Vector3(0f, 0f, -1f),
        };
        mesh.tangents = new[]
        {
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
        };
        mesh.uv = new[]
        {
            new Vector2(uMin, vMin),
            new Vector2(uMax, vMin),
            new Vector2(uMin, vMax),
            new Vector2(uMax, vMax),
        };
        // Триангуляция как у стандартного Unity Quad (PrimitiveType.Quad).
        mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
        mesh.RecalculateBounds();
        mesh.UploadMeshData(false);

        Cache[index] = mesh;
        return mesh;
    }
}
