using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;


namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class NoResponseAttack : DefaultMeleeAttack, IMeleeAttack
    {
        public NoResponseAttack(IDamage damageCalculator) : base(damageCalculator) { }

        protected override UniTask ResponseAttack(BaseUnit attacker, BaseUnit defender, bool isCanAttack)
        {
            return UniTask.CompletedTask;
        }
    }
}
