using System;
using UnityEngine;

namespace vikwhite
{
    public class ID<T> : Component where T : Enum
    {
        public T Current { get; }
        
        public ID(T id)
        {
            Current = id;
        }
    }
}