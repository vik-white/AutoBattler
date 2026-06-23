using System.Collections.Generic;

namespace vikwhite
{
    public interface ICharacterFactory
    {
        Character Create(string id, int level, int shards, int stars, IReadOnlyList<SkillData> skills);
    }
    
    public class CharacterFactory : ICharacterFactory
    {
        private readonly DiContainer _container;
        
        public CharacterFactory(DiContainer container)
        {
            _container = container;
        }

        public Character Create(string id, int level, int shards, int stars, IReadOnlyList<SkillData> skills)
        {
            var character = _container.Resolve<Character>();
            character.Initialize(id, level, shards, stars, skills);
            return character;
        }
    }
}
