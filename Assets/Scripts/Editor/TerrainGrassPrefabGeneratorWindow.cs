using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace vikwhite
{
    public sealed class TerrainGrassPrefabGeneratorWindow : EditorWindow
    {
        private const string DefaultOutputRootName = "Generated Grass Prefabs";
        private const int ConfirmCandidateCount = 50000;
        private const string PrefabPathPrefsKey = "grassPrefabPath";

        [SerializeField] private Terrain terrain;
        [SerializeField] private GameObject grassPrefab;
        [SerializeField] private Transform outputParent;
        [SerializeField] private string outputRootName = DefaultOutputRootName;
        [SerializeField] private int grassLayerIndex = -1;
        [SerializeField] private float spacing = 1.5f;
        [SerializeField] private float layerThreshold = 0.5f;
        [SerializeField] private bool requireDominantLayer = true;
        [SerializeField] private float maxSlopeAngle = 90f;
        [SerializeField] private bool useFractalNoise = true;
        [SerializeField] private float noiseScale = 18f;
        [SerializeField] private int noiseOctaves = 4;
        [SerializeField] private float noisePersistence = 0.5f;
        [SerializeField] private float noiseLacunarity = 2f;
        [SerializeField] private float noiseThreshold = 0.35f;
        [SerializeField] private float noiseDensity = 1f;
        [SerializeField] private bool clearBeforeGenerate = true;
        [SerializeField] private bool alignToTerrainNormal = true;
        [SerializeField] private float heightOffset;
        [SerializeField] private float randomOffset = 0.5f;
        [SerializeField] private bool randomYaw = true;
        [SerializeField] private Vector2 scaleRange = Vector2.one;
        [SerializeField] private int randomSeed = 12345;
        [SerializeField] private bool showPreview = true;
        [SerializeField] private float previewMarkerRadius = 0.35f;

        private GrassPlacement[] previewPlacements = Array.Empty<GrassPlacement>();
        private bool previewDirty = true;
        private string previewStatus = "Preview needs refresh";

        [MenuItem("Tools/Terrain/Grass Prefab Generator")]
        public static void Open()
        {
            TerrainGrassPrefabGeneratorWindow window = GetWindow<TerrainGrassPrefabGeneratorWindow>("Grass Prefabs");
            window.TryUseSelectionAsTerrain();
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            if (terrain == null)
            {
                terrain = Terrain.activeTerrain;
            }

            RefreshLayerSelection();
            MarkPreviewDirty();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SaveSettings();
        }

        private void OnSelectionChange()
        {
            if (terrain == null)
            {
                TryUseSelectionAsTerrain();
                MarkPreviewDirty();
                Repaint();
            }
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);
            if (EditorGUI.EndChangeCheck())
            {
                RefreshLayerSelection();
            }

            grassPrefab = (GameObject)EditorGUILayout.ObjectField("Grass Prefab", grassPrefab, typeof(GameObject), false);
            DrawTerrainLayerSelector();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Placement", EditorStyles.boldLabel);
            spacing = Mathf.Max(0.01f, EditorGUILayout.FloatField("Spacing", spacing));
            layerThreshold = EditorGUILayout.Slider("Layer Threshold", layerThreshold, 0.01f, 1f);
            requireDominantLayer = EditorGUILayout.Toggle("Require Dominant Layer", requireDominantLayer);
            maxSlopeAngle = EditorGUILayout.Slider("Max Slope Angle", maxSlopeAngle, 0f, 90f);
            alignToTerrainNormal = EditorGUILayout.Toggle("Align To Terrain Normal", alignToTerrainNormal);
            heightOffset = EditorGUILayout.FloatField("Height Offset", heightOffset);

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Fractal Noise", EditorStyles.boldLabel);
            useFractalNoise = EditorGUILayout.Toggle("Use Fractal Noise", useFractalNoise);
            using (new EditorGUI.DisabledScope(!useFractalNoise))
            {
                noiseScale = Mathf.Max(0.01f, EditorGUILayout.FloatField("Noise Scale", noiseScale));
                noiseOctaves = EditorGUILayout.IntSlider("Noise Octaves", noiseOctaves, 1, 8);
                noisePersistence = EditorGUILayout.Slider("Noise Persistence", noisePersistence, 0.01f, 1f);
                noiseLacunarity = Mathf.Max(1.01f, EditorGUILayout.FloatField("Noise Lacunarity", noiseLacunarity));
                noiseThreshold = EditorGUILayout.Slider("Noise Threshold", noiseThreshold, 0f, 0.99f);
                noiseDensity = EditorGUILayout.Slider("Noise Density", noiseDensity, 0f, 1f);
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Randomization", EditorStyles.boldLabel);
            randomSeed = EditorGUILayout.IntField("Seed", randomSeed);
            randomOffset = Mathf.Max(0f, EditorGUILayout.FloatField("Random Offset", randomOffset));
            randomYaw = EditorGUILayout.Toggle("Random Yaw", randomYaw);
            scaleRange = EditorGUILayout.Vector2Field("Scale Range", scaleRange);
            NormalizeScaleRange();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            showPreview = EditorGUILayout.Toggle("Show Scene Preview", showPreview);
            previewMarkerRadius = Mathf.Max(0.01f, EditorGUILayout.FloatField("Marker Radius", previewMarkerRadius));

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!CanPreview()))
                {
                    if (GUILayout.Button("Refresh Preview"))
                    {
                        MarkPreviewDirty();
                        RebuildPreview();
                        SceneView.RepaintAll();
                    }
                }

                string countText = previewDirty ? previewStatus : $"{previewPlacements.Length:n0} markers";
                EditorGUILayout.LabelField(countText);
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            outputParent = (Transform)EditorGUILayout.ObjectField("Output Parent", outputParent, typeof(Transform), true);
            using (new EditorGUI.DisabledScope(outputParent != null))
            {
                outputRootName = EditorGUILayout.TextField("Root Name", outputRootName);
            }

            clearBeforeGenerate = EditorGUILayout.Toggle("Clear Before Generate", clearBeforeGenerate);

            EditorGUILayout.Space(12f);
            DrawActionButtons();

            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
                MarkPreviewDirty();
                SceneView.RepaintAll();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!showPreview || !CanPreview())
            {
                return;
            }

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (previewDirty)
            {
                RebuildPreview();
            }

            DrawPreview();
        }

        private void DrawTerrainLayerSelector()
        {
            TerrainData terrainData = terrain != null ? terrain.terrainData : null;
            TerrainLayer[] layers = terrainData != null ? terrainData.terrainLayers : Array.Empty<TerrainLayer>();

            using (new EditorGUI.DisabledScope(layers.Length == 0))
            {
                if (layers.Length == 0)
                {
                    grassLayerIndex = -1;
                    EditorGUILayout.Popup("Grass Layer", 0, new[] { "No terrain layers" });
                    return;
                }

                RefreshLayerSelection();
                string[] layerNames = new string[layers.Length];
                for (int i = 0; i < layers.Length; i++)
                {
                    layerNames[i] = layers[i] != null ? layers[i].name : $"Missing Layer {i}";
                }

                grassLayerIndex = EditorGUILayout.Popup("Grass Layer", grassLayerIndex, layerNames);
            }
        }

        private void DrawActionButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!CanGenerate()))
                {
                    if (GUILayout.Button("Generate Prefabs", GUILayout.Height(28f)))
                    {
                        Generate();
                    }
                }

                using (new EditorGUI.DisabledScope(!CanClear()))
                {
                    if (GUILayout.Button("Clear Generated", GUILayout.Height(28f)))
                    {
                        ClearGenerated();
                    }
                }
            }

            if (terrain == null)
            {
                EditorGUILayout.HelpBox("Assign a Terrain or select one in the hierarchy.", MessageType.Info);
            }
            else if (grassPrefab == null)
            {
                EditorGUILayout.HelpBox("Assign the grass prefab that should be placed on the terrain texture layer.", MessageType.Info);
            }
            else if (grassLayerIndex < 0)
            {
                EditorGUILayout.HelpBox("The selected terrain has no texture layers.", MessageType.Warning);
            }
        }

        private void Generate()
        {
            SaveSettings();

            TerrainData terrainData = terrain.terrainData;
            int estimatedCandidates = EstimateCandidateCount(terrainData.size);
            if (estimatedCandidates > ConfirmCandidateCount)
            {
                bool continueGeneration = EditorUtility.DisplayDialog(
                    "Generate Grass Prefabs",
                    $"This can scan about {estimatedCandidates:n0} points before filtering by the terrain texture. Continue?",
                    "Generate",
                    "Cancel");

                if (!continueGeneration)
                {
                    return;
                }
            }

            Transform parent = GetOrCreateOutputParent();
            if (parent == null)
            {
                return;
            }

            if (clearBeforeGenerate)
            {
                ClearChildren(parent);
            }

            int createdCount = CreateGrassPrefabs(parent, terrainData, estimatedCandidates);
            EditorSceneManager.MarkSceneDirty(terrain.gameObject.scene);
            Debug.Log($"Grass Prefab Generator created {createdCount} prefab instances on '{terrain.name}'.", terrain);
        }

        private int CreateGrassPrefabs(Transform parent, TerrainData terrainData, int estimatedCandidates)
        {
            try
            {
                return ProcessGrassPlacements(
                    terrainData,
                    estimatedCandidates,
                    (checkedCount, totalCount) => checkedCount % 256 == 0 && EditorUtility.DisplayCancelableProgressBar(
                                "Generating grass prefabs",
                                $"{checkedCount:n0} / {totalCount:n0}",
                                totalCount > 0 ? checkedCount / (float)totalCount : 1f),
                    placement =>
                    {
                        GameObject instance = CreatePrefabInstance(grassPrefab);

                        Undo.RegisterCreatedObjectUndo(instance, "Generate Grass Prefabs");
                        Undo.SetTransformParent(instance.transform, parent, "Generate Grass Prefabs");
                        instance.transform.SetPositionAndRotation(placement.Position, placement.Rotation);
                        instance.transform.localScale = Vector3.Scale(instance.transform.localScale, Vector3.one * placement.ScaleMultiplier);
                        instance.name = grassPrefab.name;
                    });
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void RebuildPreview()
        {
            if (!CanPreview())
            {
                previewPlacements = Array.Empty<GrassPlacement>();
                previewDirty = false;
                previewStatus = "Preview unavailable";
                return;
            }

            TerrainData terrainData = terrain.terrainData;
            int estimatedCandidates = EstimateCandidateCount(terrainData.size);
            var placements = new List<GrassPlacement>(Mathf.Min(estimatedCandidates, 65536));
            bool canceled = false;

            try
            {
                ProcessGrassPlacements(
                    terrainData,
                    estimatedCandidates,
                    (checkedCount, totalCount) =>
                    {
                        if (totalCount < ConfirmCandidateCount || checkedCount % 1024 != 0)
                        {
                            return false;
                        }

                        canceled = EditorUtility.DisplayCancelableProgressBar(
                            "Building grass preview",
                            $"{checkedCount:n0} / {totalCount:n0}",
                            totalCount > 0 ? checkedCount / (float)totalCount : 1f);
                        return canceled;
                    },
                    placement => placements.Add(placement));
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            previewPlacements = placements.ToArray();
            previewDirty = false;
            previewStatus = canceled
                ? $"Preview canceled, {previewPlacements.Length:n0} markers"
                : $"{previewPlacements.Length:n0} markers";
            Repaint();
        }

        private void DrawPreview()
        {
            if (previewPlacements.Length == 0)
            {
                return;
            }

            Color previousColor = Handles.color;
            CompareFunction previousZTest = Handles.zTest;
            Handles.color = new Color(1f, 0f, 0f, 0.45f);
            Handles.zTest = CompareFunction.LessEqual;

            float radius = Mathf.Max(0.01f, previewMarkerRadius);
            for (int i = 0; i < previewPlacements.Length; i++)
            {
                GrassPlacement placement = previewPlacements[i];
                Handles.DrawSolidDisc(placement.Position + placement.Normal * 0.03f, placement.Normal, radius);
            }

            Handles.zTest = previousZTest;
            Handles.color = previousColor;
        }

        private int ProcessGrassPlacements(
            TerrainData terrainData,
            int estimatedCandidates,
            Func<int, int, bool> shouldCancel,
            Action<GrassPlacement> onPlacement)
        {
            float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            Vector3 terrainPosition = terrain.GetPosition();
            Vector3 terrainSize = terrainData.size;
            var random = new System.Random(randomSeed);
            int createdCount = 0;
            int checkedCount = 0;
            float maxOffset = Mathf.Min(randomOffset, spacing * 0.5f);

            for (float localZ = 0f; localZ <= terrainSize.z; localZ += spacing)
            {
                for (float localX = 0f; localX <= terrainSize.x; localX += spacing)
                {
                    checkedCount++;
                    if (shouldCancel != null && shouldCancel(checkedCount, estimatedCandidates))
                    {
                        return createdCount;
                    }

                    Vector2 localPosition = GetJitteredLocalPosition(localX, localZ, terrainSize, maxOffset, random);
                    float normalizedX = Mathf.Clamp01(localPosition.x / terrainSize.x);
                    float normalizedZ = Mathf.Clamp01(localPosition.y / terrainSize.z);

                    if (!IsGrassTextureAt(alphamaps, normalizedX, normalizedZ))
                    {
                        continue;
                    }

                    Vector3 terrainNormal = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
                    if (Vector3.Angle(Vector3.up, terrainNormal) > maxSlopeAngle)
                    {
                        continue;
                    }

                    if (!ShouldPlaceByNoise(localPosition, random))
                    {
                        continue;
                    }

                    Vector3 worldPosition = GetTerrainWorldPosition(terrainData, terrainPosition, localPosition, normalizedX, normalizedZ);
                    Quaternion rotation = GetPrefabRotation(terrainNormal, random);
                    float scaleMultiplier = RandomRange(random, scaleRange.x, scaleRange.y);
                    onPlacement?.Invoke(new GrassPlacement(worldPosition, terrainNormal, rotation, scaleMultiplier));
                    createdCount++;
                }
            }

            return createdCount;
        }

        private Vector2 GetJitteredLocalPosition(float localX, float localZ, Vector3 terrainSize, float maxOffset, System.Random random)
        {
            if (maxOffset <= 0f)
            {
                return new Vector2(localX, localZ);
            }

            float jitterX = RandomRange(random, -maxOffset, maxOffset);
            float jitterZ = RandomRange(random, -maxOffset, maxOffset);
            return new Vector2(
                Mathf.Clamp(localX + jitterX, 0f, terrainSize.x),
                Mathf.Clamp(localZ + jitterZ, 0f, terrainSize.z));
        }

        private bool ShouldPlaceByNoise(Vector2 localPosition, System.Random random)
        {
            if (!useFractalNoise)
            {
                return true;
            }

            float noise = EvaluateFractalNoise(localPosition);
            if (noise < noiseThreshold)
            {
                return false;
            }

            float density = noiseThreshold >= 0.999f
                ? 1f
                : Mathf.InverseLerp(noiseThreshold, 1f, noise);
            density *= noiseDensity;
            return RandomRange(random, 0f, 1f) <= density;
        }

        private float EvaluateFractalNoise(Vector2 localPosition)
        {
            float value = 0f;
            float amplitude = 1f;
            float frequency = 1f;
            float maxValue = 0f;
            float scale = Mathf.Max(0.01f, noiseScale);
            float offsetX = randomSeed * 13.371f + 1000f;
            float offsetZ = randomSeed * 41.731f + 2000f;

            for (int i = 0; i < noiseOctaves; i++)
            {
                float sampleX = (localPosition.x + offsetX) / scale * frequency;
                float sampleZ = (localPosition.y + offsetZ) / scale * frequency;
                value += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                maxValue += amplitude;
                amplitude *= noisePersistence;
                frequency *= noiseLacunarity;
            }

            return maxValue > 0f ? Mathf.Clamp01(value / maxValue) : 0f;
        }

        private bool IsGrassTextureAt(float[,,] alphamaps, float normalizedX, float normalizedZ)
        {
            int alphamapWidth = alphamaps.GetLength(1);
            int alphamapHeight = alphamaps.GetLength(0);
            int layerCount = alphamaps.GetLength(2);
            int alphaX = Mathf.Clamp(Mathf.RoundToInt(normalizedX * (alphamapWidth - 1)), 0, alphamapWidth - 1);
            int alphaZ = Mathf.Clamp(Mathf.RoundToInt(normalizedZ * (alphamapHeight - 1)), 0, alphamapHeight - 1);

            if (grassLayerIndex < 0 || grassLayerIndex >= layerCount)
            {
                return false;
            }

            float grassWeight = alphamaps[alphaZ, alphaX, grassLayerIndex];
            if (grassWeight < layerThreshold)
            {
                return false;
            }

            if (!requireDominantLayer)
            {
                return true;
            }

            for (int i = 0; i < layerCount; i++)
            {
                if (i == grassLayerIndex)
                {
                    continue;
                }

                if (alphamaps[alphaZ, alphaX, i] > grassWeight)
                {
                    return false;
                }
            }

            return true;
        }

        private Vector3 GetTerrainWorldPosition(
            TerrainData terrainData,
            Vector3 terrainPosition,
            Vector2 localPosition,
            float normalizedX,
            float normalizedZ)
        {
            float height = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
            return new Vector3(
                terrainPosition.x + localPosition.x,
                terrainPosition.y + height + heightOffset,
                terrainPosition.z + localPosition.y);
        }

        private Quaternion GetPrefabRotation(Vector3 terrainNormal, System.Random random)
        {
            float yaw = randomYaw ? RandomRange(random, 0f, 360f) : 0f;
            Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
            if (!alignToTerrainNormal)
            {
                return yawRotation;
            }

            return Quaternion.FromToRotation(Vector3.up, terrainNormal) * yawRotation;
        }

        private Transform GetOrCreateOutputParent()
        {
            if (outputParent != null)
            {
                return outputParent;
            }

            string rootName = string.IsNullOrWhiteSpace(outputRootName) ? DefaultOutputRootName : outputRootName.Trim();
            Transform existingRoot = terrain.transform.Find(rootName);
            if (existingRoot != null)
            {
                return existingRoot;
            }

            var root = new GameObject(rootName);
            Undo.RegisterCreatedObjectUndo(root, "Create Grass Prefab Root");
            Undo.SetTransformParent(root.transform, terrain.transform, "Create Grass Prefab Root");
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;
            return root.transform;
        }

        private void ClearGenerated()
        {
            Transform parent = outputParent != null ? outputParent : FindOutputRoot();
            if (parent == null)
            {
                return;
            }

            ClearChildren(parent);
            EditorSceneManager.MarkSceneDirty(parent.gameObject.scene);
        }

        private void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
            }
        }

        private Transform FindOutputRoot()
        {
            if (terrain == null)
            {
                return null;
            }

            string rootName = string.IsNullOrWhiteSpace(outputRootName) ? DefaultOutputRootName : outputRootName.Trim();
            return terrain.transform.Find(rootName);
        }

        private void RefreshLayerSelection()
        {
            TerrainData terrainData = terrain != null ? terrain.terrainData : null;
            TerrainLayer[] layers = terrainData != null ? terrainData.terrainLayers : Array.Empty<TerrainLayer>();
            if (layers.Length == 0)
            {
                grassLayerIndex = -1;
                return;
            }

            if (grassLayerIndex >= 0 && grassLayerIndex < layers.Length)
            {
                return;
            }

            grassLayerIndex = FindGrassLayerIndex(layers);
        }

        private int FindGrassLayerIndex(TerrainLayer[] layers)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                TerrainLayer layer = layers[i];
                if (layer == null)
                {
                    continue;
                }

                string layerName = layer.name;
                if (layerName.IndexOf("grass", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return i;
                }
            }

            return 0;
        }

        private bool CanGenerate()
        {
            TerrainData terrainData = terrain != null ? terrain.terrainData : null;
            return terrainData != null
                   && grassPrefab != null
                   && grassLayerIndex >= 0
                   && grassLayerIndex < terrainData.alphamapLayers
                   && spacing > 0f;
        }

        private bool CanPreview()
        {
            TerrainData terrainData = terrain != null ? terrain.terrainData : null;
            return terrainData != null
                   && grassLayerIndex >= 0
                   && grassLayerIndex < terrainData.alphamapLayers
                   && spacing > 0f;
        }

        private bool CanClear()
        {
            return outputParent != null || FindOutputRoot() != null;
        }

        private int EstimateCandidateCount(Vector3 terrainSize)
        {
            int xCount = Mathf.FloorToInt(terrainSize.x / spacing) + 1;
            int zCount = Mathf.FloorToInt(terrainSize.z / spacing) + 1;
            return Mathf.Max(0, xCount * zCount);
        }

        private void TryUseSelectionAsTerrain()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                return;
            }

            terrain = selectedObject.GetComponent<Terrain>();
            RefreshLayerSelection();
        }

        private void NormalizeScaleRange()
        {
            scaleRange.x = Mathf.Max(0.01f, scaleRange.x);
            scaleRange.y = Mathf.Max(0.01f, scaleRange.y);
            if (scaleRange.x > scaleRange.y)
            {
                (scaleRange.x, scaleRange.y) = (scaleRange.y, scaleRange.x);
            }
        }

        private void LoadSettings()
        {
            string prefabPath = EditorPrefs.GetString(GetPrefsKey(PrefabPathPrefsKey), string.Empty);
            if (!string.IsNullOrEmpty(prefabPath))
            {
                GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (loadedPrefab != null)
                {
                    grassPrefab = loadedPrefab;
                }
            }

            outputRootName = EditorPrefs.GetString(GetPrefsKey(nameof(outputRootName)), outputRootName);
            grassLayerIndex = EditorPrefs.GetInt(GetPrefsKey(nameof(grassLayerIndex)), grassLayerIndex);
            spacing = EditorPrefs.GetFloat(GetPrefsKey(nameof(spacing)), spacing);
            layerThreshold = EditorPrefs.GetFloat(GetPrefsKey(nameof(layerThreshold)), layerThreshold);
            requireDominantLayer = EditorPrefs.GetBool(GetPrefsKey(nameof(requireDominantLayer)), requireDominantLayer);
            maxSlopeAngle = EditorPrefs.GetFloat(GetPrefsKey(nameof(maxSlopeAngle)), maxSlopeAngle);
            useFractalNoise = EditorPrefs.GetBool(GetPrefsKey(nameof(useFractalNoise)), useFractalNoise);
            noiseScale = EditorPrefs.GetFloat(GetPrefsKey(nameof(noiseScale)), noiseScale);
            noiseOctaves = EditorPrefs.GetInt(GetPrefsKey(nameof(noiseOctaves)), noiseOctaves);
            noisePersistence = EditorPrefs.GetFloat(GetPrefsKey(nameof(noisePersistence)), noisePersistence);
            noiseLacunarity = EditorPrefs.GetFloat(GetPrefsKey(nameof(noiseLacunarity)), noiseLacunarity);
            noiseThreshold = EditorPrefs.GetFloat(GetPrefsKey(nameof(noiseThreshold)), noiseThreshold);
            noiseDensity = EditorPrefs.GetFloat(GetPrefsKey(nameof(noiseDensity)), noiseDensity);
            clearBeforeGenerate = EditorPrefs.GetBool(GetPrefsKey(nameof(clearBeforeGenerate)), clearBeforeGenerate);
            alignToTerrainNormal = EditorPrefs.GetBool(GetPrefsKey(nameof(alignToTerrainNormal)), alignToTerrainNormal);
            heightOffset = EditorPrefs.GetFloat(GetPrefsKey(nameof(heightOffset)), heightOffset);
            randomOffset = EditorPrefs.GetFloat(GetPrefsKey(nameof(randomOffset)), randomOffset);
            randomYaw = EditorPrefs.GetBool(GetPrefsKey(nameof(randomYaw)), randomYaw);
            scaleRange = new Vector2(
                EditorPrefs.GetFloat(GetPrefsKey(nameof(scaleRange) + ".x"), scaleRange.x),
                EditorPrefs.GetFloat(GetPrefsKey(nameof(scaleRange) + ".y"), scaleRange.y));
            randomSeed = EditorPrefs.GetInt(GetPrefsKey(nameof(randomSeed)), randomSeed);
            showPreview = EditorPrefs.GetBool(GetPrefsKey(nameof(showPreview)), showPreview);
            previewMarkerRadius = EditorPrefs.GetFloat(GetPrefsKey(nameof(previewMarkerRadius)), previewMarkerRadius);

            NormalizeScaleRange();
        }

        private void SaveSettings()
        {
            string prefabPath = grassPrefab != null ? AssetDatabase.GetAssetPath(grassPrefab) : string.Empty;
            EditorPrefs.SetString(GetPrefsKey(PrefabPathPrefsKey), prefabPath);
            EditorPrefs.SetString(GetPrefsKey(nameof(outputRootName)), outputRootName ?? string.Empty);
            EditorPrefs.SetInt(GetPrefsKey(nameof(grassLayerIndex)), grassLayerIndex);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(spacing)), spacing);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(layerThreshold)), layerThreshold);
            EditorPrefs.SetBool(GetPrefsKey(nameof(requireDominantLayer)), requireDominantLayer);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(maxSlopeAngle)), maxSlopeAngle);
            EditorPrefs.SetBool(GetPrefsKey(nameof(useFractalNoise)), useFractalNoise);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(noiseScale)), noiseScale);
            EditorPrefs.SetInt(GetPrefsKey(nameof(noiseOctaves)), noiseOctaves);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(noisePersistence)), noisePersistence);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(noiseLacunarity)), noiseLacunarity);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(noiseThreshold)), noiseThreshold);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(noiseDensity)), noiseDensity);
            EditorPrefs.SetBool(GetPrefsKey(nameof(clearBeforeGenerate)), clearBeforeGenerate);
            EditorPrefs.SetBool(GetPrefsKey(nameof(alignToTerrainNormal)), alignToTerrainNormal);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(heightOffset)), heightOffset);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(randomOffset)), randomOffset);
            EditorPrefs.SetBool(GetPrefsKey(nameof(randomYaw)), randomYaw);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(scaleRange) + ".x"), scaleRange.x);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(scaleRange) + ".y"), scaleRange.y);
            EditorPrefs.SetInt(GetPrefsKey(nameof(randomSeed)), randomSeed);
            EditorPrefs.SetBool(GetPrefsKey(nameof(showPreview)), showPreview);
            EditorPrefs.SetFloat(GetPrefsKey(nameof(previewMarkerRadius)), previewMarkerRadius);
        }

        private static string GetPrefsKey(string name)
        {
            return $"vikwhite.TerrainGrassPrefabGeneratorWindow.{Application.dataPath}.{name}";
        }

        private static GameObject CreatePrefabInstance(GameObject prefab)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            }

            return Instantiate(prefab);
        }

        private static float RandomRange(System.Random random, float min, float max)
        {
            return min + (max - min) * (float)random.NextDouble();
        }

        private void MarkPreviewDirty()
        {
            previewDirty = true;
            previewStatus = "Preview needs refresh";
        }

        private struct GrassPlacement
        {
            public readonly Vector3 Position;
            public readonly Vector3 Normal;
            public readonly Quaternion Rotation;
            public readonly float ScaleMultiplier;

            public GrassPlacement(Vector3 position, Vector3 normal, Quaternion rotation, float scaleMultiplier)
            {
                Position = position;
                Normal = normal;
                Rotation = rotation;
                ScaleMultiplier = scaleMultiplier;
            }
        }
    }
}
