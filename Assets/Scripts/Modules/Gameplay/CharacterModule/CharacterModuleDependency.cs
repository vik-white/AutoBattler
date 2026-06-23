namespace vikwhite
{
    public class CharacterModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ICharacterFactory, CharacterFactory>();
            Register<ICharactersService, CharactersService>();
            Register<Character>();
            
            Register<ICharacterWindow, CharacterWindow>();
            Register<CharacterWindowViewModel>();
            Register<CharacterWindowView>();

            Register<ICharacterSkillsWindow, CharacterSkillsWindow>();
            Register<CharacterSkillsWindowViewModel>();
            Register<CharacterSkillsWindowView>();
            Register<SkillItemViewModel>();
            Register<SkillItemView>();

            Register<ICharacterUpgradeWindow, CharacterUpgradeWindow>();
            Register<CharacterUpgradeWindowViewModel>();
            Register<CharacterUpgradeWindowView>();

            Register<ICharacterAscendWindow, CharacterAscendWindow>();
            Register<CharacterAscendWindowViewModel>();
            Register<CharacterAscendWindowView>();
            Register<StarsViewModel>();
            Register<StarsView>();

            Register<StatsInfoViewModel>();
            Register<StatsInfoView>();
            Register<StatViewModel>();
            Register<StatView>();
            
            Register<IRedeemShardWindow, RedeemShardWindow>();
            Register<RedeemShardWindowViewModel>();
            Register<RedeemShardWindowView>();

            Register<IRedeemBookWindow, RedeemBookWindow>();
            Register<RedeemBookWindowViewModel>();
            Register<RedeemBookWindowView>();
        }
    }
}
