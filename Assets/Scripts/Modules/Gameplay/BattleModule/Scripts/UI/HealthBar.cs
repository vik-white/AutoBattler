using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite
{
    public class HealthBar : MonoBehaviour
    {
        private Entity _character;
        private EntityManager _entityManager;
        
        public void Initialize(Entity character)
        {
            _character = character;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Update()
        {
            if (!_entityManager.Exists(_character)) return;
            var position = _entityManager.GetComponentData<LocalTransform>(_character).Position;
            transform.position = Camera.main.WorldToScreenPoint(position);
        }
    }
}