using UnityEngine;

namespace vikwhite
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<CoroutineRunner>();

                    if (_instance == null)
                    {
                        GameObject go = new GameObject("CoroutineRunner");
                        _instance = go.AddComponent<CoroutineRunner>();
                    }
                }

                return _instance;
            }
        }
    }
}