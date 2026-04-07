namespace vikwhite
{
    public class ProfileHandler<T> : EventHandler<T>
    {
        protected readonly IProfileService _profile = DI.Resolve<IProfileService>();
        
        protected override void Handle(T evnt) { }
    }
}