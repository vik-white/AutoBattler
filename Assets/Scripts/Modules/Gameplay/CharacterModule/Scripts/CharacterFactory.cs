namespace vikwhite
{
    public interface ICharacterFactory
    {
        Character Create(string id, int level);
    }
    
    public class CharacterFactory : ICharacterFactory
    {
        private readonly DiContainer _container;
        
        public CharacterFactory(DiContainer container)
        {
            _container = container;
        }

        public Character Create(string id, int level)
        {
            var character = _container.Resolve<Character>();
            character.Initialize(id, level);
            return character;
        }
    }
}