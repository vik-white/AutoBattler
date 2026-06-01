using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Объединяет все растения и пальмы на активной сцене (через
/// <see cref="PlantAtlasIndex"/>) в один-два больших меша — отдельно для
/// palm-preset и plant-preset материалов. Оригиналы не удаляются, а просто
/// выключаются (как у камней и травы).
///
/// Результат:
///   PlantsCombined (PlantsCombinedRoot)
///     ├─ Palms   (MeshRenderer = PlantAtlas_Palm.mat,  combined mesh)
///     └─ Plants  (MeshRenderer = PlantAtlas_Plant.mat, combined mesh)
///
/// UV под нужный сектор атласа берутся не из <c>MeshFilter.sharedMesh</c>
/// (там лежит runtime-генерируемый mesh с <c>HideAndDontSave</c>), а
/// строятся заново по <see cref="PlantAtlasIndex.Index"/> — так у инструмента
/// нет зависимости от состояния OnEnable у компонента.
/// </summary>
internal static class PlantsCombiner
{
    private const string MenuCombine = "Tools/Plants/Combine On Map";
    private const string MenuRestore = "Tools/Plants/Restore Originals";

    private const string CombinedRootName = "PlantsCombined";
    private const string PalmChildName    = "Palms";
    private const string PlantChildName   = "Plants";

    private const string UndoLabelCombine = "Combine Plants On Map";
    private const string UndoLabelRestore = "Restore Plants";

    private const string PalmMatPath  = "Assets/Resources/Sector/Props/PlantAtlas/PlantAtlas_Palm.mat";
    private const string PlantMatPath = "Assets/Resources/Sector/Props/PlantAtlas/PlantAtlas_Plant.mat";

    private const string FallbackPalmAssetPath  = "Assets/Resources/Sector/Props/PlantAtlas/PlantsCombined_Palm.asset";
    private const string FallbackPlantAssetPath = "Assets/Resources/Sector/Props/PlantAtlas/PlantsCombined_Plant.asset";

    private const int AtlasColumns = 4;
    private const int AtlasRows    = 4;
    private const int MaxAtlasIndex = AtlasColumns * AtlasRows - 1;

    [MenuItem(MenuCombine, priority = 102)]
    private static void CombineOnMap()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            EditorUtility.DisplayDialog("Combine Plants", "Нет активной сцены.", "OK");
            return;
        }

        var palmMat  = AssetDatabase.LoadAssetAtPath<Material>(PalmMatPath);
        var plantMat = AssetDatabase.LoadAssetAtPath<Material>(PlantMatPath);
        if (palmMat == null || plantMat == null)
        {
            EditorUtility.DisplayDialog("Combine Plants",
                $"Не нашёл общие материалы:\n{PalmMatPath}\n{PlantMatPath}", "OK");
            return;
        }

        Undo.IncrementCurrentGroup();
        var undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(UndoLabelCombine);

        // Если уже есть объединённый корень — сначала восстанавливаем «своих» оригиналов
        // и удаляем старый combined, чтобы пересобрать с нуля.
        RestoreDisabledFromMarkers(UndoLabelCombine);

        var plants = Object.FindObjectsByType<PlantAtlasIndex>(FindObjectsInactive.Exclude);
        if (plants.Length == 0)
        {
            Undo.CollapseUndoOperations(undoGroup);
            EditorUtility.DisplayDialog("Combine Plants",
                "На активной сцене не найдено активных PlantAtlasIndex.",
                "OK");
            return;
        }

        var palmInstances  = new List<CombineInstance>();
        var plantInstances = new List<CombineInstance>();
        var tempMeshes     = new List<Mesh>();
        var disabledRoots  = new List<GameObject>();
        var validPlants    = new List<PlantAtlasIndex>(plants.Length);

        try
        {
            foreach (var p in plants)
            {
                if (p == null) continue;

                var mf = p.GetComponent<MeshFilter>();
                var mr = p.GetComponent<MeshRenderer>();
                if (mf == null || mr == null) continue;

                var src = BuildQuadForIndex(p.Index);
                tempMeshes.Add(src);

                var ci = new CombineInstance
                {
                    mesh         = src,
                    transform    = mr.transform.localToWorldMatrix,
                    subMeshIndex = 0,
                };

                if (p.Preset == PlantAtlasIndex.WindPreset.Palm)
                    palmInstances.Add(ci);
                else
                    plantInstances.Add(ci);

                validPlants.Add(p);
            }

            if (palmInstances.Count == 0 && plantInstances.Count == 0)
            {
                EditorUtility.DisplayDialog("Combine Plants",
                    "Не нашлось ни одного валидного MeshFilter+MeshRenderer на растениях.",
                    "OK");
                Undo.CollapseUndoOperations(undoGroup);
                return;
            }

            var rootGo = GetOrCreateRoot(out var rootCreated);
            if (rootCreated)
                Undo.RegisterCreatedObjectUndo(rootGo, UndoLabelCombine);
            else
                Undo.RecordObject(rootGo, UndoLabelCombine);
            rootGo.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            rootGo.transform.localScale = Vector3.one;

            var marker = rootGo.GetComponent<PlantsCombinedRoot>();
            if (marker == null) marker = Undo.AddComponent<PlantsCombinedRoot>(rootGo);
            else Undo.RecordObject(marker, UndoLabelCombine);
            marker.disabledOriginals.Clear();
            marker.palmMeshAssetPath  = null;
            marker.plantMeshAssetPath = null;

            var palmsChild  = SetupChild(rootGo, PalmChildName,  palmInstances,  palmMat,  scene,
                                         FallbackPalmAssetPath,  isPalm: true,
                                         out var palmCombined,   out var palmAsset);
            var plantsChild = SetupChild(rootGo, PlantChildName, plantInstances, plantMat, scene,
                                         FallbackPlantAssetPath, isPalm: false,
                                         out var plantCombined,  out var plantAsset);

            if (palmsChild != null)  marker.palmMeshAssetPath  = palmAsset;
            if (plantsChild != null) marker.plantMeshAssetPath = plantAsset;

            // Отключаем оригиналы префабов.
            foreach (var p in validPlants)
            {
                if (p == null) continue;
                var root = ResolvePlantRoot(p.transform);
                if (root == null || !root.activeSelf) continue;

                Undo.RecordObject(root, UndoLabelCombine);
                root.SetActive(false);
                disabledRoots.Add(root);
                marker.disabledOriginals.Add(root);
            }

            marker.sourceCount = validPlants.Count;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorUtility.SetDirty(rootGo);
            Selection.activeGameObject = rootGo;

            Debug.Log($"[PlantsCombiner] Combined: " +
                      $"palms={palmInstances.Count} ({DescribeMesh(palmCombined)}), " +
                      $"plants={plantInstances.Count} ({DescribeMesh(plantCombined)}). " +
                      $"Disabled {disabledRoots.Count} originals.");
        }
        finally
        {
            // Чистим временные «исходные» quad'ы — их вершины уже скопированы в combined.
            CleanupTempMeshes(tempMeshes);
            Undo.CollapseUndoOperations(undoGroup);
        }
    }

    [MenuItem(MenuRestore, priority = 103)]
    private static void RestoreOriginals()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            EditorUtility.DisplayDialog("Restore Plants", "Нет активной сцены.", "OK");
            return;
        }

        Undo.IncrementCurrentGroup();
        var undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(UndoLabelRestore);

        var enabled = RestoreDisabledFromMarkers(UndoLabelRestore);

        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[PlantsCombiner] Restored {enabled} originals, removed combined roots.");
    }

    [MenuItem(MenuCombine, validate = true)]
    private static bool CombineOnMapValidate() => SceneManager.GetActiveScene().IsValid();

    [MenuItem(MenuRestore, validate = true)]
    private static bool RestoreOriginalsValidate()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid()) return false;
        return Object.FindAnyObjectByType<PlantsCombinedRoot>(FindObjectsInactive.Include) != null;
    }

    /// <summary>
    /// Возвращает в active растения, выключенные инструментом (по маркеру),
    /// и удаляет все <see cref="PlantsCombinedRoot"/> со сцены.
    /// </summary>
    private static int RestoreDisabledFromMarkers(string undoLabel)
    {
        var enabledCount = 0;
        var roots = Object.FindObjectsByType<PlantsCombinedRoot>(FindObjectsInactive.Include);

        foreach (var rootMarker in roots)
        {
            if (rootMarker == null) continue;

            foreach (var go in rootMarker.disabledOriginals)
            {
                if (go == null || go.activeSelf) continue;
                Undo.RecordObject(go, undoLabel);
                go.SetActive(true);
                enabledCount++;
            }

            Undo.DestroyObjectImmediate(rootMarker.gameObject);
        }

        return enabledCount;
    }

    /// <summary>
    /// Собирает CombineInstance-список в один меш под отдельный child GameObject,
    /// сохраняет mesh-ассет и настраивает MeshRenderer.
    /// Возвращает созданный child (или null, если в группе пусто).
    /// </summary>
    private static GameObject SetupChild(
        GameObject parent,
        string childName,
        List<CombineInstance> instances,
        Material material,
        Scene scene,
        string fallbackAssetPath,
        bool isPalm,
        out Mesh combinedMesh,
        out string combinedAssetPath)
    {
        combinedMesh      = null;
        combinedAssetPath = null;

        if (instances.Count == 0)
        {
            // Если ребёнок уже был от прошлой сборки — удалим, чтобы не висел пустой.
            var existing = parent.transform.Find(childName);
            if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);
            return null;
        }

        var fresh = new Mesh
        {
            name        = $"{CombinedRootName}_{(isPalm ? "Palm" : "Plant")}",
            indexFormat = IndexFormat.UInt32,
        };
        fresh.CombineMeshes(instances.ToArray(), mergeSubMeshes: true, useMatrices: true);
        fresh.RecalculateBounds();

        combinedAssetPath = ResolveCombinedMeshPath(scene, fallbackAssetPath, isPalm);
        SaveCombinedMesh(fresh, combinedAssetPath, out combinedMesh);

        var childTr = parent.transform.Find(childName);
        GameObject child;
        bool childCreated;
        if (childTr == null)
        {
            child = new GameObject(childName);
            Undo.RegisterCreatedObjectUndo(child, UndoLabelCombine);
            child.transform.SetParent(parent.transform, worldPositionStays: false);
            childCreated = true;
        }
        else
        {
            child = childTr.gameObject;
            Undo.RecordObject(child, UndoLabelCombine);
            childCreated = false;
        }

        child.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        child.transform.localScale = Vector3.one;

        var mf = child.GetComponent<MeshFilter>();
        if (mf == null) mf = Undo.AddComponent<MeshFilter>(child);
        else Undo.RecordObject(mf, UndoLabelCombine);
        mf.sharedMesh = combinedMesh;

        var mr = child.GetComponent<MeshRenderer>();
        if (mr == null) mr = Undo.AddComponent<MeshRenderer>(child);
        else Undo.RecordObject(mr, UndoLabelCombine);
        mr.sharedMaterial    = material;
        mr.shadowCastingMode = ShadowCastingMode.On;
        mr.receiveShadows    = true;

        if (!childCreated)
            EditorUtility.SetDirty(child);

        return child;
    }

    /// <summary>
    /// Строит 4-вершинный quad с UV под нужный сектор атласа 4×4.
    /// Геометрия/нормали/тангенты — как у стандартного Unity Quad
    /// и как у меша, который генерит <see cref="PlantAtlasIndex"/> в OnEnable.
    /// </summary>
    private static Mesh BuildQuadForIndex(int index)
    {
        index = Mathf.Clamp(index, 0, MaxAtlasIndex);
        var col = index % AtlasColumns;
        var row = index / AtlasColumns;

        var uMin = (float)col / AtlasColumns;
        var uMax = (float)(col + 1) / AtlasColumns;
        var vMax = (float)(AtlasRows - row) / AtlasRows;
        var vMin = (float)(AtlasRows - row - 1) / AtlasRows;

        var mesh = new Mesh { name = $"PlantQuad_{index}_temp" };
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
        mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
        return mesh;
    }

    private static string ResolveCombinedMeshPath(Scene scene, string fallback, bool isPalm)
    {
        if (string.IsNullOrEmpty(scene.path)) return fallback;

        var dir       = Path.GetDirectoryName(scene.path)?.Replace('\\', '/');
        var sceneName = Path.GetFileNameWithoutExtension(scene.path);
        if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(sceneName)) return fallback;

        var suffix = isPalm ? "Palm" : "Plant";
        return $"{dir}/{sceneName}_PlantsCombined_{suffix}.asset";
    }

    private static void SaveCombinedMesh(Mesh fresh, string assetPath, out Mesh result)
    {
        var dir = Path.GetDirectoryName(assetPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
        if (existing != null)
        {
            EditorUtility.CopySerialized(fresh, existing);
            Object.DestroyImmediate(fresh);
            result = existing;
        }
        else
        {
            AssetDatabase.CreateAsset(fresh, assetPath);
            result = fresh;
        }
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(result);
    }

    private static void CleanupTempMeshes(List<Mesh> meshes)
    {
        foreach (var m in meshes)
        {
            if (m != null) Object.DestroyImmediate(m);
        }
        meshes.Clear();
    }

    private static GameObject GetOrCreateRoot(out bool wasCreated)
    {
        var existing = Object.FindAnyObjectByType<PlantsCombinedRoot>(FindObjectsInactive.Include);
        if (existing != null)
        {
            wasCreated = false;
            return existing.gameObject;
        }

        wasCreated = true;
        return new GameObject(CombinedRootName);
    }

    /// <summary>
    /// Корень префаба, который надо отключить. Для инстанса префаба возвращает
    /// outermost prefab root; для обычной иерархии — родитель Mesh-объекта.
    /// </summary>
    private static GameObject ResolvePlantRoot(Transform meshTransform)
    {
        if (meshTransform == null) return null;

        var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(meshTransform.gameObject);
        if (prefabRoot != null) return prefabRoot;

        return meshTransform.parent != null ? meshTransform.parent.gameObject : meshTransform.gameObject;
    }

    private static string DescribeMesh(Mesh m) =>
        m == null ? "—" : $"verts={m.vertexCount}, tris={m.triangles.Length / 3}";
}
