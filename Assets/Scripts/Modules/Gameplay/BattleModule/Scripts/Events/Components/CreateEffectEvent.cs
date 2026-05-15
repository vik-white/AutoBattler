using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CreateEffectEvent : IComponentData
    {
        public BlobAssetReference<SkillConfig> Skill;
        public Entity Target;
        public Entity Provider;
    }

    public static class EffectEventCommandBufferExtensions
    {
        public static void CreateEffectEvent(this EntityCommandBuffer ecb, BlobAssetReference<SkillConfig> skill, Entity target, Entity provider)
        {
            if (!skill.IsCreated || skill.Value.VFXPrefab == 0) return;
            ecb.CreateFrameEntity(new CreateEffectEvent { Skill = skill, Target = target, Provider = provider });
        }
    }
}
