using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillEffectVisualEvent : IComponentData
    {
        public BlobAssetReference<SkillConfig> Skill;
        public Entity Target;
        public Entity Provider;
    }

    public static class SkillEffectVisualEventCommandBufferExtensions
    {
        public static void CreateSkillEffectVisualEvent(this EntityCommandBuffer ecb, BlobAssetReference<SkillConfig> skill, Entity target, Entity provider)
        {
            if (!skill.IsCreated || skill.Value.VFXPrefab == 0) return;
            ecb.CreateFrameEntity(new SkillEffectVisualEvent { Skill = skill, Target = target, Provider = provider });
        }
    }
}
