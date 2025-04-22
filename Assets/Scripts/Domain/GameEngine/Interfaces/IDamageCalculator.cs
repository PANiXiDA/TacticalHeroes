using Assets.Scripts.GameEngine.DTO.DamageCalculator;

namespace Assets.Scripts.GameEngine.Interfaces
{
    public interface IDamageCalculator
    {
        int CalculateDamage(DamageContext context);
        DamageResult ComputeCasualties(DefenderTakeDamageContext context);
    }
}
