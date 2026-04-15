using System.Collections.Generic;
using Rukhanka.Toolbox;
using UnityEngine;
using vikwhite.ECS;

public class PrefabSpawner : MonoBehaviour
{
    public List<GameObject> Prefabs;

    private void Awake()
    {
        CreatePrefabEventSystem.OnExecute += CreatePrefab;
    }

    private void CreatePrefab(CreatePrefabEvent evnt)
    {
        var prefab = Prefabs.Find(e => e.name.CalculateHash32() == evnt.ID);
        var go = Instantiate(prefab);
        go.transform.position = evnt.Position;
    }
}
