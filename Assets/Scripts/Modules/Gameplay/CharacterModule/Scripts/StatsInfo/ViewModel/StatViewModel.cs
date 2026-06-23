using UniRx;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public class StatInfoModel
    {
        public string Title { get; }
        public StatType Type { get; }

        public StatInfoModel(string title, StatType type)
        {
            Title = title;
            Type = type;
        }
    }

    public class StatViewModel : ViewModel<StatInfoModel>
    {
        private readonly ReactiveProperty<string> _amount = new();
        private readonly ReactiveProperty<string> _amountUpgrade = new();

        public string Title => Model.Title;
        public StatType Type => Model.Type;
        public IReadOnlyReactiveProperty<string> Amount => _amount;
        public IReadOnlyReactiveProperty<string> AmountUpgrade => _amountUpgrade;

        public StatViewModel(StatInfoModel model) : base(model)
        {
            AddDisposables(_amount, _amountUpgrade);
        }

        public void UpdateValue(float currentValue, float selectedValue)
        {
            _amount.Value = FormatValue(currentValue);
            _amountUpgrade.Value = FormatUpgrade(selectedValue - currentValue);
        }

        private string FormatValue(float value)
        {
            return IsPercentStat() ? $"{value * 100f:0.#}%" : FormatNumber(value);
        }

        private string FormatUpgrade(float value)
        {
            var prefix = value >= 0 ? "+" : "";
            return IsPercentStat() ? $"{prefix}{value * 100f:0.#}%" : $"{prefix}{FormatNumber(value)}";
        }

        private static string FormatNumber(float value)
        {
            var rounded = Mathf.Round(value);
            return Mathf.Approximately(value, rounded) ? Mathf.RoundToInt(value).ToString() : $"{value:0.#}";
        }

        private bool IsPercentStat() => Type == StatType.CritChance || Type == StatType.CritValue;
    }
}
