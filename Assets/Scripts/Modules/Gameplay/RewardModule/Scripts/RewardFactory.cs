using System.Collections.Generic;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRewardFactory
    {
        List<Reward> Create(string id);
        List<Reward> Create(string id, int count);
    }

    public class RewardFactory : IRewardFactory
    {
        private static readonly CharacterClassType[] AllCharacterClasses =
            (CharacterClassType[])System.Enum.GetValues(typeof(CharacterClassType));
        private static readonly RarityType[] AllRarities =
            (RarityType[])System.Enum.GetValues(typeof(RarityType));

        private readonly IConfigs _configs;

        public RewardFactory(IConfigs configs)
        {
            _configs = configs;
        }

        public List<Reward> Create(string id)
        {
            var result = new List<Reward>();
            var data = _configs.Rewards.Get(id);
            if (data == null) return result;

            foreach (var rewardData in data.Rewards)
            {
                if (rewardData.Probability < 1f && Random.value > rewardData.Probability) continue;

                var reward = CreateReward(rewardData);
                if (reward == null) continue;

                reward.Value = Random.Range(rewardData.MinValue, rewardData.MaxValue + 1);
                AddOrMerge(result, reward);
            }

            if (TryPickFromBasket(data.RewardBasket, out var basketReward))
                AddOrMerge(result, basketReward);

            return result;
        }

        public List<Reward> Create(string id, int count)
        {
            var result = new List<Reward>();
            for (int i = 0; i < count; i++)
            {
                foreach (var reward in Create(id))
                    AddOrMerge(result, reward);
            }
            return result;
        }

        private static void AddOrMerge(List<Reward> list, Reward reward)
        {
            if (reward == null) return;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsSameAs(reward))
                {
                    list[i].Value += reward.Value;
                    return;
                }
            }

            list.Add(reward);
        }

        private bool TryPickFromBasket(IReadOnlyCollection<RewardData> basket, out Reward picked)
        {
            picked = null;
            if (basket == null || basket.Count == 0) return false;

            var chosen = PickWeighted(basket);
            picked = CreateReward(chosen);
            if (picked == null) return false;

            picked.Value = Random.Range(chosen.MinValue, chosen.MaxValue + 1);
            return true;
        }

        private static RewardData PickWeighted(IReadOnlyCollection<RewardData> basket)
        {
            float totalWeight = 0f;
            foreach (var item in basket) totalWeight += Mathf.Max(0f, item.Probability);

            if (totalWeight <= 0f)
            {
                int targetIndex = Random.Range(0, basket.Count);
                int i = 0;
                RewardData fallback = default;
                foreach (var item in basket)
                {
                    fallback = item;
                    if (i++ == targetIndex) return item;
                }
                return fallback;
            }

            var roll = Random.value * totalWeight;
            float accum = 0f;
            RewardData last = default;
            foreach (var item in basket)
            {
                last = item;
                accum += Mathf.Max(0f, item.Probability);
                if (roll < accum) return item;
            }
            return last;
        }

        private Reward CreateReward(RewardData data)
        {
            switch (data.Type)
            {
                case RewardType.Res:
                    return new ResourceReward { ResourceType = data.ResourceType };
                case RewardType.Shard:
                    return new ShardReward { ID = data.ID };
                case RewardType.ShardGroup:
                    return new ShardReward { ID = ResolveShardGroup(data.ShardGroupType) };
                case RewardType.ClassShard:
                    return new ClassShardReward { Class = data.Class, Rarity = data.Rarity };
                case RewardType.ClassShardGroup:
                    switch (data.ShardGroupType)
                    {
                        case ShardGroupType.Any: return new ClassShardReward { Class = GetRandomCharacterClass(), Rarity = GetRandomRarity() };
                        case ShardGroupType.Class: return new ClassShardReward { Class = data.Class, Rarity = GetRandomRarity() };
                        case ShardGroupType.Rarity: return new ClassShardReward { Class = GetRandomCharacterClass(), Rarity = data.Rarity };
                        default: return null;
                    }
                default:
                    return null;
            }
        }

        private string ResolveShardGroup(ShardGroupType groupType)
        {
            switch (groupType)
            {
                case ShardGroupType.Any:
                    return PickRandomCharacterId(c => c.Squad);
                default:
                    return null;
            }
        }

        private string PickRandomCharacterId(System.Func<ICharacterData, bool> predicate)
        {
            var all = _configs.Characters.GetAll();
            string picked = null;
            int matches = 0;
            for (int i = 0; i < all.Count; i++)
            {
                var character = all[i];
                if (!predicate(character)) continue;

                matches++;
                if (Random.Range(0, matches) == 0) picked = character.ID;
            }
            return picked;
        }

        private CharacterClassType GetRandomCharacterClass()
        {
            return AllCharacterClasses[Random.Range(0, AllCharacterClasses.Length)];
        }

        private RarityType GetRandomRarity()
        {
            return AllRarities[Random.Range(0, AllRarities.Length)];
        }
    }
}
