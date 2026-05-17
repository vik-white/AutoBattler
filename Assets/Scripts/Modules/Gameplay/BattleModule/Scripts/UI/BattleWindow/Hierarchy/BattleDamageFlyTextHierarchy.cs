using System;
using TMPro;
using UnityEngine;

namespace vikwhite
{
    public class BattleDamageFlyTextHierarchy : MonoBehaviour
    {
        public TMP_Text Text;

        public event Action Updated;

        private void Update()
        {
            Updated?.Invoke();
        }
    }
}
