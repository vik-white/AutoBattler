namespace vikwhite
{
    public interface ICharacterFactory
    {
        Character Create(string id, int level);
    }
    
    public class CharacterFactory : ICharacterFactory
    {
        /*private readonly IDiContainer _container;
        
        public CharacterFactory(IDiContainer container)
        {
            _container = container;
        }*/

        public Character Create(string id, int level)
        {
            var character = DI.Resolve<Character>();
            character.Initialize(id, level);
            return character;
        }
    }
}