using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct LevelUpConfig : IID
    {
        public uint ID { get; set; }
        public float Damage;
        public float Health;
        public float Shield;
        public float Heal;
    }
}
