namespace vikwhite
{
    public class SetSectorLocationEvent
    {
        public string PreviousID;
        public string ID;

        public SetSectorLocationEvent(string previousId, string id)
        {
            PreviousID = previousId;
            ID = id;
        }
    }
}
