using UnityEngine;

namespace vikwhite
{
    public class DiSceneContext : MonoBehaviour
    {
        private DiAggregator _aggregator;

        public void Initialize(DiAggregator aggregator) => _aggregator = aggregator;

        private void Update() => _aggregator.Update();
    }
}