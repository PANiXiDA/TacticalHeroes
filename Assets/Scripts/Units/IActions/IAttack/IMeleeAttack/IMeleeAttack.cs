using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.IActions
{

    public interface IMeleeAttack
    {
        public UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender);
    }
}
