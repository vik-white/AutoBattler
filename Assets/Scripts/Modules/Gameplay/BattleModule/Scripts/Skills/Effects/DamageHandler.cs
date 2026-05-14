namespace vikwhite.ECS
{
    public static class DamageHandler
    {
        public static float CalculateDamage(float rawAttack, float defense)
        {
            return rawAttack * rawAttack / (rawAttack + 5f * defense);
        }
    }
}
