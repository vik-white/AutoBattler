using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Валидатор сетапа атласа Palm/Plant: проверяет, что все 16 префабов
/// (<c>Palm_1..5</c>, <c>Plant_1..11</c>) подключены к общему атласу через
/// один из двух материалов (<c>PlantAtlas_Palm.mat</c> /
/// <c>PlantAtlas_Plant.mat</c>) и имеют корректно настроенный
/// <see cref="PlantAtlasIndex"/>.
///
/// Меши под сектор атласа сейчас не лежат в виде ассетов — их генерит сам
/// <see cref="PlantAtlasIndex"/> в <c>OnEnable</c>, поэтому отдельного
/// шага «build meshes» больше нет.
/// </summary>
public static class PlantAtlasBuilder
{
    private const string RootDir      = "Assets/Resources/Sector/Props";
    private const string PalmMatPath  = "Assets/Resources/Sector/Props/PlantAtlas/PlantAtlas_Palm.mat";
    private const string PlantMatPath = "Assets/Resources/Sector/Props/PlantAtlas/PlantAtlas_Plant.mat";

    /// <summary>Описание одного префаба: имя и preset (Palm/Plant).</summary>
    private readonly struct Entry
    {
        public readonly string Name;
        public readonly string PrefabPath;
        public readonly PlantAtlasIndex.WindPreset Preset;
        public readonly int Index;

        public Entry(string name, PlantAtlasIndex.WindPreset preset, int index)
        {
            Name       = name;
            PrefabPath = $"{RootDir}/{name}/{name}.prefab";
            Preset     = preset;
            Index      = index;
        }
    }

    // Порядок ДОЛЖЕН совпадать с раскладкой атласа 4×4.
    // Plant_9 исторически использовал палмовый _Param=(0.3, 0.1, 10, 1).
    private static readonly Entry[] Entries = new[]
    {
        new Entry("Palm_1",  PlantAtlasIndex.WindPreset.Palm,  0),
        new Entry("Palm_2",  PlantAtlasIndex.WindPreset.Palm,  1),
        new Entry("Palm_3",  PlantAtlasIndex.WindPreset.Palm,  2),
        new Entry("Palm_4",  PlantAtlasIndex.WindPreset.Palm,  3),
        new Entry("Palm_5",  PlantAtlasIndex.WindPreset.Palm,  4),

        new Entry("Plant_1", PlantAtlasIndex.WindPreset.Plant, 5),
        new Entry("Plant_2", PlantAtlasIndex.WindPreset.Plant, 6),
        new Entry("Plant_3", PlantAtlasIndex.WindPreset.Plant, 7),
        new Entry("Plant_4", PlantAtlasIndex.WindPreset.Plant, 8),
        new Entry("Plant_5", PlantAtlasIndex.WindPreset.Plant, 9),
        new Entry("Plant_6", PlantAtlasIndex.WindPreset.Plant, 10),
        new Entry("Plant_7", PlantAtlasIndex.WindPreset.Plant, 11),
        new Entry("Plant_8", PlantAtlasIndex.WindPreset.Plant, 12),
        new Entry("Plant_9", PlantAtlasIndex.WindPreset.Palm,  13),
        new Entry("Plant_10",PlantAtlasIndex.WindPreset.Plant, 14),
        new Entry("Plant_11",PlantAtlasIndex.WindPreset.Plant, 15),
    };

    [MenuItem("Tools/Plants/Validate Setup")]
    public static void ValidateSetup()
    {
        var palmMat  = AssetDatabase.LoadAssetAtPath<Material>(PalmMatPath);
        var plantMat = AssetDatabase.LoadAssetAtPath<Material>(PlantMatPath);
        if (palmMat == null || plantMat == null)
        {
            EditorUtility.DisplayDialog("Plant Atlas Builder",
                $"Не нашёл общие материалы:\n{PalmMatPath}\n{PlantMatPath}", "OK");
            return;
        }

        var ok  = new List<string>();
        var bad = new List<string>();

        foreach (var entry in Entries)
        {
            var expected = entry.Preset == PlantAtlasIndex.WindPreset.Palm ? palmMat : plantMat;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.PrefabPath);
            if (prefab == null) { bad.Add($"{entry.Name}: prefab not found"); continue; }

            var meshGo = FindMeshChild(prefab);
            if (meshGo == null) { bad.Add($"{entry.Name}: 'Mesh' GameObject not found"); continue; }

            var mr  = meshGo.GetComponent<MeshRenderer>();
            var idx = meshGo.GetComponent<PlantAtlasIndex>();

            if (mr == null || mr.sharedMaterial != expected)
                bad.Add($"{entry.Name}: material != {expected.name}");
            else if (idx == null)
                bad.Add($"{entry.Name}: PlantAtlasIndex missing");
            else if (idx.Index != entry.Index)
                bad.Add($"{entry.Name}: PlantAtlasIndex.Index={idx.Index}, expected {entry.Index}");
            else if (idx.Preset != entry.Preset)
                bad.Add($"{entry.Name}: PlantAtlasIndex.Preset={idx.Preset}, expected {entry.Preset}");
            else
                ok.Add(entry.Name);
        }

        Debug.Log($"[PlantAtlasBuilder] OK: {ok.Count}/{Entries.Length}" +
                  (bad.Count > 0 ? "\nIssues:\n" + string.Join("\n", bad) : ""));
    }

    private static GameObject FindMeshChild(GameObject root)
    {
        for (var i = 0; i < root.transform.childCount; i++)
        {
            var child = root.transform.GetChild(i);
            if (child.name == "Mesh" && child.GetComponent<MeshFilter>() != null)
                return child.gameObject;
        }
        return null;
    }
}
