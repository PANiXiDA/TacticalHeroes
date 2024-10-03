using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.IActions
{
    public interface ITakeDamage
    {
        public UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator);
        public UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator);
    }
}
