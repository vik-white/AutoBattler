using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite
{
    public class FollowEntity : MonoBehaviour
    {
        private Entity _entity;
        private EntityManager _entityManager;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Update()
        {
            if (!_entityManager.Exists(_entity))
            {
                Destroy(gameObject);
                return;
            }
            transform.position = _entityManager.GetComponentData<LocalTransform>(_entity).Position;
        }
    }
}