using UnityEngine;

namespace vikwhite
{
    public class CoroutineRunner : MonoBehaviour
    {
        public static CoroutineRunner Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}