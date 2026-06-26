namespace vikwhite
{
    public interface IProfileService
    {
        ProfileData Data { get; }
        void SetAutoUseSkills(bool value);
        void Save();
        void Load();
    }
}
