using Unity.Entities;
using UnityEngine;
using UnityTime = UnityEngine.Time;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(TimeSystemGroup))]
    [UpdateAfter(typeof(InitializeTimeSystem))]
    public partial struct TimeSystem : ISystem
    {
        private static float _timeScaleBeforePause = 1f;

        public static float DeltaTime { get; private set; }
        public static float UnscaledDeltaTime { get; private set; }
        public static bool IsPaused { get; private set; }

        public void OnCreate(ref SystemState state)
        {
            var entity = state.EntityManager.CreateEntity();
            var time = new Time();
            Reset(ref time);
            state.EntityManager.AddComponentData(entity, time);
        }

        public void OnDestroy(ref SystemState state)
        {
            IsPaused = false;
            DeltaTime = 0f;
            UnscaledDeltaTime = 0f;
            UnityTime.timeScale = 1f;
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var time in SystemAPI.Query<RefRW<Time>>())
            {
                time.ValueRW.DeltaTime = time.ValueRO.IsPaused ? 0f : SystemAPI.Time.DeltaTime;
                time.ValueRW.TotalTime += time.ValueRO.DeltaTime;

                DeltaTime = time.ValueRO.DeltaTime;
                UnscaledDeltaTime = UnityTime.unscaledDeltaTime;
                IsPaused = time.ValueRO.IsPaused;

                if (!IsPaused && UnityTime.timeScale > 0f)
                    _timeScaleBeforePause = UnityTime.timeScale;
            }
        }

        public static void Reset()
        {
            if (TryGetTime(out var entityManager, out var entity, out var time))
            {
                Reset(ref time);
                entityManager.SetComponentData(entity, time);
                return;
            }

            ResetStaticState();
        }

        public static bool TogglePause()
        {
            Debug.Log("TogglePause");
            SetPaused(!IsPaused);
            return IsPaused;
        }

        public static void SetPaused(bool isPaused)
        {
            if (TryGetTime(out var entityManager, out var entity, out var time))
            {
                time.IsPaused = isPaused;
                time.DeltaTime = 0f;
                entityManager.SetComponentData(entity, time);
            }

            IsPaused = isPaused;
            DeltaTime = 0f;

            if (isPaused)
            {
                if (UnityTime.timeScale > 0f)
                    _timeScaleBeforePause = UnityTime.timeScale;
                UnityTime.timeScale = 0f;
            }
            else
            {
                UnityTime.timeScale = _timeScaleBeforePause > 0f ? _timeScaleBeforePause : 1f;
            }
        }

        public static void Reset(ref Time time)
        {
            time.TotalTime = 0f;
            time.DeltaTime = 0f;
            time.IsPaused = false;
            ResetStaticState();
        }

        private static void ResetStaticState()
        {
            DeltaTime = 0f;
            UnscaledDeltaTime = 0f;
            IsPaused = false;
            _timeScaleBeforePause = 1f;
            UnityTime.timeScale = 1f;
        }

        private static bool TryGetTime(out EntityManager entityManager, out Entity entity, out Time time)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
            {
                entityManager = default;
                entity = Entity.Null;
                time = default;
                return false;
            }

            entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<Time>());
            if (query.IsEmptyIgnoreFilter)
            {
                query.Dispose();
                entity = Entity.Null;
                time = default;
                return false;
            }

            entity = query.GetSingletonEntity();
            time = entityManager.GetComponentData<Time>(entity);
            query.Dispose();
            return true;
        }
    }
}
