using System;
using Unity.Collections;

namespace vikwhite.ECS
{
    public static class SkillSlotEntryExtensions
    {
        public static T Get<T>(this in FixedList64Bytes<SkillSlotData<T>> list, SkillSlotType slot, T fallback = default)
            where T : unmanaged
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Type == slot) return list[i].Value;
            }
            return fallback;
        }

        public static bool TryFindSlot<T>(this in FixedList64Bytes<SkillSlotData<T>> list, T value, out SkillSlotType slot)
            where T : unmanaged, IEquatable<T>
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Value.Equals(value))
                {
                    slot = list[i].Type;
                    return true;
                }
            }
            slot = SkillSlotType.None;
            return false;
        }
    }
}
