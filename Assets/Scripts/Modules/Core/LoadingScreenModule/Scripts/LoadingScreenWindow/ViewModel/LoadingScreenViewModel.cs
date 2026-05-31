using System;
using UnityEngine;

namespace vikwhite
{
    public class LoadingScreenViewModel : WindowViewModel
    {
        public event Action<float> OnProgressChanged;
        public float Progress { get; private set; }

        public void SetProgress(float progress)
        {
            Progress = Mathf.Clamp01(progress);
            OnProgressChanged?.Invoke(Progress);
        }
    }
}
