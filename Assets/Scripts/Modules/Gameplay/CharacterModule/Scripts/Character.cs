namespace vikwhite
{
    public class Character
    {
        private string _id;
        private int _level;
        
        public string ID => _id;
        public int Level => _level;

        public Character(string id, int level)
        {
            _id = id;
            _level = level;
        }
    }
}