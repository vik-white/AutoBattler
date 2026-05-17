using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleWindowCharacterArgs
    {
        public Entity Character { get; }
        public CharacterConfigData Config { get; }
        public bool IsEnemy { get; }

        public BattleWindowCharacterArgs(Entity character, CharacterConfigData config, bool isEnemy)
        {
            Character = character;
            Config = config;
            IsEnemy = isEnemy;
        }
    }
}
