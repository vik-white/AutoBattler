using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterAscendWindowViewModel : WindowViewModel<Character>
    {
        private readonly IConfigs _configs;
        private readonly ReactiveProperty<int> _selectedStars;
        private readonly ReactiveProperty<bool> _canSelectPreviousStar = new();
        private readonly ReactiveProperty<bool> _canSelectNextStar = new();
        private readonly ReactiveProperty<string> _shardPrice = new();
        private readonly ReactiveProperty<float> _shardProgress = new();
        private readonly ResourceType _redeemResourceType;

        public readonly string Name;
        public readonly Sprite Image;
        public readonly Sprite ClassIcon;
        public readonly Sprite ShardIcon;
        public readonly Sprite HeroShardIcon;

        public IReadOnlyReactiveProperty<bool> CanSelectPreviousStar => _canSelectPreviousStar;
        public IReadOnlyReactiveProperty<bool> CanSelectNextStar => _canSelectNextStar;
        public IReadOnlyReactiveProperty<string> ShardPrice => _shardPrice;
        public IReadOnlyReactiveProperty<float> ShardProgress => _shardProgress;
        public IReadOnlyReactiveProperty<int> Might;
        public ResourceViewModel RedeemResource { get; }
        public StarsViewModel Stars { get; }
        public StatsInfoViewModel StatsInfo { get; }
        public UnityAction OnAscend;
        public UnityAction OnSummon;
        public UnityAction OnRedeem;
        public UnityAction OnSelectPreviousStar;
        public UnityAction OnSelectNextStar;

        public CharacterAscendWindowViewModel(Character character, IConfigs configs, IResourceService resourceService, ISummonWindow summonWindow, IRedeemShardWindow redeemShardWindow) : base(character)
        {
            _configs = configs;
            _redeemResourceType = ResourceHandler.GetShardResourceType(character.Config.Rarity);

            Name = character.Config.Name;
            Image = character.Config.Image;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            ShardIcon = _configs.UI.Rarities[character.Config.Rarity].Shard;
            HeroShardIcon = character.Config.ShardImage;
            Might = character.Might;

            _selectedStars = new ReactiveProperty<int>(GetInitialSelectedStars(character));
            AddDisposables(_selectedStars, _canSelectPreviousStar, _canSelectNextStar, _shardPrice, _shardProgress);

            RedeemResource = CreateViewModel<ResourceViewModel, Resource>(resourceService.Get(_redeemResourceType));
            Stars = CreateViewModel<StarsViewModel, StarsModel>(new StarsModel(character.Stars, _selectedStars));
            StatsInfo = CreateViewModel<StatsInfoViewModel, StatsInfoModel>(new StatsInfoModel(character, character.Level, character.Stars, character.Level, _selectedStars));

            OnAscend = Ascend;
            OnSummon = summonWindow.ShowWindow;
            OnRedeem = () => redeemShardWindow.ShowWindow(character);
            OnSelectPreviousStar = SelectPreviousStar;
            OnSelectNextStar = SelectNextStar;

            AddDisposable(character.Stars.Subscribe(UpdateSelectedStars));
            AddDisposable(character.Shards.Subscribe(_ => RefreshShardState()));
        }

        public override void Dispose()
        {
            base.Dispose();
            OnAscend = null;
            OnSummon = null;
            OnRedeem = null;
            OnSelectPreviousStar = null;
            OnSelectNextStar = null;
        }

        private void Ascend()
        {
            if (Model.Stars.Value >= GetMaxStars()) return;
            var price = _configs.Settings.StarUpPrice;
            if (price > 0 && Model.Shards.Value < price)return;
            if (price > 0) Model.RemoveShards(price);
            Model.UpgradeStars();
        }

        private void SelectPreviousStar()
        {
            if (_selectedStars.Value <= Model.Stars.Value) return;
            _selectedStars.Value--;
            RefreshStarSelectionState();
        }

        private void SelectNextStar()
        {
            if (_selectedStars.Value >= GetMaxStars())return;
            _selectedStars.Value++;
            RefreshStarSelectionState();
        }

        private void UpdateSelectedStars(int currentStars)
        {
            if (_selectedStars.Value <= currentStars)
                _selectedStars.Value = GetInitialSelectedStars(Model);
            else
                _selectedStars.Value = Mathf.Clamp(_selectedStars.Value, currentStars, GetMaxStars());

            RefreshStarSelectionState();
            RefreshShardState();
        }

        private void RefreshStarSelectionState()
        {
            _canSelectPreviousStar.Value = _selectedStars.Value > Model.Stars.Value;
            _canSelectNextStar.Value = _selectedStars.Value < GetMaxStars();
        }

        private void RefreshShardState()
        {
            var price = _configs.Settings.StarUpPrice;
            _shardPrice.Value = $"{Model.Shards.Value}/{price}";
            _shardProgress.Value = price <= 0 ? 1f : Mathf.Clamp01((float)Model.Shards.Value / price);
        }

        private int GetInitialSelectedStars(Character character) => Mathf.Clamp(character.Stars.Value + 1, character.Stars.Value, GetMaxStars());

        private int GetMaxStars() => _configs.Stars.GetAll().Count;
    }
}
