using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class LevelUpConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }

    public class LevelUpConfigsAuthoringBaker : Baker<LevelUpConfigsAuthoring>
    {
        public override void Bake(LevelUpConfigsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var configs = new List<LevelUpConfig>();

            foreach (var levelUpData in authoring.Configs.LevelUp.GetAll())
            {
                configs.Add(new LevelUpConfig
                {
                    ID = levelUpData.ID.CalculateHash32(),
                    Damage = levelUpData.Damage,
                    Health = levelUpData.Health,
                    Shield = levelUpData.Shield,
                    Heal = levelUpData.Heal,
                });
            }

            AddComponent(entity, new LevelUpConfigsBlob
            {
                Value = CreateConfigsBlob(configs)
            });
        }

        private BlobAssetReference<BlobArrayContainer<LevelUpConfig>> CreateConfigsBlob(List<LevelUpConfig> configs)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArrayContainer<LevelUpConfig>>();
            var arrayBuilder = builder.Allocate(ref root.Array, configs.Count);
            for (int i = 0; i < configs.Count; i++)
                arrayBuilder[i] = configs[i];

            var blob = builder.CreateBlobAssetReference<BlobArrayContainer<LevelUpConfig>>(Allocator.Persistent);
            AddBlobAsset(ref blob, out _);
            return blob;
        }
    }
}
