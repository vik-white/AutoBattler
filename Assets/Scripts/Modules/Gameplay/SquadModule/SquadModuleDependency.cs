namespace vikwhite
{
    public class SquadModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ISquadWindow, SquadWindow>();
            Register<SquadWindowViewModel>();
            Register<SquadWindowView>();
            
            Register<ICardViewFactory, CardViewFactory>();
            Register<CardViewModel>();
            Register<CardView>();
        }
    }
}