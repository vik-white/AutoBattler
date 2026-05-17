using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleWindowCharacterModel
    {
        public Entity Character { get; }
        public CharacterConfigData Config { get; }
        public bool IsEnemy { get; }

        public BattleWindowCharacterModel(Entity character, CharacterConfigData config, bool isEnemy)
        {
            Character = character;
            Config = config;
            IsEnemy = isEnemy;
        }
    }
}
