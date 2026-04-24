using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite
{
    public class FollowEntity : MonoBehaviour
    {
        private Entity _entity;
        private EntityManager _entityManager;
        private bool _isInitialized;
        
        public void Initialize(Entity entity, EntityManager entityManager)
        {
            _entity = entity;
            _entityManager = entityManager;
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized) return;
            if (!_entityManager.Exists(_entity))
            {
                Destroy(gameObject);
                return;
            }
            transform.position = _entityManager.GetComponentData<LocalTransform>(_entity).Position;
        }
    }
}
