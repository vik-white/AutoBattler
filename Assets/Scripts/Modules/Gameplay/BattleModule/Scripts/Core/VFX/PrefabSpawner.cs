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
        public GameObject DamageFlyTextPrefab;

        private void Awake()
        {
            CreatePrefabEventSystem.OnExecute += CreatePrefab;
            CreateFollowPrefabEventSystem.OnExecute += CreateFollowPrefab;
            CreateDamageFlyTextEventSystem.OnExecute += CreateDamageFlyText;
        }

        private void OnDestroy()
        {
            CreatePrefabEventSystem.OnExecute -= CreatePrefab;
            CreateFollowPrefabEventSystem.OnExecute -= CreateFollowPrefab;
            CreateDamageFlyTextEventSystem.OnExecute -= CreateDamageFlyText;
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

        private void CreateDamageFlyText(CreateDamageFlyTextEvent evnt)
        {
            if (DamageFlyTextPrefab == null) return;

            var canvas = FindAnyObjectByType<Canvas>();
            var go = canvas != null
                ? Instantiate(DamageFlyTextPrefab, canvas.transform, false)
                : Instantiate(DamageFlyTextPrefab);

            var damageFlyText = go.GetComponent<DamageFlyText>();
            if (damageFlyText == null)
            {
                Destroy(go);
                return;
            }

            damageFlyText.Initialize(new Vector3(evnt.Position.x, evnt.Position.y, evnt.Position.z), evnt.Damage, evnt.IsEnemyTarget);
        }
    }
}
