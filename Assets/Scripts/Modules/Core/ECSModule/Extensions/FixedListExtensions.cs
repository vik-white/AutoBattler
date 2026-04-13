namespace vikwhite.ECS
{
    public static class FixedListExtensions
    {
        public static bool Contains<T>(this Unity.Collections.FixedList64Bytes<T> list, T value)
            where T : unmanaged
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Equals(value))
                    return true;
            }
            return false;
        }
    }
}