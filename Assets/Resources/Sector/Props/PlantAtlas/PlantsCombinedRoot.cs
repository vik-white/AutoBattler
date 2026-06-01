using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Маркер на корневом GameObject, который держит объединённые меши растений
/// и пальм. Под этим объектом лежат до двух детей — <c>Palms</c> и
/// <c>Plants</c> — со своими <see cref="MeshFilter"/>/<see cref="MeshRenderer"/>
/// и соответствующим preset-материалом.
///
/// Сохраняет список отключённых при сборке оригиналов, чтобы Restore
/// возвращал в active только то, что выключил сам инструмент, а не любые
/// неактивные объекты на сцене.
///
/// MaterialPropertyBlock сознательно не используется — это сломало бы
/// SRP Batcher на Wind.shadergraph. Параметры ветра живут в материалах.
/// </summary>
[DisallowMultipleComponent]
public sealed class PlantsCombinedRoot : MonoBehaviour
{
    [Tooltip("Сколько отдельных растений (Palm+Plant) объединили на этом корне.")]
    public int sourceCount;

    [Tooltip("GameObject'ы, отключённые инструментом при сборке.\n" +
             "Используется для корректного Restore: возвращаем именно их.")]
    public List<GameObject> disabledOriginals = new List<GameObject>();

#if UNITY_EDITOR
    [Tooltip("Asset-путь объединённого меша palm-preset (Editor-only).")]
    public string palmMeshAssetPath;

    [Tooltip("Asset-путь объединённого меша plant-preset (Editor-only).")]
    public string plantMeshAssetPath;
#endif
}
