namespace vikwhite
{
    public class ChangeCharacterStarsEvent
    {
        public string ID;
        public int Stars;

        public ChangeCharacterStarsEvent(string id, int stars)
        {
            ID = id;
            Stars = stars;
        }
    }
}
