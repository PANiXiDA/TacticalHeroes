namespace Assets.Scripts.IActions
{
    public interface IDamage
    {
        (int damage, int deathCount) CalculateDamageAndDeathUnit(BaseUnit attacker, BaseUnit defender);
        int CalculateDamage(BaseUnit attacker, BaseUnit defender);
        int CalculateDeathCount(BaseUnit defender, int damage);
        bool isLuck { get; set; }
    }
}
