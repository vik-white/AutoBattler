using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace vikwhite.EditorTools
{
    public sealed class MeshToTerrainHeightmapWindow : EditorWindow
    {
        private const string UndoName = "Mesh To Terrain Heightmap";

        [SerializeField] private Object meshSource;
        [SerializeField] private Terrain targetTerrain;
        [SerializeField] private bool searchInChildren = true;
        [SerializeField] private bool fitSourceBoundsToTerrain = true;
        [SerializeField] private bool scaleHeightToFullTerrain;
        [SerializeField] private bool keepExistingHeightOnMiss;
        [SerializeField] private bool hitBackfaces = true;

        private string statusMessage;
        private MessageType statusType = MessageType.Info;

        [MenuItem("Tools/Terrain/Mesh To Terrain Heightmap", false, 100)]
        private static void Open()
        {
            GetWindow<MeshToTerrainHeightmapWindow>("Mesh To Terrain");
        }

        private void OnEnable()
        {
            AssignFromSelection(false);
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Mesh To Terrain Heightmap", EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);

            meshSource = EditorGUILayout.ObjectField(
                new GUIContent("Mesh Source", "Mesh asset, GameObject, MeshFilter, or SkinnedMeshRenderer."),
                meshSource,
                typeof(Object),
                true);

            targetTerrain = (Terrain)EditorGUILayout.ObjectField(
                new GUIContent("Target Terrain", "Terrain that receives the generated heightmap."),
                targetTerrain,
                typeof(Terrain),
                true);

            if (GUILayout.Button("Use Selection"))
            {
                AssignFromSelection(true);
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            searchInChildren = EditorGUILayout.ToggleLeft("Search Mesh In Children", searchInChildren);
            fitSourceBoundsToTerrain = EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "Fit Source Bounds To Terrain",
                    "Maps the source mesh X/Z bounds to the full terrain heightmap."),
                fitSourceBoundsToTerrain);
            scaleHeightToFullTerrain = EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "Scale Height To Full Terrain Height",
                    "Maps source min Y to 0 and source max Y to the terrain height scale. Disabled keeps mesh height proportional to the X/Z terrain fit."),
                scaleHeightToFullTerrain);
            keepExistingHeightOnMiss = EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "Keep Existing Height On Ray Miss",
                    "Leaves the current terrain height when the vertical ray does not hit the mesh."),
                keepExistingHeightOnMiss);
            hitBackfaces = EditorGUILayout.ToggleLeft(
                new GUIContent(
                    "Hit Backfaces",
                    "Allows sampling meshes whose triangles face down or have mixed winding."),
                hitBackfaces);

            EditorGUILayout.Space(8f);

            string validationError = GetValidationError();
            using (new EditorGUI.DisabledScope(!string.IsNullOrEmpty(validationError)))
            {
                if (GUILayout.Button("Convert", GUILayout.Height(32f)))
                {
                    Convert();
                }
            }

            if (!string.IsNullOrEmpty(validationError))
            {
                EditorGUILayout.HelpBox(validationError, MessageType.Warning);
            }

            if (!string.IsNullOrEmpty(statusMessage))
            {
                EditorGUILayout.HelpBox(statusMessage, statusType);
            }
        }

        private void AssignFromSelection(bool force)
        {
            bool assignTerrain = force || targetTerrain == null;
            bool assignMesh = force || meshSource == null;

            foreach (Object selectedObject in Selection.objects)
            {
                if (assignTerrain && TryGetTerrain(selectedObject, out Terrain selectedTerrain))
                {
                    targetTerrain = selectedTerrain;
                    assignTerrain = false;
                    continue;
                }

                if (assignMesh && IsValidMeshSelection(selectedObject))
                {
                    meshSource = selectedObject;
                    assignMesh = false;
                }
            }

            Repaint();
        }

        private void Convert()
        {
            statusMessage = null;

            if (!TryCreateSampler(
                    out GameObject samplerObject,
                    out Mesh temporaryMesh,
                    out MeshCollider sampler,
                    out Bounds sourceBounds,
                    out string error))
            {
                SetStatus(error, MessageType.Error);
                return;
            }

            bool oldQueriesHitBackfaces = Physics.queriesHitBackfaces;
            bool canceled = false;

            try
            {
                TerrainData terrainData = targetTerrain.terrainData;
                int resolution = terrainData.heightmapResolution;
                float[,] heights = keepExistingHeightOnMiss
                    ? terrainData.GetHeights(0, 0, resolution, resolution)
                    : new float[resolution, resolution];

                Vector3 terrainPosition = targetTerrain.GetPosition();
                Vector3 terrainSize = terrainData.size;
                float rayPadding = Mathf.Max(1f, sourceBounds.size.y * 0.25f);
                float maxRayDistance = Mathf.Max(2f, sourceBounds.size.y + rayPadding * 2f);
                float sourceHeightRange = sourceBounds.size.y;
                float proportionalHeightScale = GetProportionalHeightScale(sourceBounds, terrainSize);
                int hitCount = 0;
                int missCount = 0;

                Physics.queriesHitBackfaces = hitBackfaces;

                for (int z = 0; z < resolution; z++)
                {
                    if (z % 8 == 0)
                    {
                        float progress = z / (float)resolution;
                        if (EditorUtility.DisplayCancelableProgressBar(
                                "Mesh To Terrain Heightmap",
                                $"Sampling height row {z + 1} of {resolution}",
                                progress))
                        {
                            canceled = true;
                            break;
                        }
                    }

                    float v = resolution <= 1 ? 0f : z / (float)(resolution - 1);

                    for (int x = 0; x < resolution; x++)
                    {
                        float u = resolution <= 1 ? 0f : x / (float)(resolution - 1);
                        Vector3 samplePosition = GetSamplePosition(
                            u,
                            v,
                            sourceBounds,
                            terrainPosition,
                            terrainSize);

                        Vector3 rayOrigin = new Vector3(
                            samplePosition.x,
                            sourceBounds.max.y + rayPadding,
                            samplePosition.z);

                        if (sampler.Raycast(new Ray(rayOrigin, Vector3.down), out RaycastHit hit, maxRayDistance))
                        {
                            heights[z, x] = GetTerrainHeight01(
                                hit.point.y,
                                sourceBounds.min.y,
                                sourceHeightRange,
                                terrainSize.y,
                                proportionalHeightScale);
                            hitCount++;
                        }
                        else
                        {
                            missCount++;
                        }
                    }
                }

                if (canceled)
                {
                    SetStatus("Conversion canceled. Terrain was not changed.", MessageType.Info);
                    return;
                }

                Undo.RegisterCompleteObjectUndo(terrainData, UndoName);
                terrainData.SetHeights(0, 0, heights);
                targetTerrain.Flush();
                EditorUtility.SetDirty(terrainData);
                EditorUtility.SetDirty(targetTerrain);

                if (!Application.isPlaying && targetTerrain.gameObject.scene.IsValid())
                {
                    EditorSceneManager.MarkSceneDirty(targetTerrain.gameObject.scene);
                }

                SetStatus($"Done. Hits: {hitCount}. Misses: {missCount}.", MessageType.Info);

                if (missCount > 0)
                {
                    Debug.LogWarning(
                        $"{nameof(MeshToTerrainHeightmapWindow)}: {missCount} heightmap samples did not hit the source mesh.",
                        targetTerrain);
                }
            }
            finally
            {
                Physics.queriesHitBackfaces = oldQueriesHitBackfaces;
                EditorUtility.ClearProgressBar();

                if (samplerObject != null)
                {
                    DestroyImmediate(samplerObject);
                }

                if (temporaryMesh != null)
                {
                    DestroyImmediate(temporaryMesh);
                }
            }
        }

        private Vector3 GetSamplePosition(
            float u,
            float v,
            Bounds sourceBounds,
            Vector3 terrainPosition,
            Vector3 terrainSize)
        {
            if (fitSourceBoundsToTerrain)
            {
                return new Vector3(
                    Mathf.Lerp(sourceBounds.min.x, sourceBounds.max.x, u),
                    0f,
                    Mathf.Lerp(sourceBounds.min.z, sourceBounds.max.z, v));
            }

            return new Vector3(
                terrainPosition.x + terrainSize.x * u,
                0f,
                terrainPosition.z + terrainSize.z * v);
        }

        private float GetTerrainHeight01(
            float worldY,
            float sourceMinY,
            float sourceHeightRange,
            float terrainHeight,
            float proportionalHeightScale)
        {
            float sourceRelativeHeight = worldY - sourceMinY;

            if (scaleHeightToFullTerrain)
            {
                if (sourceHeightRange <= Mathf.Epsilon)
                {
                    return 0f;
                }

                return Mathf.Clamp01(sourceRelativeHeight / sourceHeightRange);
            }

            if (terrainHeight <= Mathf.Epsilon)
            {
                return 0f;
            }

            return Mathf.Clamp01(sourceRelativeHeight * proportionalHeightScale / terrainHeight);
        }

        private float GetProportionalHeightScale(Bounds sourceBounds, Vector3 terrainSize)
        {
            if (!fitSourceBoundsToTerrain)
            {
                return 1f;
            }

            float xScale = terrainSize.x / sourceBounds.size.x;
            float zScale = terrainSize.z / sourceBounds.size.z;
            return (xScale + zScale) * 0.5f;
        }

        private bool TryCreateSampler(
            out GameObject samplerObject,
            out Mesh temporaryMesh,
            out MeshCollider sampler,
            out Bounds sourceBounds,
            out string error)
        {
            samplerObject = null;
            temporaryMesh = null;
            sampler = null;
            sourceBounds = default;

            if (!TryResolveMeshSource(out MeshSourceDescriptor descriptor, out error))
            {
                return false;
            }

            Mesh colliderMesh = descriptor.Mesh;
            if (descriptor.SkinnedMeshRenderer != null)
            {
                temporaryMesh = new Mesh
                {
                    name = $"{descriptor.Mesh.name}_BakedTerrainSource",
                    hideFlags = HideFlags.HideAndDontSave
                };
                descriptor.SkinnedMeshRenderer.BakeMesh(temporaryMesh);
                colliderMesh = temporaryMesh;
            }

            samplerObject = new GameObject("Mesh To Terrain Height Sampler")
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            if (descriptor.Transform != null)
            {
                samplerObject.transform.SetPositionAndRotation(
                    descriptor.Transform.position,
                    descriptor.Transform.rotation);
                samplerObject.transform.localScale = descriptor.Transform.lossyScale;
            }

            sampler = samplerObject.AddComponent<MeshCollider>();
            sampler.sharedMesh = colliderMesh;
            Physics.SyncTransforms();
            sourceBounds = sampler.bounds;

            if (sampler.sharedMesh == null || sourceBounds.size.x <= Mathf.Epsilon || sourceBounds.size.z <= Mathf.Epsilon)
            {
                error = "Source mesh needs non-zero X and Z bounds.";
                DestroyImmediate(samplerObject);
                samplerObject = null;

                if (temporaryMesh != null)
                {
                    DestroyImmediate(temporaryMesh);
                    temporaryMesh = null;
                }

                return false;
            }

            error = null;
            return true;
        }

        private string GetValidationError()
        {
            if (meshSource == null)
            {
                return "Mesh Source is required.";
            }

            if (targetTerrain == null)
            {
                return "Target Terrain is required.";
            }

            if (targetTerrain.terrainData == null)
            {
                return "Target Terrain has no TerrainData.";
            }

            return TryResolveMeshSource(out _, out string error) ? null : error;
        }

        private bool TryResolveMeshSource(out MeshSourceDescriptor descriptor, out string error)
        {
            descriptor = default;

            if (meshSource is Mesh meshAsset)
            {
                if (meshAsset == null)
                {
                    error = "Selected Mesh asset is missing.";
                    return false;
                }

                descriptor = new MeshSourceDescriptor(meshAsset, null, null);
                error = null;
                return true;
            }

            if (meshSource is MeshFilter meshFilter)
            {
                return TryUseMeshFilter(meshFilter, out descriptor, out error);
            }

            if (meshSource is SkinnedMeshRenderer skinnedMeshRenderer)
            {
                return TryUseSkinnedMeshRenderer(skinnedMeshRenderer, out descriptor, out error);
            }

            if (meshSource is GameObject gameObject)
            {
                if (TryFindMeshFilter(gameObject, out MeshFilter childMeshFilter))
                {
                    return TryUseMeshFilter(childMeshFilter, out descriptor, out error);
                }

                if (TryFindSkinnedMeshRenderer(gameObject, out SkinnedMeshRenderer childSkinnedMeshRenderer))
                {
                    return TryUseSkinnedMeshRenderer(childSkinnedMeshRenderer, out descriptor, out error);
                }
            }

            error = "Mesh Source must be a Mesh asset, GameObject, MeshFilter, or SkinnedMeshRenderer.";
            return false;
        }

        private bool TryUseMeshFilter(MeshFilter meshFilter, out MeshSourceDescriptor descriptor, out string error)
        {
            descriptor = default;

            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                error = "Selected MeshFilter has no shared mesh.";
                return false;
            }

            descriptor = new MeshSourceDescriptor(meshFilter.sharedMesh, meshFilter.transform, null);
            error = null;
            return true;
        }

        private bool TryUseSkinnedMeshRenderer(
            SkinnedMeshRenderer skinnedMeshRenderer,
            out MeshSourceDescriptor descriptor,
            out string error)
        {
            descriptor = default;

            if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
            {
                error = "Selected SkinnedMeshRenderer has no shared mesh.";
                return false;
            }

            descriptor = new MeshSourceDescriptor(
                skinnedMeshRenderer.sharedMesh,
                skinnedMeshRenderer.transform,
                skinnedMeshRenderer);
            error = null;
            return true;
        }

        private bool TryFindMeshFilter(GameObject gameObject, out MeshFilter meshFilter)
        {
            if (gameObject.TryGetComponent(out meshFilter) && meshFilter.sharedMesh != null)
            {
                return true;
            }

            if (searchInChildren)
            {
                MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter childMeshFilter in meshFilters)
                {
                    if (childMeshFilter.sharedMesh != null)
                    {
                        meshFilter = childMeshFilter;
                        return true;
                    }
                }
            }

            meshFilter = null;
            return false;
        }

        private bool TryFindSkinnedMeshRenderer(GameObject gameObject, out SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if (gameObject.TryGetComponent(out skinnedMeshRenderer) && skinnedMeshRenderer.sharedMesh != null)
            {
                return true;
            }

            if (searchInChildren)
            {
                SkinnedMeshRenderer[] skinnedMeshRenderers =
                    gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

                foreach (SkinnedMeshRenderer childSkinnedMeshRenderer in skinnedMeshRenderers)
                {
                    if (childSkinnedMeshRenderer.sharedMesh != null)
                    {
                        skinnedMeshRenderer = childSkinnedMeshRenderer;
                        return true;
                    }
                }
            }

            skinnedMeshRenderer = null;
            return false;
        }

        private bool IsValidMeshSelection(Object selectedObject)
        {
            if (selectedObject is Mesh || selectedObject is MeshFilter || selectedObject is SkinnedMeshRenderer)
            {
                return true;
            }

            if (selectedObject is GameObject gameObject)
            {
                return TryFindMeshFilter(gameObject, out _) || TryFindSkinnedMeshRenderer(gameObject, out _);
            }

            return false;
        }

        private static bool TryGetTerrain(Object selectedObject, out Terrain terrain)
        {
            if (selectedObject is Terrain selectedTerrain)
            {
                terrain = selectedTerrain;
                return true;
            }

            if (selectedObject is GameObject gameObject && gameObject.TryGetComponent(out terrain))
            {
                return true;
            }

            terrain = null;
            return false;
        }

        private void SetStatus(string message, MessageType type)
        {
            statusMessage = message;
            statusType = type;
            Repaint();
        }

        private readonly struct MeshSourceDescriptor
        {
            public readonly Mesh Mesh;
            public readonly Transform Transform;
            public readonly SkinnedMeshRenderer SkinnedMeshRenderer;

            public MeshSourceDescriptor(
                Mesh mesh,
                Transform transform,
                SkinnedMeshRenderer skinnedMeshRenderer)
            {
                Mesh = mesh;
                Transform = transform;
                SkinnedMeshRenderer = skinnedMeshRenderer;
            }
        }
    }
}
