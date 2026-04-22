namespace vikwhite
{
    public class SetSquadCharacterEvent
    {
        public int Index;
        public string ID;

        public SetSquadCharacterEvent(int index, string id)
        {
            Index = index;
            ID = id;
        }
    }
}