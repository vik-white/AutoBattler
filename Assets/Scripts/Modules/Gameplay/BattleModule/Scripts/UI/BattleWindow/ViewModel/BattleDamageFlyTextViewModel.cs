using UnityEngine;

namespace vikwhite
{
    public class BattleDamageFlyTextViewModel : ViewModel<BattleDamageFlyTextModel>
    {
        private const float Lifetime = 0.8f;

        private float _elapsed;

        public Vector3 Position => Model.Position;
        public string Text => Mathf.CeilToInt(Model.Damage).ToString();
        public bool IsEnemyTarget => Model.IsEnemyTarget;
        public bool IsCrit => Model.IsCrit;
        public float Progress { get; private set; }
        public bool IsComplete { get; private set; }

        public BattleDamageFlyTextViewModel(BattleDamageFlyTextModel model) : base(model) { }

        public void Tick(float deltaTime)
        {
            if (IsComplete) return;

            _elapsed += deltaTime;
            Progress = Mathf.Clamp01(_elapsed / Lifetime);
            IsComplete = _elapsed >= Lifetime;
        }
    }
}
