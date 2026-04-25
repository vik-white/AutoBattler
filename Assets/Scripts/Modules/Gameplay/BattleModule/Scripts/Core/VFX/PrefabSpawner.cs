using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Entities;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public class PrefabSpawner : MonoBehaviour
    {
        public List<GameObject> Prefabs;

        private void Awake()
        {
            CreatePrefabEventSystem.OnExecute += CreatePrefab;
            CreateFollowPrefabEventSystem.OnExecute += CreateFollowPrefab;
        }

        private void OnDestroy()
        {
            CreatePrefabEventSystem.OnExecute -= CreatePrefab;
            CreateFollowPrefabEventSystem.OnExecute -= CreateFollowPrefab;
        }

        private void CreatePrefab(CreatePrefabEvent evnt)
        {
            var prefab = Prefabs.Find(e => e.name.CalculateHash32() == evnt.ID);
            var go = Instantiate(prefab);
            go.transform.position = evnt.Position;
        }
        
        private void CreateFollowPrefab(CreateFollowPrefabEvent evnt)
        {
            var prefab = Prefabs.Find(e => e.name.CalculateHash32() == evnt.ID);
            var go = Instantiate(prefab);
            go.transform.position = evnt.Position;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
            {
                Destroy(go);
                Debug.LogWarning("CreateFollowPrefab skipped because DefaultGameObjectInjectionWorld is not ready.");
                return;
            }

            go.GetComponent<FollowEntity>().Initialize(evnt.Entity, world.EntityManager);
        }
    }
}
