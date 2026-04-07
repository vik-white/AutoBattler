namespace vikwhite
{
    public interface IProfileService
    {
        ProfileData Data { get; }
    }

    public class ProfileService : IProfileService
    {
        public ProfileData Data { get; private set; } = new();
    }
}