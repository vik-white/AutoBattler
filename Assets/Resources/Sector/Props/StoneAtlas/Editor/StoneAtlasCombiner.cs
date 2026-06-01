using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

internal static class StoneAtlasCombiner
{
    private const string MenuCombine = "Tools/Stones/Combine On Map";
    private const string MenuRestore = "Tools/Stones/Restore Originals";
    private const string CombinedGameObjectName = "StonesCombined";
    private const string UndoLabelCombine = "Combine Stones On Map";
    private const string UndoLabelRestore = "Restore Stones";
    private const string FallbackAssetPath = "Assets/Resources/Sector/Props/StoneAtlas/StonesCombined.asset";

    private const int AtlasColumns = 4;
    private const int AtlasRows = 2;

    [MenuItem(MenuCombine, priority = 100)]
    private static void CombineOnMap()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            EditorUtility.DisplayDialog("Combine Stones", "Нет активной сцены.", "OK");
            return;
        }

        Undo.IncrementCurrentGroup();
        var undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(UndoLabelCombine);

        // Если уже есть объединённый объект — сначала восстанавливаем «свои» отключённые камни
        // и удаляем старый combined, чтобы пересобрать с нуля
        RestoreDisabledFromMarkers(UndoLabelCombine);

        var stones = Object.FindObjectsByType<StoneAtlasIndex>(FindObjectsInactive.Exclude);
        if (stones.Length == 0)
        {
            Undo.CollapseUndoOperations(undoGroup);
            EditorUtility.DisplayDialog("Combine Stones",
                "На активной сцене не найдено активных StoneAtlasIndex.",
                "OK");
            return;
        }

        Material sharedMat = null;
        var combineInstances = new List<CombineInstance>(stones.Length);
        var bakedMeshes = new List<Mesh>(stones.Length);
        var disabledRoots = new List<GameObject>(stones.Length);

        foreach (var stone in stones)
        {
            if (stone == null) continue;

            var mr = stone.GetComponent<MeshRenderer>();
            var mf = stone.GetComponent<MeshFilter>();
            if (mr == null || mf == null || mf.sharedMesh == null) continue;

            sharedMat ??= mr.sharedMaterial;
            if (sharedMat == null) continue;

            var baked = BakeAtlasUVs(mf.sharedMesh, stone.Index);
            bakedMeshes.Add(baked);

            combineInstances.Add(new CombineInstance
            {
                mesh = baked,
                transform = mr.transform.localToWorldMatrix,
                subMeshIndex = 0,
            });
        }

        if (combineInstances.Count == 0)
        {
            CleanupTempMeshes(bakedMeshes);
            EditorUtility.DisplayDialog("Combine Stones",
                "Не нашлось ни одного валидного MeshFilter+MeshRenderer на камнях.",
                "OK");
            return;
        }

        var combined = new Mesh
        {
            name = CombinedGameObjectName,
            indexFormat = IndexFormat.UInt32,
        };
        combined.CombineMeshes(combineInstances.ToArray(), mergeSubMeshes: true, useMatrices: true);
        combined.RecalculateBounds();

        var assetPath = ResolveCombinedMeshPath(scene);
        SaveCombinedMesh(combined, assetPath, out combined);

        CleanupTempMeshes(bakedMeshes);

        var go = GetOrCreateCombinedGameObject(out var wasCreated);

        if (wasCreated)
            Undo.RegisterCreatedObjectUndo(go, UndoLabelCombine);
        else
            Undo.RecordObject(go, UndoLabelCombine);

        go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        go.transform.localScale = Vector3.one;

        var newMf = go.GetComponent<MeshFilter>();
        if (newMf == null) newMf = Undo.AddComponent<MeshFilter>(go);
        else Undo.RecordObject(newMf, UndoLabelCombine);
        newMf.sharedMesh = combined;

        var newMr = go.GetComponent<MeshRenderer>();
        if (newMr == null) newMr = Undo.AddComponent<MeshRenderer>(go);
        else Undo.RecordObject(newMr, UndoLabelCombine);
        newMr.sharedMaterial = sharedMat;
        newMr.shadowCastingMode = ShadowCastingMode.On;
        newMr.receiveShadows = true;

        // StonesCombinedRoot сам выставит MaterialPropertyBlock (_AtlasTiling=1, _AtlasOffset=0)
        // в OnEnable как в editor (через [ExecuteAlways]), так и в Play Mode после reload сцены.
        var marker = go.GetComponent<StonesCombinedRoot>();
        if (marker == null) marker = Undo.AddComponent<StonesCombinedRoot>(go);
        else Undo.RecordObject(marker, UndoLabelCombine);
        marker.sourceCount = combineInstances.Count;
        marker.meshAssetPath = assetPath;
        marker.disabledOriginals.Clear();

        foreach (var stone in stones)
        {
            if (stone == null) continue;
            var root = ResolveStoneRoot(stone.transform);
            if (root == null || !root.activeSelf) continue;

            Undo.RecordObject(root, UndoLabelCombine);
            root.SetActive(false);
            disabledRoots.Add(root);
            marker.disabledOriginals.Add(root);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorUtility.SetDirty(go);
        Selection.activeGameObject = go;
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[StoneAtlasCombiner] Combined {combineInstances.Count} stones → '{assetPath}'. " +
                  $"Vertices: {combined.vertexCount}, triangles: {combined.triangles.Length / 3}. " +
                  $"Disabled {disabledRoots.Count} originals.");
    }

    [MenuItem(MenuRestore, priority = 101)]
    private static void RestoreOriginals()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            EditorUtility.DisplayDialog("Restore Stones", "Нет активной сцены.", "OK");
            return;
        }

        Undo.IncrementCurrentGroup();
        var undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(UndoLabelRestore);

        var enabled = RestoreDisabledFromMarkers(UndoLabelRestore);

        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[StoneAtlasCombiner] Restored {enabled} originals, removed combined roots.");
    }

    /// <summary>
    /// Возвращает в активное состояние камни, которые мы сами отключили (по маркеру),
    /// и удаляет все StonesCombinedRoot со сцены. Возвращает количество восстановленных GO.
    /// </summary>
    private static int RestoreDisabledFromMarkers(string undoLabel)
    {
        int enabledCount = 0;
        var combinedRoots = Object.FindObjectsByType<StonesCombinedRoot>(FindObjectsInactive.Include);

        foreach (var combinedRoot in combinedRoots)
        {
            if (combinedRoot == null) continue;

            foreach (var go in combinedRoot.disabledOriginals)
            {
                if (go == null || go.activeSelf) continue;
                Undo.RecordObject(go, undoLabel);
                go.SetActive(true);
                enabledCount++;
            }

            Undo.DestroyObjectImmediate(combinedRoot.gameObject);
        }

        return enabledCount;
    }

    [MenuItem(MenuCombine, validate = true)]
    private static bool CombineOnMapValidate()
    {
        return SceneManager.GetActiveScene().IsValid();
    }

    [MenuItem(MenuRestore, validate = true)]
    private static bool RestoreOriginalsValidate()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid()) return false;
        return Object.FindAnyObjectByType<StonesCombinedRoot>(FindObjectsInactive.Include) != null;
    }

    private static Mesh BakeAtlasUVs(Mesh src, int stoneIndex)
    {
        var baked = new Mesh
        {
            name = src.name + "_baked",
            indexFormat = src.indexFormat,
            vertices = src.vertices,
            normals = src.normals,
            tangents = src.tangents,
            colors = src.colors,
        };

        var subMeshCount = src.subMeshCount;
        baked.subMeshCount = subMeshCount;
        for (int i = 0; i < subMeshCount; i++)
            baked.SetTriangles(src.GetTriangles(i), i);

        int idx = Mathf.Clamp(stoneIndex, 0, AtlasColumns * AtlasRows - 1);
        int col = idx % AtlasColumns;
        int row = idx / AtlasColumns;

        const float tileX = 1f / AtlasColumns;
        const float tileY = 1f / AtlasRows;
        float offsetX = col * tileX;
        float offsetY = (AtlasRows - 1 - row) * tileY;

        var srcUV = src.uv;
        if (srcUV == null || srcUV.Length == 0)
        {
            baked.uv = srcUV;
            return baked;
        }

        var newUV = new Vector2[srcUV.Length];
        for (int i = 0; i < srcUV.Length; i++)
            newUV[i] = new Vector2(srcUV[i].x * tileX + offsetX, srcUV[i].y * tileY + offsetY);
        baked.uv = newUV;
        return baked;
    }

    private static string ResolveCombinedMeshPath(Scene scene)
    {
        if (string.IsNullOrEmpty(scene.path))
            return FallbackAssetPath;

        var dir = Path.GetDirectoryName(scene.path)?.Replace('\\', '/');
        var sceneName = Path.GetFileNameWithoutExtension(scene.path);
        if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(sceneName))
            return FallbackAssetPath;

        return $"{dir}/{sceneName}_StonesCombined.asset";
    }

    private static void SaveCombinedMesh(Mesh combined, string assetPath, out Mesh result)
    {
        var dir = Path.GetDirectoryName(assetPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
        if (existing != null)
        {
            EditorUtility.CopySerialized(combined, existing);
            Object.DestroyImmediate(combined);
            result = existing;
        }
        else
        {
            AssetDatabase.CreateAsset(combined, assetPath);
            result = combined;
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

    private static GameObject GetOrCreateCombinedGameObject(out bool wasCreated)
    {
        var existing = Object.FindAnyObjectByType<StonesCombinedRoot>(FindObjectsInactive.Include);
        if (existing != null)
        {
            wasCreated = false;
            return existing.gameObject;
        }

        wasCreated = true;
        return new GameObject(CombinedGameObjectName);
    }

    private static GameObject ResolveStoneRoot(Transform t)
    {
        if (t == null) return null;

        var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(t.gameObject);
        if (prefabRoot != null) return prefabRoot;

        return t.parent != null ? t.parent.gameObject : t.gameObject;
    }
}
