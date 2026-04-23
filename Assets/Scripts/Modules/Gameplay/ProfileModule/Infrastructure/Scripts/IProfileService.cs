namespace vikwhite
{
    public interface IProfileService
    {
        ProfileData Data { get; }
        void Save();
        void Load();
    }
}