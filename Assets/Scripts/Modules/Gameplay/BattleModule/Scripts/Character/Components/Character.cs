using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Character : IComponentData
    {
        public BlobAssetReference<CharacterConfigData> Config;
        public int Level;
    }

    public static class CharacterExtensions
    {
        public static CharacterConfigData GetConfig(this in Character character)
        {
            return character.Config.Value;
        }
    }
}
