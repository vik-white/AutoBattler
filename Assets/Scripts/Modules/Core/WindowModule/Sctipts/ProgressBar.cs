using UnityEngine;

namespace vikwhite
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Transform _bar;

        public void SetProgress(float value) {
            if (value < 0) value = 0;
            if (value > 1) value = 1;
            _bar.localScale = new Vector3(value, 1, 1);
        }
    }
}