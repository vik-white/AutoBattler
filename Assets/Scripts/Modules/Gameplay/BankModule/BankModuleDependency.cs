namespace vikwhite
{
    public class BankModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IBankService, BankService>();

            Register<ISummonWindow, SummonWindow>();
            Register<SummonWindowViewModel>();
            Register<SummonWindowView>();

            Register<SummonItemViewModel>();
            Register<SummonItemView>();
        }
    }
}
